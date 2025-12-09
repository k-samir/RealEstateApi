using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGranularPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgentId",
                table: "transactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "property",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "property",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgentId",
                table: "clients",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "property");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "property");

            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "clients");
        }
    }
}
