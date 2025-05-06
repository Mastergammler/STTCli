using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stt.Prototype.Cli.Migrations
{
    /// <inheritdoc />
    public partial class FixName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Fiters",
                table: "Fiters");

            migrationBuilder.RenameTable(
                name: "Fiters",
                newName: "Filters");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Filters",
                table: "Filters",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Filters",
                table: "Filters");

            migrationBuilder.RenameTable(
                name: "Filters",
                newName: "Fiters");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fiters",
                table: "Fiters",
                column: "Id");
        }
    }
}
