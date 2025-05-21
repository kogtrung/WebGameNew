using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGame.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForumReportRestrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForumReports_ForumComments_ForumCommentId",
                table: "ForumReports");

            migrationBuilder.DropForeignKey(
                name: "FK_ForumReports_ForumPosts_ForumPostId",
                table: "ForumReports");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumReports_ForumComments_ForumCommentId",
                table: "ForumReports",
                column: "ForumCommentId",
                principalTable: "ForumComments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ForumReports_ForumPosts_ForumPostId",
                table: "ForumReports",
                column: "ForumPostId",
                principalTable: "ForumPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForumReports_ForumComments_ForumCommentId",
                table: "ForumReports");

            migrationBuilder.DropForeignKey(
                name: "FK_ForumReports_ForumPosts_ForumPostId",
                table: "ForumReports");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumReports_ForumComments_ForumCommentId",
                table: "ForumReports",
                column: "ForumCommentId",
                principalTable: "ForumComments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumReports_ForumPosts_ForumPostId",
                table: "ForumReports",
                column: "ForumPostId",
                principalTable: "ForumPosts",
                principalColumn: "Id");
        }
    }
}
