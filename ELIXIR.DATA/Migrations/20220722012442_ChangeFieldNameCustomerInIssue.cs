using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class ChangeFieldNameCustomerInIssue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Supplier",
                table: "MiscellaneousIssues",
                newName: "Customer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Customer",
                table: "MiscellaneousIssues",
                newName: "Supplier");
        }
    }
}
