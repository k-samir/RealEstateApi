using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStandaloneUnitFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "area_unit",
                table: "property");

            migrationBuilder.DropColumn(
                name: "bathrooms",
                table: "property");

            migrationBuilder.DropColumn(
                name: "bedrooms",
                table: "property");

            migrationBuilder.DropColumn(
                name: "unit_area",
                table: "property");

            migrationBuilder.DropColumn(
                name: "unit_price",
                table: "property");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "area_unit",
                table: "property",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "bathrooms",
                table: "property",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "bedrooms",
                table: "property",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "unit_area",
                table: "property",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "unit_price",
                table: "property",
                type: "numeric(15,2)",
                precision: 15,
                scale: 2,
                nullable: true);
        }
    }
}
