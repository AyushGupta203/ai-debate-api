using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIDebate.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSummaryToTurn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Turns",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Turns");
        }
    }
}
