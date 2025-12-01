using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPublishedToProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "property",
                type: "text",
                nullable: false,
                defaultValue: "Draft",
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "draft");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "property",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "property");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "property",
                type: "text",
                nullable: false,
                defaultValue: "draft",
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Draft");
        }
    }
}
