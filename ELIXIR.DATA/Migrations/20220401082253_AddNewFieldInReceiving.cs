using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddNewFieldInReceiving : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "POSummary");

            migrationBuilder.AddColumn<string>(
                name: "CancelRemarks",
                table: "QC_Receiving",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelRemarks",
                table: "QC_Receiving");

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "POSummary",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
