using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGame.Migrations
{
    /// <inheritdoc />
    public partial class AddPlatformToReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlatformId",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_PlatformId",
                table: "Reviews",
                column: "PlatformId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Platforms_PlatformId",
                table: "Reviews",
                column: "PlatformId",
                principalTable: "Platforms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Platforms_PlatformId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_PlatformId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "PlatformId",
                table: "Reviews");
        }
    }
}
