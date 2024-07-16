using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESDashboard.Migrations
{
    /// <inheritdoc />
    public partial class DropTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PipeProductionReports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PipeProductionReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LadleCompositionDataId = table.Column<int>(type: "int", nullable: false),
                    LadleNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PipeNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Shift = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubReason = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipeProductionReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PipeProductionReports_LadleCompositionData_LadleCompositionDataId",
                        column: x => x.LadleCompositionDataId,
                        principalTable: "LadleCompositionData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PipeProductionReports_LadleCompositionDataId",
                table: "PipeProductionReports",
                column: "LadleCompositionDataId");
        }
    }
}
