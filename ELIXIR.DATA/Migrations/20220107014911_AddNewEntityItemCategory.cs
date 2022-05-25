using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddNewEntityItemCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RawMaterials_ItemCategory_ItemCategoryId",
                table: "RawMaterials");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemCategory",
                table: "ItemCategory");

            migrationBuilder.RenameTable(
                name: "ItemCategory",
                newName: "ItemCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemCategories",
                table: "ItemCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RawMaterials_ItemCategories_ItemCategoryId",
                table: "RawMaterials",
                column: "ItemCategoryId",
                principalTable: "ItemCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RawMaterials_ItemCategories_ItemCategoryId",
                table: "RawMaterials");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemCategories",
                table: "ItemCategories");

            migrationBuilder.RenameTable(
                name: "ItemCategories",
                newName: "ItemCategory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemCategory",
                table: "ItemCategory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RawMaterials_ItemCategory_ItemCategoryId",
                table: "RawMaterials",
                column: "ItemCategoryId",
                principalTable: "ItemCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
