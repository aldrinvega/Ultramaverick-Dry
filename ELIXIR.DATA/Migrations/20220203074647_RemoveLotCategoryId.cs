using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class RemoveLotCategoryId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseReceived_LotCategories_LotCategoryId",
                table: "WarehouseReceived");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseReceived_LotCategoryId",
                table: "WarehouseReceived");

            migrationBuilder.DropColumn(
                name: "LotCategoryId",
                table: "WarehouseReceived");

            migrationBuilder.AddColumn<string>(
                name: "LotCategory",
                table: "WarehouseReceived",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LotCategory",
                table: "WarehouseReceived");

            migrationBuilder.AddColumn<int>(
                name: "LotCategoryId",
                table: "WarehouseReceived",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseReceived_LotCategoryId",
                table: "WarehouseReceived",
                column: "LotCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseReceived_LotCategories_LotCategoryId",
                table: "WarehouseReceived",
                column: "LotCategoryId",
                principalTable: "LotCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
