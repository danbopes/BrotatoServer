using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrotatoServer.Migrations
{
    /// <inheritdoc />
    public partial class Indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Run_UserId",
                table: "Run");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ApiKey",
                table: "Users",
                column: "ApiKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TwitchUsername",
                table: "Users",
                column: "TwitchUsername");

            migrationBuilder.CreateIndex(
                name: "IX_Run_UserId_Date",
                table: "Run",
                columns: new[] { "UserId", "Date" },
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_ApiKey",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_TwitchUsername",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Run_UserId_Date",
                table: "Run");

            migrationBuilder.CreateIndex(
                name: "IX_Run_UserId",
                table: "Run",
                column: "UserId");
        }
    }
}
