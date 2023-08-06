using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrotatoServer.Migrations
{
    /// <inheritdoc />
    public partial class MultipleChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RunData",
                table: "Run",
                newName: "RunInformation");

            migrationBuilder.AddColumn<ulong>(
                name: "UserId",
                table: "Run",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateIndex(
                name: "IX_Run_UserId",
                table: "Run",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Run_Users_UserId",
                table: "Run",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "SteamId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Run_Users_UserId",
                table: "Run");

            migrationBuilder.DropIndex(
                name: "IX_Run_UserId",
                table: "Run");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Run");

            migrationBuilder.RenameColumn(
                name: "RunInformation",
                table: "Run",
                newName: "RunData");
        }
    }
}
