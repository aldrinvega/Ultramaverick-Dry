using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AdjustAccountTitles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CancelledOrders_Customers_CustomersId",
                table: "CancelledOrders");

            migrationBuilder.DropIndex(
                name: "IX_CancelledOrders_CustomersId",
                table: "CancelledOrders");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "CancelledOrders");

            migrationBuilder.DropColumn(
                name: "CustomersId",
                table: "CancelledOrders");

            migrationBuilder.DropColumn(
                name: "DateNeeded",
                table: "CancelledOrders");

            migrationBuilder.DropColumn(
                name: "ItemCode",
                table: "CancelledOrders");

            migrationBuilder.DropColumn(
                name: "ItemDescription",
                table: "CancelledOrders");

            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "CancelledOrders");

            migrationBuilder.DropColumn(
                name: "Uom",
                table: "CancelledOrders");

            migrationBuilder.RenameColumn(
                name: "QuantityOrdered",
                table: "CancelledOrders",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "OrderNo",
                table: "CancelledOrders",
                newName: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CancelledOrders_CustomerId",
                table: "CancelledOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CancelledOrders_OrderId",
                table: "CancelledOrders",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CancelledOrders_Customers_CustomerId",
                table: "CancelledOrders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CancelledOrders_Orders_OrderId",
                table: "CancelledOrders",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CancelledOrders_Customers_CustomerId",
                table: "CancelledOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_CancelledOrders_Orders_OrderId",
                table: "CancelledOrders");

            migrationBuilder.DropIndex(
                name: "IX_CancelledOrders_CustomerId",
                table: "CancelledOrders");

            migrationBuilder.DropIndex(
                name: "IX_CancelledOrders_OrderId",
                table: "CancelledOrders");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "CancelledOrders",
                newName: "QuantityOrdered");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "CancelledOrders",
                newName: "OrderNo");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "CancelledOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomersId",
                table: "CancelledOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateNeeded",
                table: "CancelledOrders",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ItemCode",
                table: "CancelledOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemDescription",
                table: "CancelledOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                table: "CancelledOrders",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Uom",
                table: "CancelledOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CancelledOrders_CustomersId",
                table: "CancelledOrders",
                column: "CustomersId");

            migrationBuilder.AddForeignKey(
                name: "FK_CancelledOrders_Customers_CustomersId",
                table: "CancelledOrders",
                column: "CustomersId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
