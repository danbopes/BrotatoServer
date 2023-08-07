using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrotatoServer.Migrations
{
    /// <inheritdoc />
    public partial class UserSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    OnRunStartedMessage = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    OnRunWonMessage = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    OnRunLostMessage = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    ClipOnRunWon = table.Column<bool>(type: "INTEGER", nullable: false),
                    ClipOnRunLost = table.Column<bool>(type: "INTEGER", nullable: false),
                    ClipDelaySeconds = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "SteamId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSettings");
        }
    }
}
