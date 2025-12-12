using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeparateCurrencyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "property",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "MaxPrice",
                table: "property",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinPrice",
                table: "property",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "property");

            migrationBuilder.DropColumn(
                name: "MaxPrice",
                table: "property");

            migrationBuilder.DropColumn(
                name: "MinPrice",
                table: "property");
        }
    }
}
