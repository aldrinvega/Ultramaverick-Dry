using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddNewProductTypeIdOnQCChecklistEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductTypeId",
                table: "QcChecklists",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_QcChecklists_ProductTypeId",
                table: "QcChecklists",
                column: "ProductTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_QcChecklists_ProductTypes_ProductTypeId",
                table: "QcChecklists",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QcChecklists_ProductTypes_ProductTypeId",
                table: "QcChecklists");

            migrationBuilder.DropIndex(
                name: "IX_QcChecklists_ProductTypeId",
                table: "QcChecklists");

            migrationBuilder.DropColumn(
                name: "ProductTypeId",
                table: "QcChecklists");
        }
    }
}
