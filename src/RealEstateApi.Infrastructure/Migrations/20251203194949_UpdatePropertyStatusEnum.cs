using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePropertyStatusEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Migrate existing status values to new enum
            // Old enum: Draft, Published, Reserved, Sold
            // New enum: Draft, Active, Sold, Rented, UnderConstruction, Completed, Archived

            // Update Published -> Active
            migrationBuilder.Sql("UPDATE property SET status = 'Active' WHERE status = 'Published'");

            // Update Reserved -> Active (treating reserved as active properties)
            migrationBuilder.Sql("UPDATE property SET status = 'Active' WHERE status = 'Reserved'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert to old enum values
            // New enum: Draft, Active, Sold, Rented, UnderConstruction, Completed, Archived
            // Old enum: Draft, Published, Reserved, Sold

            // Revert Active -> Published (best effort)
            migrationBuilder.Sql("UPDATE property SET status = 'Published' WHERE status = 'Active'");

            // Revert Rented -> Published (best effort)
            migrationBuilder.Sql("UPDATE property SET status = 'Published' WHERE status = 'Rented'");

            // Revert UnderConstruction -> Draft (best effort)
            migrationBuilder.Sql("UPDATE property SET status = 'Draft' WHERE status = 'UnderConstruction'");

            // Revert Completed -> Published (best effort)
            migrationBuilder.Sql("UPDATE property SET status = 'Published' WHERE status = 'Completed'");

            // Revert Archived -> Draft (best effort)
            migrationBuilder.Sql("UPDATE property SET status = 'Draft' WHERE status = 'Archived'");
        }
    }
}
