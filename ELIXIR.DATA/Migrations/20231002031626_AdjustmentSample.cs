using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AdjustmentSample : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistAnswers_ChecklistQuestions_ChecklistQuestionsId",
                table: "ChecklistAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QChecklistOpenFieldAnswers_ChecklistQuestions_ChecklistQuestionsId",
                table: "QChecklistOpenFieldAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QChecklistProductDimensions_ChecklistQuestions_ChecklistQuestionsId",
                table: "QChecklistProductDimensions");

            migrationBuilder.DropIndex(
                name: "IX_QChecklistProductDimensions_ChecklistQuestionsId",
                table: "QChecklistProductDimensions");

            migrationBuilder.DropIndex(
                name: "IX_ChecklistReviewVerificationLogs_QCChecklistId",
                table: "ChecklistReviewVerificationLogs");

            migrationBuilder.DropIndex(
                name: "IX_ChecklistOtherObservations_QCChecklistId",
                table: "ChecklistOtherObservations");

            migrationBuilder.DropIndex(
                name: "IX_ChecklistCompliances_QCChecklistId",
                table: "ChecklistCompliances");

            migrationBuilder.DropColumn(
                name: "ChecklistQuestionsId",
                table: "QChecklistProductDimensions");

            migrationBuilder.RenameColumn(
                name: "ChecklistQuestionsId",
                table: "QChecklistOpenFieldAnswers",
                newName: "ChecklistQuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_QChecklistOpenFieldAnswers_ChecklistQuestionsId",
                table: "QChecklistOpenFieldAnswers",
                newName: "IX_QChecklistOpenFieldAnswers_ChecklistQuestionId");

            migrationBuilder.RenameColumn(
                name: "ChecklistQuestionsId",
                table: "ChecklistAnswers",
                newName: "ChecklistQuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_ChecklistAnswers_ChecklistQuestionsId",
                table: "ChecklistAnswers",
                newName: "IX_ChecklistAnswers_ChecklistQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QChecklistProductDimensions_ChecklistQuestionId",
                table: "QChecklistProductDimensions",
                column: "ChecklistQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistReviewVerificationLogs_QCChecklistId",
                table: "ChecklistReviewVerificationLogs",
                column: "QCChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistOtherObservations_QCChecklistId",
                table: "ChecklistOtherObservations",
                column: "QCChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistCompliances_QCChecklistId",
                table: "ChecklistCompliances",
                column: "QCChecklistId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistAnswers_ChecklistQuestions_ChecklistQuestionId",
                table: "ChecklistAnswers",
                column: "ChecklistQuestionId",
                principalTable: "ChecklistQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QChecklistOpenFieldAnswers_ChecklistQuestions_ChecklistQuestionId",
                table: "QChecklistOpenFieldAnswers",
                column: "ChecklistQuestionId",
                principalTable: "ChecklistQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QChecklistProductDimensions_ChecklistQuestions_ChecklistQuestionId",
                table: "QChecklistProductDimensions",
                column: "ChecklistQuestionId",
                principalTable: "ChecklistQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistAnswers_ChecklistQuestions_ChecklistQuestionId",
                table: "ChecklistAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QChecklistOpenFieldAnswers_ChecklistQuestions_ChecklistQuestionId",
                table: "QChecklistOpenFieldAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QChecklistProductDimensions_ChecklistQuestions_ChecklistQuestionId",
                table: "QChecklistProductDimensions");

            migrationBuilder.DropIndex(
                name: "IX_QChecklistProductDimensions_ChecklistQuestionId",
                table: "QChecklistProductDimensions");

            migrationBuilder.DropIndex(
                name: "IX_ChecklistReviewVerificationLogs_QCChecklistId",
                table: "ChecklistReviewVerificationLogs");

            migrationBuilder.DropIndex(
                name: "IX_ChecklistOtherObservations_QCChecklistId",
                table: "ChecklistOtherObservations");

            migrationBuilder.DropIndex(
                name: "IX_ChecklistCompliances_QCChecklistId",
                table: "ChecklistCompliances");

            migrationBuilder.RenameColumn(
                name: "ChecklistQuestionId",
                table: "QChecklistOpenFieldAnswers",
                newName: "ChecklistQuestionsId");

            migrationBuilder.RenameIndex(
                name: "IX_QChecklistOpenFieldAnswers_ChecklistQuestionId",
                table: "QChecklistOpenFieldAnswers",
                newName: "IX_QChecklistOpenFieldAnswers_ChecklistQuestionsId");

            migrationBuilder.RenameColumn(
                name: "ChecklistQuestionId",
                table: "ChecklistAnswers",
                newName: "ChecklistQuestionsId");

            migrationBuilder.RenameIndex(
                name: "IX_ChecklistAnswers_ChecklistQuestionId",
                table: "ChecklistAnswers",
                newName: "IX_ChecklistAnswers_ChecklistQuestionsId");

            migrationBuilder.AddColumn<int>(
                name: "ChecklistQuestionsId",
                table: "QChecklistProductDimensions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QChecklistProductDimensions_ChecklistQuestionsId",
                table: "QChecklistProductDimensions",
                column: "ChecklistQuestionsId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistReviewVerificationLogs_QCChecklistId",
                table: "ChecklistReviewVerificationLogs",
                column: "QCChecklistId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistOtherObservations_QCChecklistId",
                table: "ChecklistOtherObservations",
                column: "QCChecklistId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistCompliances_QCChecklistId",
                table: "ChecklistCompliances",
                column: "QCChecklistId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistAnswers_ChecklistQuestions_ChecklistQuestionsId",
                table: "ChecklistAnswers",
                column: "ChecklistQuestionsId",
                principalTable: "ChecklistQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QChecklistOpenFieldAnswers_ChecklistQuestions_ChecklistQuestionsId",
                table: "QChecklistOpenFieldAnswers",
                column: "ChecklistQuestionsId",
                principalTable: "ChecklistQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QChecklistProductDimensions_ChecklistQuestions_ChecklistQuestionsId",
                table: "QChecklistProductDimensions",
                column: "ChecklistQuestionsId",
                principalTable: "ChecklistQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
