using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Cribbage.Api.Data;

#nullable disable
namespace Cribbage.Api.Migrations;

[DbContext(typeof(CribbageDbContext))]
[Migration("20260927000000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(name: "games", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            PlayerOne = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            PlayerTwo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            PlayerOneScore = table.Column<int>(type: "integer", nullable: false),
            PlayerTwoScore = table.Column<int>(type: "integer", nullable: false),
            PlayedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
            Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
        }, constraints: table => table.PrimaryKey("PK_games", x => x.Id));
        migrationBuilder.CreateIndex(name: "IX_games_PlayedAt", table: "games", column: "PlayedAt");
    }
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "games");
}
