using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddNewFieldForMasterList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupplierCode",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LotNameCode",
                table: "LotNames",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LotCategoryCode",
                table: "LotCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerCode",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupplierCode",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "LotNameCode",
                table: "LotNames");

            migrationBuilder.DropColumn(
                name: "LotCategoryCode",
                table: "LotCategories");

            migrationBuilder.DropColumn(
                name: "CustomerCode",
                table: "Customers");
        }
    }
}
