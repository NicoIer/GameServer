using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FishGame.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "configs",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    value = table.Column<uint>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nickname = table.Column<string>(type: "TEXT", nullable: true),
                    globalState = table.Column<byte>(type: "INTEGER", nullable: false),
                    gameState = table.Column<byte>(type: "INTEGER", nullable: false),
                    macToken = table.Column<string>(type: "TEXT", nullable: true),
                    lastActionTimeSeconds = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_macToken",
                table: "users",
                column: "macToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_nickname",
                table: "users",
                column: "nickname",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "configs");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
