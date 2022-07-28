using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddNewTableForIssueHeader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "IssuePKey",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "ItemCode",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "ItemDescription",
                table: "MiscellaneousIssues");

            migrationBuilder.RenameColumn(
                name: "Uom",
                table: "MiscellaneousIssues",
                newName: "CustomerCode");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "MiscellaneousIssues",
                newName: "TotalQuantity");

            migrationBuilder.CreateTable(
                name: "MiscellaneousIssueDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Customer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Uom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "Date", nullable: false),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PreparedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiscellaneousIssueDetails", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MiscellaneousIssueDetails");

            migrationBuilder.RenameColumn(
                name: "TotalQuantity",
                table: "MiscellaneousIssues",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "CustomerCode",
                table: "MiscellaneousIssues",
                newName: "Uom");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "MiscellaneousIssues",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IssuePKey",
                table: "MiscellaneousIssues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ItemCode",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemDescription",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
