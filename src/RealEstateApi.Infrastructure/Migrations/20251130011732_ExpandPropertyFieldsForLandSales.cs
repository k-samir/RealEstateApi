using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExpandPropertyFieldsForLandSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsPublished",
                table: "property",
                newName: "is_published");

            migrationBuilder.AlterColumn<bool>(
                name: "is_published",
                table: "property",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<string>(
                name: "area",
                table: "property",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "property",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "property",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "country",
                table: "property",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "developer",
                table: "property",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "floor_plans",
                table: "property",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "furnishing_status",
                table: "property",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_featured",
                table: "property",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "key_highlights",
                table: "property",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "land_area_unit",
                table: "property",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nearby_places",
                table: "property",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "postal_code",
                table: "property",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "state",
                table: "property",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "street_address",
                table: "property",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "total_floors",
                table: "property",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "total_land_area",
                table: "property",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "total_units",
                table: "property",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "unit_types_available",
                table: "property",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "video_tour_url",
                table: "property",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "area",
                table: "property");

            migrationBuilder.DropColumn(
                name: "category",
                table: "property");

            migrationBuilder.DropColumn(
                name: "city",
                table: "property");

            migrationBuilder.DropColumn(
                name: "country",
                table: "property");

            migrationBuilder.DropColumn(
                name: "developer",
                table: "property");

            migrationBuilder.DropColumn(
                name: "floor_plans",
                table: "property");

            migrationBuilder.DropColumn(
                name: "furnishing_status",
                table: "property");

            migrationBuilder.DropColumn(
                name: "is_featured",
                table: "property");

            migrationBuilder.DropColumn(
                name: "key_highlights",
                table: "property");

            migrationBuilder.DropColumn(
                name: "land_area_unit",
                table: "property");

            migrationBuilder.DropColumn(
                name: "nearby_places",
                table: "property");

            migrationBuilder.DropColumn(
                name: "postal_code",
                table: "property");

            migrationBuilder.DropColumn(
                name: "state",
                table: "property");

            migrationBuilder.DropColumn(
                name: "street_address",
                table: "property");

            migrationBuilder.DropColumn(
                name: "total_floors",
                table: "property");

            migrationBuilder.DropColumn(
                name: "total_land_area",
                table: "property");

            migrationBuilder.DropColumn(
                name: "total_units",
                table: "property");

            migrationBuilder.DropColumn(
                name: "unit_types_available",
                table: "property");

            migrationBuilder.DropColumn(
                name: "video_tour_url",
                table: "property");

            migrationBuilder.RenameColumn(
                name: "is_published",
                table: "property",
                newName: "IsPublished");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPublished",
                table: "property",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);
        }
    }
}
