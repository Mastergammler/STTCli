using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stt.Prototype.Cli.Migrations
{
    /// <inheritdoc />
    public partial class ItemDeadline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Deadline",
                table: "Items",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deadline",
                table: "Items");
        }
    }
}
