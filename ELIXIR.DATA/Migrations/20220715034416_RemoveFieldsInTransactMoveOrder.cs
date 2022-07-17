using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class RemoveFieldsInTransactMoveOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "TransactMoveOrder");

            migrationBuilder.DropColumn(
                name: "DateNeeded",
                table: "TransactMoveOrder");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "TransactMoveOrder");

            migrationBuilder.DropColumn(
                name: "ItemCode",
                table: "TransactMoveOrder");

            migrationBuilder.DropColumn(
                name: "ItemDescription",
                table: "TransactMoveOrder");

            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "TransactMoveOrder");

            migrationBuilder.DropColumn(
                name: "PlateNumber",
                table: "TransactMoveOrder");

            migrationBuilder.DropColumn(
                name: "QuantityOrdered",
                table: "TransactMoveOrder");

            migrationBuilder.DropColumn(
                name: "Uom",
                table: "TransactMoveOrder");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "TransactMoveOrder");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "TransactMoveOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateNeeded",
                table: "TransactMoveOrder",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "TransactMoveOrder",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemCode",
                table: "TransactMoveOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemDescription",
                table: "TransactMoveOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                table: "TransactMoveOrder",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PlateNumber",
                table: "TransactMoveOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "QuantityOrdered",
                table: "TransactMoveOrder",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Uom",
                table: "TransactMoveOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "TransactMoveOrder",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
