using BpstAcademy.Domain.Crm;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BpstAcademy.Infrastructure.Persistence.Configurations;

internal sealed class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> b)
    {
        b.ToTable("leads");
        b.Property(e => e.Kind).HasConversion<string>().HasMaxLength(16);
        b.Property(e => e.Status).HasConversion<string>().HasMaxLength(24);
        b.Property(e => e.BookingRef).HasMaxLength(32);
        b.Property(e => e.Name).HasMaxLength(100);
        b.Property(e => e.Mobile).HasMaxLength(16);
        b.Property(e => e.Email).HasMaxLength(254);
        b.Property(e => e.City).HasMaxLength(100);
        b.Property(e => e.CourseSlug).HasMaxLength(100);
        b.Property(e => e.CourseTitle).HasMaxLength(200);
        b.Property(e => e.Duration).HasMaxLength(60);
        b.Property(e => e.Mode).HasMaxLength(60);
        b.Property(e => e.Qualification).HasMaxLength(100);
        b.Property(e => e.Message).HasMaxLength(2000);
        b.Property(e => e.SourcePage).HasMaxLength(300);
        b.Property(e => e.Utm).HasColumnType("jsonb");
        b.Property(e => e.PaymentMethod).HasMaxLength(20);
        b.Property(e => e.PaymentReference).HasMaxLength(100);
        b.HasIndex(e => e.BookingRef).IsUnique().HasFilter("booking_ref IS NOT NULL");
        b.HasIndex(e => e.CreatedAt);
        b.HasIndex(e => e.Mobile);
    }
}

internal sealed class LeadSubmissionConfiguration : IEntityTypeConfiguration<LeadSubmission>
{
    public void Configure(EntityTypeBuilder<LeadSubmission> b)
    {
        b.ToTable("lead_submissions");
        b.Property(e => e.FormType).HasMaxLength(24);
        b.Property(e => e.BookingRef).HasMaxLength(32);
        b.Property(e => e.Payload).HasColumnType("jsonb");
        b.Property(e => e.UserAgent).HasMaxLength(512);
        b.HasOne<Lead>().WithMany().HasForeignKey(e => e.LeadId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(e => e.ReceivedAt);
    }
}
