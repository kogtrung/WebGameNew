using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGame.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGameComparisonFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameComparisonItems");

            migrationBuilder.DropTable(
                name: "GameFeatures");

            migrationBuilder.DropTable(
                name: "GameComparisons");

            migrationBuilder.DropTable(
                name: "ComparisonFeatures");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComparisonFeatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComparisonFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameComparisons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameComparisons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameComparisons_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameFeatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeatureId = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameFeatures_ComparisonFeatures_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "ComparisonFeatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameFeatures_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameComparisonItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComparisonId = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameComparisonItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameComparisonItems_GameComparisons_ComparisonId",
                        column: x => x.ComparisonId,
                        principalTable: "GameComparisons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameComparisonItems_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameComparisonItems_ComparisonId",
                table: "GameComparisonItems",
                column: "ComparisonId");

            migrationBuilder.CreateIndex(
                name: "IX_GameComparisonItems_GameId",
                table: "GameComparisonItems",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameComparisons_UserId",
                table: "GameComparisons",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameFeatures_FeatureId",
                table: "GameFeatures",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_GameFeatures_GameId",
                table: "GameFeatures",
                column: "GameId");
        }
    }
}
