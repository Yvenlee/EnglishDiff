using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishQuizApp.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToQuizSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "QuizSessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "QuizSessions");
        }
    }
}
