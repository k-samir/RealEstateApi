using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CleanupOldStatusValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure ALL old status values are converted to new ones
            // This is a safety net in case the previous migration missed any records

            migrationBuilder.Sql("UPDATE property SET status = 'Active' WHERE status = 'Published'");
            migrationBuilder.Sql("UPDATE property SET status = 'Active' WHERE status = 'Reserved'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No need to revert - this is a data cleanup migration
        }
    }
}
