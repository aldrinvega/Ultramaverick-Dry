using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AdjustCancelledOrdersEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CancelledOrders_Orders_OrdersId",
                table: "CancelledOrders");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "CancelledOrders");

            migrationBuilder.AlterColumn<int>(
                name: "OrdersId",
                table: "CancelledOrders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CancelledOrders_Orders_OrdersId",
                table: "CancelledOrders",
                column: "OrdersId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CancelledOrders_Orders_OrdersId",
                table: "CancelledOrders");

            migrationBuilder.AlterColumn<int>(
                name: "OrdersId",
                table: "CancelledOrders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "CancelledOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_CancelledOrders_Orders_OrdersId",
                table: "CancelledOrders",
                column: "OrdersId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
