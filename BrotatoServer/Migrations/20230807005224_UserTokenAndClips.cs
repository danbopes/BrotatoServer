using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrotatoServer.Migrations
{
    /// <inheritdoc />
    public partial class UserTokenAndClips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TwitchAccessToken",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwitchClip",
                table: "Run",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwitchAccessToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TwitchClip",
                table: "Run");
        }
    }
}
