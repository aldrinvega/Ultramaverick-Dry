using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddChecklistRelatedEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "ChecklistTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChecklistType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AddedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ProductTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistTypes_ProductTypes_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalTable: "ProductTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChecklistTypes_Users_AddedBy",
                        column: x => x.AddedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ChecklistTypes_Users_ModifiedBy",
                        column: x => x.ModifiedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QcChecklists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceivingId = table.Column<int>(type: "int", nullable: false),
                    ProductTypeId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QcChecklists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QcChecklists_ProductTypes_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalTable: "ProductTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QcChecklists_QC_Receiving_ReceivingId",
                        column: x => x.ReceivingId,
                        principalTable: "QC_Receiving",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChecklistQuestion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChecklistTypeId = table.Column<int>(type: "int", nullable: false),
                    ProductTypeId = table.Column<int>(type: "int", nullable: true),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AddedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AnswerType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistQuestions_ChecklistTypes_ChecklistTypeId",
                        column: x => x.ChecklistTypeId,
                        principalTable: "ChecklistTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChecklistQuestions_ProductTypes_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalTable: "ProductTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChecklistQuestions_Users_AddedBy",
                        column: x => x.AddedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ChecklistQuestions_Users_ModifiedBy",
                        column: x => x.ModifiedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistCompliances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QCChecklistId = table.Column<int>(type: "int", nullable: false),
                    Compliance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RootCause = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistCompliances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistCompliances_QcChecklists_QCChecklistId",
                        column: x => x.QCChecklistId,
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
                    QCChecklistId = table.Column<int>(type: "int", nullable: false),
                    Observation = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistOtherObservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistOtherObservations_QcChecklists_QCChecklistId",
                        column: x => x.QCChecklistId,
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
                    QCChecklistId = table.Column<int>(type: "int", nullable: false),
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
                        name: "FK_ChecklistReviewVerificationLogs_QcChecklists_QCChecklistId",
                        column: x => x.QCChecklistId,
                        principalTable: "QcChecklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "QChecklistOpenFieldAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QCChecklistId = table.Column<int>(type: "int", nullable: false),
                    ChecklistQuestionsId = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QChecklistOpenFieldAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QChecklistOpenFieldAnswers_ChecklistQuestions_ChecklistQuestionsId",
                        column: x => x.ChecklistQuestionsId,
                        principalTable: "ChecklistQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QChecklistOpenFieldAnswers_QcChecklists_QCChecklistId",
                        column: x => x.QCChecklistId,
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
                    QCChecklistId = table.Column<int>(type: "int", nullable: false),
                    ChecklistQuestionId = table.Column<int>(type: "int", nullable: false),
                    Standard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Actual = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChecklistQuestionsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QChecklistProductDimensions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QChecklistProductDimensions_ChecklistQuestions_ChecklistQuestionsId",
                        column: x => x.ChecklistQuestionsId,
                        principalTable: "ChecklistQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QChecklistProductDimensions_QcChecklists_QCChecklistId",
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

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistCompliances_QCChecklistId",
                table: "ChecklistCompliances",
                column: "QCChecklistId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistOtherObservations_QCChecklistId",
                table: "ChecklistOtherObservations",
                column: "QCChecklistId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistQuestions_AddedBy",
                table: "ChecklistQuestions",
                column: "AddedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistQuestions_ChecklistTypeId",
                table: "ChecklistQuestions",
                column: "ChecklistTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistQuestions_ModifiedBy",
                table: "ChecklistQuestions",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistQuestions_ProductTypeId",
                table: "ChecklistQuestions",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistReviewVerificationLogs_QCChecklistId",
                table: "ChecklistReviewVerificationLogs",
                column: "QCChecklistId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistTypes_AddedBy",
                table: "ChecklistTypes",
                column: "AddedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistTypes_ModifiedBy",
                table: "ChecklistTypes",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistTypes_ProductTypeId",
                table: "ChecklistTypes",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_QcChecklists_ProductTypeId",
                table: "QcChecklists",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_QcChecklists_ReceivingId",
                table: "QcChecklists",
                column: "ReceivingId");

            migrationBuilder.CreateIndex(
                name: "IX_QChecklistOpenFieldAnswers_ChecklistQuestionsId",
                table: "QChecklistOpenFieldAnswers",
                column: "ChecklistQuestionsId");

            migrationBuilder.CreateIndex(
                name: "IX_QChecklistOpenFieldAnswers_QCChecklistId",
                table: "QChecklistOpenFieldAnswers",
                column: "QCChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_QChecklistProductDimensions_ChecklistQuestionsId",
                table: "QChecklistProductDimensions",
                column: "ChecklistQuestionsId");

            migrationBuilder.CreateIndex(
                name: "IX_QChecklistProductDimensions_QCChecklistId",
                table: "QChecklistProductDimensions",
                column: "QCChecklistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "ChecklistAnswers");

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
                name: "ChecklistQuestions");

            migrationBuilder.DropTable(
                name: "QcChecklists");

            migrationBuilder.DropTable(
                name: "ChecklistTypes");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
