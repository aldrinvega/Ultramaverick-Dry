using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class ReAdjustAnyChecklistRelatedEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductTypeId",
                table: "ChecklistQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistQuestions_ProductTypeId",
                table: "ChecklistQuestions",
                column: "ProductTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistQuestions_ProductTypes_ProductTypeId",
                table: "ChecklistQuestions",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistQuestions_ProductTypes_ProductTypeId",
                table: "ChecklistQuestions");

            migrationBuilder.DropIndex(
                name: "IX_ChecklistQuestions_ProductTypeId",
                table: "ChecklistQuestions");

            migrationBuilder.DropColumn(
                name: "ProductTypeId",
                table: "ChecklistQuestions");
        }
    }
}
