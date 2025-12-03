using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyModeStandaloneFieldsAndUnitMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "amenities",
                table: "unit",
                type: "jsonb",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "unit",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "floor_plans",
                table: "unit",
                type: "jsonb",
                nullable: false);

            migrationBuilder.AddColumn<List<string>>(
                name: "images",
                table: "unit",
                type: "jsonb",
                nullable: false);

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

            migrationBuilder.AddColumn<string>(
                name: "property_mode",
                table: "property",
                type: "text",
                nullable: false,
                defaultValue: "Standalone");

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

            migrationBuilder.CreateIndex(
                name: "IX_property_property_mode",
                table: "property",
                column: "property_mode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_property_property_mode",
                table: "property");

            migrationBuilder.DropColumn(
                name: "amenities",
                table: "unit");

            migrationBuilder.DropColumn(
                name: "description",
                table: "unit");

            migrationBuilder.DropColumn(
                name: "floor_plans",
                table: "unit");

            migrationBuilder.DropColumn(
                name: "images",
                table: "unit");

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
                name: "property_mode",
                table: "property");

            migrationBuilder.DropColumn(
                name: "unit_area",
                table: "property");

            migrationBuilder.DropColumn(
                name: "unit_price",
                table: "property");
        }
    }
}
