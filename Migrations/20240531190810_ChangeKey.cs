using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESDashboard.Migrations
{
    /// <inheritdoc />
    public partial class ChangeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PipeProductionReports_LadleCompositionData_LadleCompositionDataId",
                table: "PipeProductionReports");

            migrationBuilder.DropIndex(
                name: "IX_PipeProductionReports_LadleCompositionDataId",
                table: "PipeProductionReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LadleCompositionData",
                table: "LadleCompositionData");

            migrationBuilder.DropColumn(
                name: "LadleCompositionDataId",
                table: "PipeProductionReports");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "LadleCompositionData");

            migrationBuilder.AddColumn<string>(
                name: "LadleCompositionDataLadleNo",
                table: "PipeProductionReports",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LadleNo",
                table: "LadleCompositionData",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LadleCompositionData",
                table: "LadleCompositionData",
                column: "LadleNo");

            migrationBuilder.CreateIndex(
                name: "IX_PipeProductionReports_LadleCompositionDataLadleNo",
                table: "PipeProductionReports",
                column: "LadleCompositionDataLadleNo");

            migrationBuilder.AddForeignKey(
                name: "FK_PipeProductionReports_LadleCompositionData_LadleCompositionDataLadleNo",
                table: "PipeProductionReports",
                column: "LadleCompositionDataLadleNo",
                principalTable: "LadleCompositionData",
                principalColumn: "LadleNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PipeProductionReports_LadleCompositionData_LadleCompositionDataLadleNo",
                table: "PipeProductionReports");

            migrationBuilder.DropIndex(
                name: "IX_PipeProductionReports_LadleCompositionDataLadleNo",
                table: "PipeProductionReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LadleCompositionData",
                table: "LadleCompositionData");

            migrationBuilder.DropColumn(
                name: "LadleCompositionDataLadleNo",
                table: "PipeProductionReports");

            migrationBuilder.AddColumn<int>(
                name: "LadleCompositionDataId",
                table: "PipeProductionReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "LadleNo",
                table: "LadleCompositionData",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "LadleCompositionData",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LadleCompositionData",
                table: "LadleCompositionData",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PipeProductionReports_LadleCompositionDataId",
                table: "PipeProductionReports",
                column: "LadleCompositionDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_PipeProductionReports_LadleCompositionData_LadleCompositionDataId",
                table: "PipeProductionReports",
                column: "LadleCompositionDataId",
                principalTable: "LadleCompositionData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
