using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishQuizApp.Migrations
{
    /// <inheritdoc />
    public partial class AddContentHashToQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentHash",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentHash",
                table: "Questions");
        }
    }
}
