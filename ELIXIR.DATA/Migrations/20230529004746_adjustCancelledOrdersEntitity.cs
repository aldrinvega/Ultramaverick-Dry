using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class adjustCancelledOrdersEntitity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "CustomersId",
                table: "CancelledOrders",
                type: "int",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CancelledOrders_Customers_CustomersId",
                table: "CancelledOrders");

            migrationBuilder.DropIndex(
                name: "IX_CancelledOrders_CustomersId",
                table: "CancelledOrders");

            migrationBuilder.DropColumn(
                name: "CustomersId",
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
    }
}
