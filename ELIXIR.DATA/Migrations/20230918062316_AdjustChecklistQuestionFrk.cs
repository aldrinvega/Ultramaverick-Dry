using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AdjustChecklistQuestionFrk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistQuestions_ChecklistTypes_ChecklistTypeId",
                table: "ChecklistQuestions");

            migrationBuilder.AlterColumn<int>(
                name: "ChecklistTypeId",
                table: "ChecklistQuestions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistQuestions_ChecklistTypes_ChecklistTypeId",
                table: "ChecklistQuestions",
                column: "ChecklistTypeId",
                principalTable: "ChecklistTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistQuestions_ChecklistTypes_ChecklistTypeId",
                table: "ChecklistQuestions");

            migrationBuilder.AlterColumn<int>(
                name: "ChecklistTypeId",
                table: "ChecklistQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistQuestions_ChecklistTypes_ChecklistTypeId",
                table: "ChecklistQuestions",
                column: "ChecklistTypeId",
                principalTable: "ChecklistTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
