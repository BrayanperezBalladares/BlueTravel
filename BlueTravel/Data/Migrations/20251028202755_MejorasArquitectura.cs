using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueTravel.Data.Migrations
{
    /// <inheritdoc />
    public partial class MejorasArquitectura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Usuario",
                table: "Pagos");

            migrationBuilder.RenameColumn(
                name: "Monto",
                table: "Pagos",
                newName: "TotalPagado");

            migrationBuilder.RenameColumn(
                name: "Fecha",
                table: "Pagos",
                newName: "FechaCreacion");

            migrationBuilder.AddColumn<string>(
                name: "EstadoAnterior",
                table: "Reservas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCambioEstado",
                table: "Reservas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModificadoPor",
                table: "Reservas",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PagoId",
                table: "Reservas",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Metodo",
                table: "Pagos",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<decimal>(
                name: "CargosAdicionales",
                table: "Pagos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Descuentos",
                table: "Pagos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "EsReembolso",
                table: "Pagos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Pagos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAprobacion",
                table: "Pagos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCancelacion",
                table: "Pagos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Impuestos",
                table: "Pagos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "MarcaTarjeta",
                table: "Pagos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoBase",
                table: "Pagos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "NotasInternas",
                table: "Pagos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PagoOriginalId",
                table: "Pagos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenciaBancaria",
                table: "Pagos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReservaId",
                table: "Pagos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TransaccionExternaId",
                table: "Pagos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UltimosDigitosTarjeta",
                table: "Pagos",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "Pagos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_PagoId",
                table: "Reservas",
                column: "PagoId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_ReservaId",
                table: "Pagos",
                column: "ReservaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_Reservas_ReservaId",
                table: "Pagos",
                column: "ReservaId",
                principalTable: "Reservas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Pagos_PagoId",
                table: "Reservas",
                column: "PagoId",
                principalTable: "Pagos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_Reservas_ReservaId",
                table: "Pagos");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Pagos_PagoId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_PagoId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Pagos_ReservaId",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "EstadoAnterior",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "FechaCambioEstado",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "ModificadoPor",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "PagoId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "CargosAdicionales",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "Descuentos",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "EsReembolso",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "FechaAprobacion",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "FechaCancelacion",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "Impuestos",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "MarcaTarjeta",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "MontoBase",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "NotasInternas",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "PagoOriginalId",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "ReferenciaBancaria",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "ReservaId",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "TransaccionExternaId",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "UltimosDigitosTarjeta",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Pagos");

            migrationBuilder.RenameColumn(
                name: "TotalPagado",
                table: "Pagos",
                newName: "Monto");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "Pagos",
                newName: "Fecha");

            migrationBuilder.AlterColumn<string>(
                name: "Metodo",
                table: "Pagos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Usuario",
                table: "Pagos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
