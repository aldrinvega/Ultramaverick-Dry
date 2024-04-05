using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIR.DATA.Migrations
{
    /// <inheritdoc />
    public partial class AddCancellationRemarksOnOrderingEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "Orders",
                newName: "PreparingCancellationRemarks");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "Orders",
                newName: "OrderCancellationRemarks");

            migrationBuilder.AddColumn<string>(
                name: "MoveOrderCancellationRemarks",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoveOrderCancellationRemarks",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "PreparingCancellationRemarks",
                table: "Orders",
                newName: "Remarks");

            migrationBuilder.RenameColumn(
                name: "OrderCancellationRemarks",
                table: "Orders",
                newName: "Reason");
        }
    }
}
