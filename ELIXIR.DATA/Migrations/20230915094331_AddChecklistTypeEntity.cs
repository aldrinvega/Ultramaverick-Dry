using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddChecklistTypeEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistQuestions_ProductTypes_ProductTypeId",
                table: "ChecklistQuestions");

            migrationBuilder.RenameColumn(
                name: "ProductTypeId",
                table: "ChecklistQuestions",
                newName: "ChecklistTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ChecklistQuestions_ProductTypeId",
                table: "ChecklistQuestions",
                newName: "IX_ChecklistQuestions_ChecklistTypeId");

            migrationBuilder.CreateTable(
                name: "ChecklistTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChecklistType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductTypeId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistTypes_ProductTypeId",
                table: "ChecklistTypes",
                column: "ProductTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistQuestions_ChecklistTypes_ChecklistTypeId",
                table: "ChecklistQuestions",
                column: "ChecklistTypeId",
                principalTable: "ChecklistTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistQuestions_ChecklistTypes_ChecklistTypeId",
                table: "ChecklistQuestions");

            migrationBuilder.DropTable(
                name: "ChecklistTypes");

            migrationBuilder.RenameColumn(
                name: "ChecklistTypeId",
                table: "ChecklistQuestions",
                newName: "ProductTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ChecklistQuestions_ChecklistTypeId",
                table: "ChecklistQuestions",
                newName: "IX_ChecklistQuestions_ProductTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistQuestions_ProductTypes_ProductTypeId",
                table: "ChecklistQuestions",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
