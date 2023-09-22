using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AdjustChecklistQuestionAnswerType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOpenField",
                table: "ChecklistQuestions");

            migrationBuilder.AddColumn<int>(
                name: "AnswerType",
                table: "ChecklistQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerType",
                table: "ChecklistQuestions");

            migrationBuilder.AddColumn<bool>(
                name: "IsOpenField",
                table: "ChecklistQuestions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
