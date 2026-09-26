using System.Linq.Expressions;
using BpstEdu.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace BpstEdu.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>For contexts that extend this one (tests).</summary>
    protected AppDbContext(DbContextOptions options) : base(options) { }

    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Money is numeric(12,2) everywhere.
        configurationBuilder.Properties<decimal>().HavePrecision(12, 2);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clr = entityType.ClrType;

            if (typeof(ISoftDelete).IsAssignableFrom(clr))
            {
                // Hide soft-deleted rows from every query; use IgnoreQueryFilters() to see them (audit, restore).
                var p = Expression.Parameter(clr, "e");
                var filter = Expression.Lambda(Expression.Not(Expression.Property(p, nameof(ISoftDelete.IsDeleted))), p);
                modelBuilder.Entity(clr).HasQueryFilter(filter);
            }

            if (typeof(AuditableEntity).IsAssignableFrom(clr))
            {
                modelBuilder.Entity(clr).Property(nameof(AuditableEntity.Version)).HasColumnName("xmin").HasColumnType("xid").IsRowVersion();
                modelBuilder.Entity(clr).Property(nameof(AuditableEntity.CreatedBy)).HasMaxLength(64);
                modelBuilder.Entity(clr).Property(nameof(AuditableEntity.UpdatedBy)).HasMaxLength(64);
                modelBuilder.Entity(clr).Property(nameof(AuditableEntity.DeletedBy)).HasMaxLength(64);
            }
        }
    }
}
