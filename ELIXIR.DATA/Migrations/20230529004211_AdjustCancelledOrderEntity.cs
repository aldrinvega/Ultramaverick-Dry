using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AdjustCancelledOrderEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CancelledOrders_Orders_OrderId",
                table: "CancelledOrders");

            migrationBuilder.DropIndex(
                name: "IX_CancelledOrders_OrderId",
                table: "CancelledOrders");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "CancelledOrders");

            migrationBuilder.DropColumn(
                name: "CustomerPosition",
                table: "CancelledOrders");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "CancelledOrders");

            migrationBuilder.AddColumn<int>(
                name: "CancelledOrdersId",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CancelledOrdersId",
                table: "Customers",
                column: "CancelledOrdersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_CancelledOrders_CancelledOrdersId",
                table: "Customers",
                column: "CancelledOrdersId",
                principalTable: "CancelledOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_CancelledOrders_CancelledOrdersId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_CancelledOrdersId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CancelledOrdersId",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "CancelledOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerPosition",
                table: "CancelledOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "CancelledOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CancelledOrders_OrderId",
                table: "CancelledOrders",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CancelledOrders_Orders_OrderId",
                table: "CancelledOrders",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
