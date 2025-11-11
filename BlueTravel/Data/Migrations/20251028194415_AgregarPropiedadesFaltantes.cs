using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueTravel.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPropiedadesFaltantes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HorariosDisponibles",
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
                name: "PoliticaCancelacion",
                table: "Hospedajes");

            migrationBuilder.RenameColumn(
                name: "CupoMaximo",
                table: "Tours",
                newName: "CuposTotales");

            migrationBuilder.AlterColumn<string>(
                name: "NivelDificultad",
                table: "Tours",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<decimal>(
                name: "DescuentoGrupo",
                table: "Tours",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "TipoHospedaje",
                table: "Hospedajes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CuposTotales",
                table: "Tours",
                newName: "CupoMaximo");

            migrationBuilder.AlterColumn<string>(
                name: "NivelDificultad",
                table: "Tours",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DescuentoGrupo",
                table: "Tours",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "HorariosDisponibles",
                table: "Tours",
                type: "nvarchar(200)",
                maxLength: 200,
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

            migrationBuilder.AlterColumn<string>(
                name: "TipoHospedaje",
                table: "Hospedajes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PoliticaCancelacion",
                table: "Hospedajes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
