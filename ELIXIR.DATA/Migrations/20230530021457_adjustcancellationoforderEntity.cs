using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class adjustcancellationoforderEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "CancelledOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CancelledOrders_CustomerId",
                table: "CancelledOrders",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CancelledOrders_Customers_CustomerId",
                table: "CancelledOrders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CancelledOrders_Customers_CustomerId",
                table: "CancelledOrders");

            migrationBuilder.DropIndex(
                name: "IX_CancelledOrders_CustomerId",
                table: "CancelledOrders");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "CancelledOrders");
        }
    }
}
