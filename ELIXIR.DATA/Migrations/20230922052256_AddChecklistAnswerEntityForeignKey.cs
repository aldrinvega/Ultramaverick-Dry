using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddChecklistAnswerEntityForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistCompliances_QcChecklists_QcChecklistId",
                table: "ChecklistCompliances");

            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistOtherObservations_QcChecklists_QcChecklistId",
                table: "ChecklistOtherObservations");

            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistReviewVerificationLogs_QcChecklists_QcChecklistId",
                table: "ChecklistReviewVerificationLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_QChecklistOpenFieldAnswers_QcChecklists_QcChecklistId",
                table: "QChecklistOpenFieldAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QChecklistProductDimensions_QcChecklists_QcChecklistId",
                table: "QChecklistProductDimensions");

            migrationBuilder.DropIndex(
                name: "IX_ChecklistReviewVerificationLogs_QcChecklistId",
                table: "ChecklistReviewVerificationLogs");

            migrationBuilder.DropIndex(
                name: "IX_ChecklistOtherObservations_QcChecklistId",
                table: "ChecklistOtherObservations");

            migrationBuilder.DropIndex(
                name: "IX_ChecklistCompliances_QcChecklistId",
                table: "ChecklistCompliances");

            migrationBuilder.RenameColumn(
                name: "QcChecklistId",
                table: "QChecklistProductDimensions",
                newName: "QCChecklistId");

            migrationBuilder.RenameIndex(
                name: "IX_QChecklistProductDimensions_QcChecklistId",
                table: "QChecklistProductDimensions",
                newName: "IX_QChecklistProductDimensions_QCChecklistId");

            migrationBuilder.RenameColumn(
                name: "QcChecklistId",
                table: "QChecklistOpenFieldAnswers",
                newName: "QCChecklistId");

            migrationBuilder.RenameIndex(
                name: "IX_QChecklistOpenFieldAnswers_QcChecklistId",
                table: "QChecklistOpenFieldAnswers",
                newName: "IX_QChecklistOpenFieldAnswers_QCChecklistId");

            migrationBuilder.RenameColumn(
                name: "QcChecklistId",
                table: "ChecklistReviewVerificationLogs",
                newName: "QCChecklistId");

            migrationBuilder.RenameColumn(
                name: "QcChecklistId",
                table: "ChecklistOtherObservations",
                newName: "QCChecklistId");

            migrationBuilder.RenameColumn(
                name: "QcChecklistId",
                table: "ChecklistCompliances",
                newName: "QCChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistReviewVerificationLogs_QCChecklistId",
                table: "ChecklistReviewVerificationLogs",
                column: "QCChecklistId",
                unique: false);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistOtherObservations_QCChecklistId",
                table: "ChecklistOtherObservations",
                column: "QCChecklistId",
                unique: false);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistCompliances_QCChecklistId",
                table: "ChecklistCompliances",
                column: "QCChecklistId",
                unique: false);

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistCompliances_QcChecklists_QCChecklistId",
                table: "ChecklistCompliances",
                column: "QCChecklistId",
                principalTable: "QcChecklists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistOtherObservations_QcChecklists_QCChecklistId",
                table: "ChecklistOtherObservations",
                column: "QCChecklistId",
                principalTable: "QcChecklists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistReviewVerificationLogs_QcChecklists_QCChecklistId",
                table: "ChecklistReviewVerificationLogs",
                column: "QCChecklistId",
                principalTable: "QcChecklists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QChecklistOpenFieldAnswers_QcChecklists_QCChecklistId",
                table: "QChecklistOpenFieldAnswers",
                column: "QCChecklistId",
                principalTable: "QcChecklists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QChecklistProductDimensions_QcChecklists_QCChecklistId",
                table: "QChecklistProductDimensions",
                column: "QCChecklistId",
                principalTable: "QcChecklists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistCompliances_QcChecklists_QCChecklistId",
                table: "ChecklistCompliances");

            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistOtherObservations_QcChecklists_QCChecklistId",
                table: "ChecklistOtherObservations");

            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistReviewVerificationLogs_QcChecklists_QCChecklistId",
                table: "ChecklistReviewVerificationLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_QChecklistOpenFieldAnswers_QcChecklists_QCChecklistId",
                table: "QChecklistOpenFieldAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QChecklistProductDimensions_QcChecklists_QCChecklistId",
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
                name: "QCChecklistId",
                table: "QChecklistProductDimensions",
                newName: "QcChecklistId");

            migrationBuilder.RenameIndex(
                name: "IX_QChecklistProductDimensions_QCChecklistId",
                table: "QChecklistProductDimensions",
                newName: "IX_QChecklistProductDimensions_QcChecklistId");

            migrationBuilder.RenameColumn(
                name: "QCChecklistId",
                table: "QChecklistOpenFieldAnswers",
                newName: "QcChecklistId");

            migrationBuilder.RenameIndex(
                name: "IX_QChecklistOpenFieldAnswers_QCChecklistId",
                table: "QChecklistOpenFieldAnswers",
                newName: "IX_QChecklistOpenFieldAnswers_QcChecklistId");

            migrationBuilder.RenameColumn(
                name: "QCChecklistId",
                table: "ChecklistReviewVerificationLogs",
                newName: "QcChecklistId");

            migrationBuilder.RenameColumn(
                name: "QCChecklistId",
                table: "ChecklistOtherObservations",
                newName: "QcChecklistId");

            migrationBuilder.RenameColumn(
                name: "QCChecklistId",
                table: "ChecklistCompliances",
                newName: "QcChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistReviewVerificationLogs_QcChecklistId",
                table: "ChecklistReviewVerificationLogs",
                column: "QcChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistOtherObservations_QcChecklistId",
                table: "ChecklistOtherObservations",
                column: "QcChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistCompliances_QcChecklistId",
                table: "ChecklistCompliances",
                column: "QcChecklistId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistCompliances_QcChecklists_QcChecklistId",
                table: "ChecklistCompliances",
                column: "QcChecklistId",
                principalTable: "QcChecklists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistOtherObservations_QcChecklists_QcChecklistId",
                table: "ChecklistOtherObservations",
                column: "QcChecklistId",
                principalTable: "QcChecklists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistReviewVerificationLogs_QcChecklists_QcChecklistId",
                table: "ChecklistReviewVerificationLogs",
                column: "QcChecklistId",
                principalTable: "QcChecklists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QChecklistOpenFieldAnswers_QcChecklists_QcChecklistId",
                table: "QChecklistOpenFieldAnswers",
                column: "QcChecklistId",
                principalTable: "QcChecklists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QChecklistProductDimensions_QcChecklists_QcChecklistId",
                table: "QChecklistProductDimensions",
                column: "QcChecklistId",
                principalTable: "QcChecklists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
