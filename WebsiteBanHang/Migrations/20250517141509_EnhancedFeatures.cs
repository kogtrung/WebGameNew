using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGame.Migrations
{
    /// <inheritdoc />
    public partial class EnhancedFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Score",
                table: "Reviews",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "CriticName",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CriticWebsite",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Downvotes",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsHelpful",
                table: "Reviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerifiedPurchase",
                table: "Reviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PlayTimeHours",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewType",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Upvotes",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiscountPercentage",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FollowersCount",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnSale",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdateDate",
                table: "Games",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PreviousPrice",
                table: "Games",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PriceChangeDate",
                table: "Games",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SaleEndDate",
                table: "Games",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdateNotes",
                table: "Games",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ComparisonFeatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false)
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
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false)
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
                name: "GameFollows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    FollowDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NotifyOnUpdates = table.Column<bool>(type: "bit", nullable: false),
                    NotifyOnPriceChanges = table.Column<bool>(type: "bit", nullable: false),
                    NotifyOnNewReviews = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameFollows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameFollows_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameFollows_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameNotifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameNotifications_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameFeatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    FeatureId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
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
                    Order = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
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

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "DiscountPercentage", "FollowersCount", "IsOnSale", "LastUpdateDate", "PreviousPrice", "PriceChangeDate", "SaleEndDate", "UpdateNotes" },
                values: new object[] { 0, 0, false, null, null, null, null, null });

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

            migrationBuilder.CreateIndex(
                name: "IX_GameFollows_GameId",
                table: "GameFollows",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameFollows_UserId",
                table: "GameFollows",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameNotifications_GameId",
                table: "GameNotifications",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameNotifications_UserId",
                table: "GameNotifications",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameComparisonItems");

            migrationBuilder.DropTable(
                name: "GameFeatures");

            migrationBuilder.DropTable(
                name: "GameFollows");

            migrationBuilder.DropTable(
                name: "GameNotifications");

            migrationBuilder.DropTable(
                name: "GameComparisons");

            migrationBuilder.DropTable(
                name: "ComparisonFeatures");

            migrationBuilder.DropColumn(
                name: "CriticName",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "CriticWebsite",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Downvotes",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "IsHelpful",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "IsVerifiedPurchase",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "PlayTimeHours",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ReviewType",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Upvotes",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "FollowersCount",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "IsOnSale",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "LastUpdateDate",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "PreviousPrice",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "PriceChangeDate",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "SaleEndDate",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "UpdateNotes",
                table: "Games");

            migrationBuilder.AlterColumn<int>(
                name: "Score",
                table: "Reviews",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
