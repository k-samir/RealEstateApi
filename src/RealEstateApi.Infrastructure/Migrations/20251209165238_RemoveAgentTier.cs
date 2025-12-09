using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAgentTier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "agent_tier",
                table: "user_permissions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "agent_tier",
                table: "user_permissions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
