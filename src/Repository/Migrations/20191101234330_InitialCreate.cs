using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Repository.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Search",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Url = table.Column<string>(nullable: true),
                    Keywords = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false),
                    SearchEngineId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Search", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchResult",
                columns: table => new
                {
                    SearchResultId = table.Column<Guid>(nullable: false),
                    SearchId = table.Column<Guid>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Result = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchResult", x => x.SearchResultId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Search");

            migrationBuilder.DropTable(
                name: "SearchResult");
        }
    }
}
