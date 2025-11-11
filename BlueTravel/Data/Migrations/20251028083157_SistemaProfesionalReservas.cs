using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueTravel.Data.Migrations
{
    /// <inheritdoc />
    public partial class SistemaProfesionalReservas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinimoPersonasDescuento",
                table: "Tours",
                newName: "EdadMinima");

            migrationBuilder.RenameColumn(
                name: "CantidadPersonas",
                table: "Reservas",
                newName: "CantidadSeniors");

            migrationBuilder.RenameColumn(
                name: "PrecioPorPersonaAdicional",
                table: "Hospedajes",
                newName: "CargoPorPersonaExtra");

            migrationBuilder.RenameColumn(
                name: "CapacidadBase",
                table: "Hospedajes",
                newName: "PersonasIncluidasEnPrecio");

            migrationBuilder.AlterColumn<int>(
                name: "DescuentoGrupo",
                table: "Tours",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "CupoMaximo",
                table: "Tours",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CuposReservados",
                table: "Tours",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EdadMaxima",
                table: "Tours",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HorariosDisponibles",
                table: "Tours",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NivelDificultad",
                table: "Tours",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioNino",
                table: "Tours",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioSenior",
                table: "Tours",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QueIncluye",
                table: "Tours",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QueLlevar",
                table: "Tours",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QueNoIncluye",
                table: "Tours",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiereConfirmacion",
                table: "Tours",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CantidadAdultos",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CantidadNinos",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CargoPersonasExtra",
                table: "Reservas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ConfirmadoPor",
                table: "Reservas",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DescuentoAplicado",
                table: "Reservas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaConfirmacion",
                table: "Reservas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoRechazo",
                table: "Reservas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioBase",
                table: "Reservas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "RequiereConfirmacion",
                table: "Reservas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "HoraCheckIn",
                table: "Hospedajes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HoraCheckOut",
                table: "Hospedajes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "PermiteMascotas",
                table: "Hospedajes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PermiteNinos",
                table: "Hospedajes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PoliticaCancelacion",
                table: "Hospedajes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoHospedaje",
                table: "Hospedajes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CupoMaximo",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "CuposReservados",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "EdadMaxima",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "HorariosDisponibles",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "NivelDificultad",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "PrecioNino",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "PrecioSenior",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "QueIncluye",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "QueLlevar",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "QueNoIncluye",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "RequiereConfirmacion",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "CantidadAdultos",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "CantidadNinos",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "CargoPersonasExtra",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "ConfirmadoPor",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "DescuentoAplicado",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "FechaConfirmacion",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "MotivoRechazo",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "PrecioBase",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "RequiereConfirmacion",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "HoraCheckIn",
                table: "Hospedajes");

            migrationBuilder.DropColumn(
                name: "HoraCheckOut",
                table: "Hospedajes");

            migrationBuilder.DropColumn(
                name: "PermiteMascotas",
                table: "Hospedajes");

            migrationBuilder.DropColumn(
                name: "PermiteNinos",
                table: "Hospedajes");

            migrationBuilder.DropColumn(
                name: "PoliticaCancelacion",
                table: "Hospedajes");

            migrationBuilder.DropColumn(
                name: "TipoHospedaje",
                table: "Hospedajes");

            migrationBuilder.RenameColumn(
                name: "EdadMinima",
                table: "Tours",
                newName: "MinimoPersonasDescuento");

            migrationBuilder.RenameColumn(
                name: "CantidadSeniors",
                table: "Reservas",
                newName: "CantidadPersonas");

            migrationBuilder.RenameColumn(
                name: "PersonasIncluidasEnPrecio",
                table: "Hospedajes",
                newName: "CapacidadBase");

            migrationBuilder.RenameColumn(
                name: "CargoPorPersonaExtra",
                table: "Hospedajes",
                newName: "PrecioPorPersonaAdicional");

            migrationBuilder.AlterColumn<decimal>(
                name: "DescuentoGrupo",
                table: "Tours",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
