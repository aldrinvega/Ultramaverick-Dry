using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIR.DATA.Migrations
{
    /// <inheritdoc />
    public partial class AddReasonOnOrderEntityForCancellationOfOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Orders");
        }
    }
}
