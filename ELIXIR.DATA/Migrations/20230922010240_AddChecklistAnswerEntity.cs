using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddChecklistAnswerEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChecklistAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QCChecklistId = table.Column<int>(type: "int", nullable: false),
                    ChecklistQuestionsId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistAnswers_ChecklistQuestions_ChecklistQuestionsId",
                        column: x => x.ChecklistQuestionsId,
                        principalTable: "ChecklistQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChecklistAnswers_QcChecklists_QCChecklistId",
                        column: x => x.QCChecklistId,
                        principalTable: "QcChecklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistAnswers_ChecklistQuestionsId",
                table: "ChecklistAnswers",
                column: "ChecklistQuestionsId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistAnswers_QCChecklistId",
                table: "ChecklistAnswers",
                column: "QCChecklistId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChecklistAnswers");
        }
    }
}
