using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAppUserToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                schema: "sandbox",
                table: "humantasks",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_humantasks_AppUserId",
                schema: "sandbox",
                table: "humantasks",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_humantasks_AspNetUsers_AppUserId",
                schema: "sandbox",
                table: "humantasks",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_humantasks_AspNetUsers_AppUserId",
                schema: "sandbox",
                table: "humantasks");

            migrationBuilder.DropIndex(
                name: "IX_humantasks_AppUserId",
                schema: "sandbox",
                table: "humantasks");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                schema: "sandbox",
                table: "humantasks");
        }
    }
}
