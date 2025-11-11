using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueTravel.Data.Migrations
{
    /// <inheritdoc />
    public partial class SistemaPreciosRealista : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DescuentoGrupo",
                table: "Tours",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "MinimoPersonasDescuento",
                table: "Tours",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CapacidadBase",
                table: "Hospedajes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CapacidadMaxima",
                table: "Hospedajes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioPorPersonaAdicional",
                table: "Hospedajes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescuentoGrupo",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "MinimoPersonasDescuento",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "CapacidadBase",
                table: "Hospedajes");

            migrationBuilder.DropColumn(
                name: "CapacidadMaxima",
                table: "Hospedajes");

            migrationBuilder.DropColumn(
                name: "PrecioPorPersonaAdicional",
                table: "Hospedajes");
        }
    }
}
