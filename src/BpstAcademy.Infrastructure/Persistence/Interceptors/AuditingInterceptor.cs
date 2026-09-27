using System.Text.Json;
using BpstAcademy.Application.Abstractions;
using BpstAcademy.Application.Security;
using BpstAcademy.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BpstAcademy.Infrastructure.Persistence.Interceptors;

/// <summary>
/// On every save: fills audit fields, turns deletes into soft deletes, and writes one <see cref="AuditEntry"/>
/// per created / changed / deleted record, in the same transaction.
/// </summary>
public sealed class AuditingInterceptor(ICurrentUser currentUser, IClock clock) : SaveChangesInterceptor
{
    private static readonly HashSet<string> SkippedProperties =
    [
        nameof(AuditableEntity.CreatedAt), nameof(AuditableEntity.CreatedBy),
        nameof(AuditableEntity.UpdatedAt), nameof(AuditableEntity.UpdatedBy),
        nameof(AuditableEntity.IsDeleted), nameof(AuditableEntity.DeletedAt), nameof(AuditableEntity.DeletedBy),
        nameof(AuditableEntity.Version),
    ];

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Apply(eventData.Context);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        Apply(eventData.Context);
        return ValueTask.FromResult(result);
    }

    private void Apply(DbContext? context)
    {
        if (context is null) return;

        var now = clock.UtcNow;
        var userId = currentUser.UserId;
        var audit = new List<AuditEntry>();

        foreach (var entry in context.ChangeTracker.Entries<Entity>().ToList())
        {
            AuditAction action;
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity is IAuditable added)
                    {
                        added.CreatedAt = now;
                        added.CreatedBy = userId;
                    }
                    action = AuditAction.Created;
                    break;

                case EntityState.Modified:
                    if (entry.Entity is IAuditable modified)
                    {
                        modified.UpdatedAt = now;
                        modified.UpdatedBy = userId;
                    }
                    action = AuditAction.Updated;
                    break;

                case EntityState.Deleted when entry.Entity is ISoftDelete deleted:
                    entry.State = EntityState.Modified;
                    deleted.IsDeleted = true;
                    deleted.DeletedAt = now;
                    deleted.DeletedBy = userId;
                    action = AuditAction.Deleted;
                    break;

                case EntityState.Deleted:
                    action = AuditAction.Deleted;
                    break;

                default:
                    continue;
            }

            audit.Add(new AuditEntry
            {
                At = now,
                UserId = userId,
                EntityType = entry.Metadata.ClrType.Name,
                EntityId = entry.Entity.Id.ToString(),
                Action = action,
                Changes = action == AuditAction.Deleted ? null : DescribeChanges(entry, action),
            });
        }

        if (audit.Count > 0) context.Set<AuditEntry>().AddRange(audit);
    }

    private static string? DescribeChanges(EntityEntry entry, AuditAction action)
    {
        var changes = new Dictionary<string, object?>();
        foreach (var p in entry.Properties)
        {
            var name = p.Metadata.Name;
            if (SkippedProperties.Contains(name) || p.Metadata.IsPrimaryKey()) continue;

            if (action == AuditAction.Created)
                changes[name] = new { @new = p.CurrentValue };
            else if (p.IsModified && !Equals(p.OriginalValue, p.CurrentValue))
                changes[name] = new { old = p.OriginalValue, @new = p.CurrentValue };
        }
        return changes.Count == 0 ? null : JsonSerializer.Serialize(changes);
    }
}
