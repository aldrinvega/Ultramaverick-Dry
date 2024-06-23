using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIR.DATA.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactIdOnMoveOrderEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "MoveOrders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "MoveOrders");
        }
    }
}
