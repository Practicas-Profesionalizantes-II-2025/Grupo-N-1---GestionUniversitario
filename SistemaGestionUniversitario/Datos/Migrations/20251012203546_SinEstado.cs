using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datos.Migrations
{
    /// <inheritdoc />
    public partial class SinEstado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencia_DiaHorarioMateria_IdDiaHorarioMateria",
                table: "Asistencia");

            migrationBuilder.AddColumn<bool>(
                name: "Estado",
                table: "Inscripcion",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Fecha",
                table: "Examen",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "IdDiaHorarioMateria",
                table: "Asistencia",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencia_DiaHorarioMateria_IdDiaHorarioMateria",
                table: "Asistencia",
                column: "IdDiaHorarioMateria",
                principalTable: "DiaHorarioMateria",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencia_DiaHorarioMateria_IdDiaHorarioMateria",
                table: "Asistencia");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Inscripcion");

            migrationBuilder.DropColumn(
                name: "Fecha",
                table: "Examen");

            migrationBuilder.AlterColumn<int>(
                name: "IdDiaHorarioMateria",
                table: "Asistencia",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencia_DiaHorarioMateria_IdDiaHorarioMateria",
                table: "Asistencia",
                column: "IdDiaHorarioMateria",
                principalTable: "DiaHorarioMateria",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
