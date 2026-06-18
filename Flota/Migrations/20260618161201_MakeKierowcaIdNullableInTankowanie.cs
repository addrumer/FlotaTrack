using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flota.Migrations
{
    /// <inheritdoc />
    public partial class MakeKierowcaIdNullableInTankowanie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tankowania_Kierowcy_KierowcaId",
                table: "Tankowania");

            migrationBuilder.AlterColumn<int>(
                name: "KierowcaId",
                table: "Tankowania",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Tankowania_Kierowcy_KierowcaId",
                table: "Tankowania",
                column: "KierowcaId",
                principalTable: "Kierowcy",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tankowania_Kierowcy_KierowcaId",
                table: "Tankowania");

            migrationBuilder.AlterColumn<int>(
                name: "KierowcaId",
                table: "Tankowania",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tankowania_Kierowcy_KierowcaId",
                table: "Tankowania",
                column: "KierowcaId",
                principalTable: "Kierowcy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
