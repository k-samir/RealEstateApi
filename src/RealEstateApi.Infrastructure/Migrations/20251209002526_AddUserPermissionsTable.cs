using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPermissionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_permissions",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    agent_tier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    clients_permissions = table.Column<string>(type: "jsonb", nullable: false),
                    properties_permissions = table.Column<string>(type: "jsonb", nullable: false),
                    transactions_permissions = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_permissions", x => x.user_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_permissions");
        }
    }
}
