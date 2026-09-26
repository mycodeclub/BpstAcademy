using System.Text.Json;
using BpstEdu.Application.Abstractions;
using BpstEdu.Application.Security;
using BpstEdu.Domain.Common;
using BpstEdu.Infrastructure.Persistence;
using BpstEdu.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BpstEdu.IntegrationTests;

/// <summary>Audit fields, soft delete, concurrency token and audit log, on a sample table in its own database.</summary>
[Collection(AppCollection.Name)]
public class AuditingTests(AppFactory factory) : IAsyncLifetime
{
    private readonly FakeUser _user = new();
    private readonly FakeClock _clock = new();
    private string _connectionString = "";

    public async ValueTask InitializeAsync()
    {
        var name = "audit_" + Guid.NewGuid().ToString("N")[..8];
        await using (var admin = new NpgsqlConnection(factory.ConnectionString))
        {
            await admin.OpenAsync();
            await using var create = new NpgsqlCommand($"CREATE DATABASE {name}", admin);
            await create.ExecuteNonQueryAsync();
        }
        _connectionString = new NpgsqlConnectionStringBuilder(factory.ConnectionString) { Database = name }.ConnectionString;
        await using var db = NewContext();
        await db.Database.EnsureCreatedAsync();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Create_update_delete_are_stamped_soft_deleted_and_logged()
    {
        var ct = TestContext.Current.CancellationToken;
        var sample = new Sample { Name = "First" };

        _user.UserId = "u-create";
        await using (var db = NewContext())
        {
            db.Samples.Add(sample);
            await db.SaveChangesAsync(ct);
        }

        _user.UserId = "u-update";
        _clock.UtcNow = _clock.UtcNow.AddMinutes(5);
        await using (var db = NewContext())
        {
            var s = await db.Samples.SingleAsync(x => x.Id == sample.Id, ct);
            s.Name = "Second";
            await db.SaveChangesAsync(ct);
        }

        _user.UserId = "u-delete";
        await using (var db = NewContext())
        {
            db.Samples.Remove(await db.Samples.SingleAsync(x => x.Id == sample.Id, ct));
            await db.SaveChangesAsync(ct);
        }

        await using var check = NewContext();
        Assert.False(await check.Samples.AnyAsync(x => x.Id == sample.Id, ct));

        var row = await check.Samples.IgnoreQueryFilters().SingleAsync(x => x.Id == sample.Id, ct);
        Assert.Equal("Second", row.Name);
        Assert.Equal("u-create", row.CreatedBy);
        Assert.Equal("u-update", row.UpdatedBy);
        Assert.True(row.IsDeleted);
        Assert.Equal("u-delete", row.DeletedBy);

        var log = await check.AuditEntries.Where(a => a.EntityId == sample.Id.ToString()).OrderBy(a => a.Id).ToListAsync(ct);
        Assert.Equal([AuditAction.Created, AuditAction.Updated, AuditAction.Deleted], log.Select(a => a.Action));
        using var changes = JsonDocument.Parse(log[1].Changes!);
        var name = changes.RootElement.GetProperty("Name");
        Assert.Equal("First", name.GetProperty("old").GetString());
        Assert.Equal("Second", name.GetProperty("new").GetString());
    }

    [Fact]
    public async Task Concurrent_edits_of_the_same_row_are_rejected()
    {
        var ct = TestContext.Current.CancellationToken;
        var sample = new Sample { Name = "Original" };
        await using (var db = NewContext())
        {
            db.Samples.Add(sample);
            await db.SaveChangesAsync(ct);
        }

        await using var first = NewContext();
        await using var second = NewContext();
        var a = await first.Samples.SingleAsync(x => x.Id == sample.Id, ct);
        var b = await second.Samples.SingleAsync(x => x.Id == sample.Id, ct);

        a.Name = "Edited by A";
        await first.SaveChangesAsync(ct);
        b.Name = "Edited by B";

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => second.SaveChangesAsync(ct));
    }

    private TestDbContext NewContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseNpgsql(_connectionString)
            .UseSnakeCaseNamingConvention()
            .AddInterceptors(new AuditingInterceptor(_user, _clock))
            .Options;
        return new TestDbContext(options);
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : AppDbContext(options)
    {
        public DbSet<Sample> Samples => Set<Sample>();
    }

    private sealed class Sample : AuditableEntity
    {
        public string Name { get; set; } = "";
    }

    private sealed class FakeUser : ICurrentUser
    {
        public string? UserId { get; set; }
        public bool IsAuthenticated => UserId is not null;
    }

    private sealed class FakeClock : IClock
    {
        public DateTimeOffset UtcNow { get; set; } = new(2026, 9, 26, 6, 30, 0, TimeSpan.Zero);
    }
}
