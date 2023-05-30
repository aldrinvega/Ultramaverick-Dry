using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddCustomerIdonCancellationEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CancelledOrders_Customers_CustomerId",
                table: "CancelledOrders");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "CancelledOrders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CancelledOrders_Customers_CustomerId",
                table: "CancelledOrders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CancelledOrders_Customers_CustomerId",
                table: "CancelledOrders");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "CancelledOrders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CancelledOrders_Customers_CustomerId",
                table: "CancelledOrders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
