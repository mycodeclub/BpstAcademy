using BpstEdu.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BpstEdu.Infrastructure.Persistence.Configurations;

internal sealed class AuditEntryConfiguration : IEntityTypeConfiguration<AuditEntry>
{
    public void Configure(EntityTypeBuilder<AuditEntry> b)
    {
        b.ToTable("audit_log");
        b.Property(e => e.Id).UseIdentityAlwaysColumn();
        b.Property(e => e.UserId).HasMaxLength(64);
        b.Property(e => e.EntityType).HasMaxLength(128);
        b.Property(e => e.EntityId).HasMaxLength(64);
        b.Property(e => e.Action).HasConversion<string>().HasMaxLength(16);
        b.Property(e => e.Changes).HasColumnType("jsonb");
        b.HasIndex(e => new { e.EntityType, e.EntityId });
        b.HasIndex(e => e.At);
    }
}
