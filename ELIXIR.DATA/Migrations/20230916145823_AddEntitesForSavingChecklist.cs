using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddEntitesForSavingChecklist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QcChecklists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceivingId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QcChecklists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QcChecklists_QC_Receiving_ReceivingId",
                        column: x => x.ReceivingId,
                        principalTable: "QC_Receiving",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistCompliances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QcChecklistId = table.Column<int>(type: "int", nullable: false),
                    Compliance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RootCause = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistCompliances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistCompliances_QcChecklists_QcChecklistId",
                        column: x => x.QcChecklistId,
                        principalTable: "QcChecklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistOtherObservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QcChecklistId = table.Column<int>(type: "int", nullable: false),
                    Observation = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistOtherObservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistOtherObservations_QcChecklists_QcChecklistId",
                        column: x => x.QcChecklistId,
                        principalTable: "QcChecklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistReviewVerificationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QcChecklistId = table.Column<int>(type: "int", nullable: false),
                    DispositionId = table.Column<int>(type: "int", nullable: false),
                    QtyAccepted = table.Column<int>(type: "int", nullable: false),
                    QtyRejected = table.Column<int>(type: "int", nullable: false),
                    MonitoredBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistReviewVerificationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistReviewVerificationLogs_QcChecklists_QcChecklistId",
                        column: x => x.QcChecklistId,
                        principalTable: "QcChecklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QChecklistOpenFieldAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QcChecklistId = table.Column<int>(type: "int", nullable: false),
                    ChecklistQuestionId = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QChecklistOpenFieldAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QChecklistOpenFieldAnswers_ChecklistQuestions_ChecklistQuestionId",
                        column: x => x.ChecklistQuestionId,
                        principalTable: "ChecklistQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QChecklistOpenFieldAnswers_QcChecklists_QcChecklistId",
                        column: x => x.QcChecklistId,
                        principalTable: "QcChecklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QChecklistProductDimensions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QcChecklistId = table.Column<int>(type: "int", nullable: false),
                    ChecklistQuestionId = table.Column<int>(type: "int", nullable: false),
                    Standard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Actual = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QChecklistProductDimensions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QChecklistProductDimensions_ChecklistQuestions_ChecklistQuestionId",
                        column: x => x.ChecklistQuestionId,
                        principalTable: "ChecklistQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QChecklistProductDimensions_QcChecklists_QcChecklistId",
                        column: x => x.QcChecklistId,
                        principalTable: "QcChecklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistCompliances_QcChecklistId",
                table: "ChecklistCompliances",
                column: "QcChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistOtherObservations_QcChecklistId",
                table: "ChecklistOtherObservations",
                column: "QcChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistReviewVerificationLogs_QcChecklistId",
                table: "ChecklistReviewVerificationLogs",
                column: "QcChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_QcChecklists_ReceivingId",
                table: "QcChecklists",
                column: "ReceivingId");

            migrationBuilder.CreateIndex(
                name: "IX_QChecklistOpenFieldAnswers_ChecklistQuestionId",
                table: "QChecklistOpenFieldAnswers",
                column: "ChecklistQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QChecklistOpenFieldAnswers_QcChecklistId",
                table: "QChecklistOpenFieldAnswers",
                column: "QcChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_QChecklistProductDimensions_ChecklistQuestionId",
                table: "QChecklistProductDimensions",
                column: "ChecklistQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QChecklistProductDimensions_QcChecklistId",
                table: "QChecklistProductDimensions",
                column: "QcChecklistId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChecklistCompliances");

            migrationBuilder.DropTable(
                name: "ChecklistOtherObservations");

            migrationBuilder.DropTable(
                name: "ChecklistReviewVerificationLogs");

            migrationBuilder.DropTable(
                name: "QChecklistOpenFieldAnswers");

            migrationBuilder.DropTable(
                name: "QChecklistProductDimensions");

            migrationBuilder.DropTable(
                name: "QcChecklists");
        }
    }
}
