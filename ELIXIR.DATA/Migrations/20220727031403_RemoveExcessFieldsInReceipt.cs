using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class RemoveExcessFieldsInReceipt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "ItemCode",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "ItemDescription",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "ReceiptPKey",
                table: "MiscellaneousReceipts");

            migrationBuilder.RenameColumn(
                name: "Uom",
                table: "MiscellaneousReceipts",
                newName: "SupplierCode");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "MiscellaneousReceipts",
                newName: "TotalQuantity");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalQuantity",
                table: "MiscellaneousReceipts",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "SupplierCode",
                table: "MiscellaneousReceipts",
                newName: "Uom");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "MiscellaneousReceipts",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ItemCode",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemDescription",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceiptPKey",
                table: "MiscellaneousReceipts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
