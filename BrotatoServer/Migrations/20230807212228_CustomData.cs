using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrotatoServer.Migrations
{
    /// <inheritdoc />
    public partial class CustomData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomData",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomData",
                table: "Run",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomData",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CustomData",
                table: "Run");
        }
    }
}
