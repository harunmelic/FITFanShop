using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitFanShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSecurityQuestionToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecurityAnswerHash",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecurityQuestion",
                table: "Users",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecurityAnswerHash",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SecurityQuestion",
                table: "Users");
        }
    }
}
