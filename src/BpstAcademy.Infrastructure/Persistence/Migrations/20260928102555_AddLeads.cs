using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BpstAcademy.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLeads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "leads",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    booking_ref = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    mobile = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    course_slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    course_title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    duration = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    mode = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    qualification = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    consent = table.Column<bool>(type: "boolean", nullable: false),
                    source_page = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    utm = table.Column<string>(type: "jsonb", nullable: true),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    payment_method = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    payment_reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    payment_reported_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_leads", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lead_submissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    lead_id = table.Column<Guid>(type: "uuid", nullable: false),
                    form_type = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    booking_ref = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    received_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    user_agent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lead_submissions", x => x.id);
                    table.ForeignKey(
                        name: "fk_lead_submissions_leads_lead_id",
                        column: x => x.lead_id,
                        principalTable: "leads",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_lead_submissions_lead_id",
                table: "lead_submissions",
                column: "lead_id");

            migrationBuilder.CreateIndex(
                name: "ix_lead_submissions_received_at",
                table: "lead_submissions",
                column: "received_at");

            migrationBuilder.CreateIndex(
                name: "ix_leads_booking_ref",
                table: "leads",
                column: "booking_ref",
                unique: true,
                filter: "booking_ref IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_leads_created_at",
                table: "leads",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_leads_mobile",
                table: "leads",
                column: "mobile");

            // Existing Admin roles get the new permission (the seed adds it to new databases).
            migrationBuilder.Sql("""
                INSERT INTO role_claims (role_id, claim_type, claim_value)
                SELECT r.id, 'permission', 'leads.view' FROM roles r
                WHERE r.normalized_name = 'ADMIN'
                  AND NOT EXISTS (SELECT 1 FROM role_claims c
                                  WHERE c.role_id = r.id AND c.claim_type = 'permission' AND c.claim_value = 'leads.view');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM role_claims WHERE claim_type = 'permission' AND claim_value = 'leads.view';");
            migrationBuilder.DropTable(
                name: "lead_submissions");

            migrationBuilder.DropTable(
                name: "leads");
        }
    }
}
