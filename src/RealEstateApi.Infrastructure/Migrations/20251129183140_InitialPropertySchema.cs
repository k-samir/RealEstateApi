using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RealEstateApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialPropertySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "property",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    agent_id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    location = table.Column<string>(type: "text", nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: true),
                    longitude = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: true),
                    type = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false, defaultValue: "draft"),
                    completion_date = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    long_description = table.Column<string>(type: "text", nullable: true),
                    features = table.Column<string>(type: "jsonb", nullable: false, defaultValue: "[]"),
                    amenities = table.Column<string>(type: "jsonb", nullable: false, defaultValue: "[]"),
                    specifications = table.Column<string>(type: "jsonb", nullable: false, defaultValue: "[]"),
                    main_image = table.Column<string>(type: "text", nullable: true),
                    images = table.Column<string>(type: "jsonb", nullable: false, defaultValue: "[]"),
                    bedrooms_range = table.Column<string>(type: "text", nullable: true),
                    bathrooms_range = table.Column<string>(type: "text", nullable: true),
                    area_range = table.Column<string>(type: "text", nullable: true),
                    price_range = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_property", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "unit",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    property_id = table.Column<int>(type: "integer", nullable: false),
                    unit_number = table.Column<string>(type: "text", nullable: false),
                    floor = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "text", nullable: false),
                    bedrooms = table.Column<int>(type: "integer", nullable: false),
                    bathrooms = table.Column<int>(type: "integer", nullable: false),
                    area = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    price = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    status = table.Column<string>(type: "text", nullable: false, defaultValue: "Available"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unit", x => x.id);
                    table.ForeignKey(
                        name: "FK_unit_property_property_id",
                        column: x => x.property_id,
                        principalTable: "property",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Add FK constraint to user table (managed by Drizzle in Next.js)
            migrationBuilder.Sql(@"
                ALTER TABLE property
                ADD CONSTRAINT fk_property_agent_id
                FOREIGN KEY (agent_id)
                REFERENCES ""user""(id)
                ON DELETE CASCADE;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_property_agent_id",
                table: "property",
                column: "agent_id");

            migrationBuilder.CreateIndex(
                name: "IX_property_location",
                table: "property",
                column: "location");

            migrationBuilder.CreateIndex(
                name: "IX_property_status",
                table: "property",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_unit_property_id",
                table: "unit",
                column: "property_id");

            migrationBuilder.CreateIndex(
                name: "IX_unit_status",
                table: "unit",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "unit");

            // Drop FK constraint before dropping property table
            migrationBuilder.Sql(@"
                ALTER TABLE property
                DROP CONSTRAINT IF EXISTS fk_property_agent_id;
            ");

            migrationBuilder.DropTable(
                name: "property");
        }
    }
}
