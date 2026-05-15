using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UDBFRaceFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Races",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RaceNumber = table.Column<int>(type: "integer", nullable: false),
                    BoatSize = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    Distance = table.Column<int>(type: "integer", nullable: false),
                    GenderCategory = table.Column<int>(type: "integer", nullable: false),
                    RaceStatus = table.Column<int>(type: "integer", nullable: false),
                    RaceType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Races", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RaceEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartLane = table.Column<int>(type: "integer", nullable: false),
                    FinishPlace = table.Column<int>(type: "integer", nullable: false),
                    FinishTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    RaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RaceEntries_Races_RaceId",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RaceEntries_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RaceEntries_RaceId",
                table: "RaceEntries",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "IX_RaceEntries_TeamId",
                table: "RaceEntries",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Races_RaceNumber",
                table: "Races",
                column: "RaceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Name",
                table: "Teams",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RaceEntries");

            migrationBuilder.DropTable(
                name: "Races");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
