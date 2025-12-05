using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SplitTransactionAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "transactions",
                newName: "UndeclaredAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "DeclaredAmount",
                table: "transactions",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeclaredAmount",
                table: "transactions");

            migrationBuilder.RenameColumn(
                name: "UndeclaredAmount",
                table: "transactions",
                newName: "Amount");
        }
    }
}
