using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddNewChecklistAnswerEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChecklistDescriptions");

            migrationBuilder.CreateTable(
                name: "ChecklistQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChecklistQuestion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductTypeId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AddedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AnswerType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistQuestions_ProductTypes_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalTable: "ProductTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ChecklistQuestions_Users_AddedBy",
                        column: x => x.AddedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChecklistAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChecklistQuestionId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AddedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistAnswers_ChecklistQuestions_ChecklistQuestionId",
                        column: x => x.ChecklistQuestionId,
                        principalTable: "ChecklistQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ChecklistAnswers_Users_AddedBy",
                        column: x => x.AddedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistAnswers_AddedBy",
                table: "ChecklistAnswers",
                column: "AddedBy",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistAnswers_ChecklistQuestionId",
                table: "ChecklistAnswers",
                column: "ChecklistQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistQuestions_AddedBy",
                table: "ChecklistQuestions",
                column: "AddedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistQuestions_ProductTypeId",
                table: "ChecklistQuestions",
                column: "ProductTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChecklistAnswers");

            migrationBuilder.DropTable(
                name: "ChecklistQuestions");

            migrationBuilder.CreateTable(
                name: "ChecklistDescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddedBy = table.Column<int>(type: "int", nullable: false),
                    ChecklistDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ProductTypeId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistDescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistDescriptions_ProductTypes_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalTable: "ProductTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ChecklistDescriptions_Users_AddedBy",
                        column: x => x.AddedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistDescriptions_AddedBy",
                table: "ChecklistDescriptions",
                column: "AddedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistDescriptions_ProductTypeId",
                table: "ChecklistDescriptions",
                column: "ProductTypeId");
        }
    }
}
