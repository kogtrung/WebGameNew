using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGame.Migrations
{
    /// <inheritdoc />
    public partial class AddAchievementConditions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConditionType",
                table: "Achievements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ConditionValue",
                table: "Achievements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Achievements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsSecret",
                table: "Achievements",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConditionType",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "ConditionValue",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "IsSecret",
                table: "Achievements");
        }
    }
}
