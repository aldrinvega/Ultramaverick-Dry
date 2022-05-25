using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddNewTableTransformationReject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transformation_Reject",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransformId = table.Column<int>(type: "int", nullable: false),
                    FormulaCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormulaDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Uom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Batch = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RawmaterialCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RawmaterialDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProdPlan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RejectedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RejectedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transformation_Reject", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transformation_Reject");
        }
    }
}
