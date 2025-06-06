using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stt.Prototype.Cli.Migrations
{
    /// <inheritdoc />
    public partial class ListItemCompositeStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "ParentId",
                table: "Items",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_ParentId",
                table: "Items",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Items_ParentId",
                table: "Items",
                column: "ParentId",
                principalTable: "Items",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Items_ParentId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_ParentId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Items");
        }
    }
}
