using BlueTravel.Models;
using BlueTravel.Services;
using FluentAssertions;
using Xunit;

namespace BlueTravel.Tests.Services
{
    public class PrecioServiceTests
    {
        private readonly PrecioService _service;

        public PrecioServiceTests()
        {
            _service = new PrecioService();
        }

        #region Tests de Hospedaje

        [Fact]
        public async Task CalcularPrecioHospedaje_ConPersonasIncluidas_NoDebeAgregarCargo()
        {
            // Arrange
            var hospedaje = new Hospedaje
            {
                Id = 1,
                Nombre = "Hotel Test",
                PrecioPorNoche = 100,
                PersonasIncluidasEnPrecio = 2,
                CapacidadMaxima = 4,
                CargoPorPersonaExtra = 20
            };

            var fechaInicio = DateTime.Today.AddDays(1);
            var fechaFin = DateTime.Today.AddDays(3); // 2 noches

            // Act
            var resultado = await _service.CalcularPrecioHospedaje(
                hospedaje, 
                fechaInicio, 
                fechaFin, 
                cantidadAdultos: 2,  // Exactamente las personas incluidas
                cantidadNinos: 0, 
                cantidadSeniors: 0
            );

            // Assert
            resultado.PrecioBase.Should().Be(200); // 2 noches * 100
            resultado.CargoPersonasExtra.Should().Be(0); // No hay cargo extra
            resultado.Subtotal.Should().Be(200);
            resultado.Impuestos.Should().Be(26); // 13% de 200
            resultado.Total.Should().Be(226); // 200 + 26
            resultado.Desglose.Should().Contain("2 días");
        }

        [Fact]
        public async Task CalcularPrecioHospedaje_ConPersonasExtra_DebeAgregarCargo()
        {
            // Arrange
            var hospedaje = new Hospedaje
            {
                Id = 1,
                Nombre = "Hotel Test",
                PrecioPorNoche = 100,
                PersonasIncluidasEnPrecio = 2,
                CapacidadMaxima = 4,
                CargoPorPersonaExtra = 20
            };

            var fechaInicio = DateTime.Today.AddDays(1);
            var fechaFin = DateTime.Today.AddDays(3); // 2 noches

            // Act
            var resultado = await _service.CalcularPrecioHospedaje(
                hospedaje, 
                fechaInicio, 
                fechaFin, 
                cantidadAdultos: 3,  // 1 persona extra
                cantidadNinos: 0, 
                cantidadSeniors: 0
            );

            // Assert
            resultado.PrecioBase.Should().Be(200); // 2 noches * 100
            resultado.CargoPersonasExtra.Should().Be(40); // 2 noches * 20
            resultado.Subtotal.Should().Be(240);
            resultado.Impuestos.Should().Be(31.2m); // 13% de 240
            resultado.Total.Should().Be(271.2m);
        }

        [Theory]
        [InlineData(1, 100, 113)] // 1 noche (100 + 13% IVA)
        [InlineData(2, 200, 226)] // 2 noches (200 + 13% IVA)
        [InlineData(7, 700, 711.9)] // 1 semana (700 - 10% desc = 630 + 13% IVA = 711.9)
        public async Task CalcularPrecioHospedaje_CalculaCorrectamenteSegunNoches(
            int numeroNoches, 
            decimal precioBaseEsperado,
            decimal totalEsperado)
        {
            // Arrange
            var hospedaje = new Hospedaje
            {
                Id = 1,
                Nombre = "Hotel Test",
                PrecioPorNoche = 100,
                PersonasIncluidasEnPrecio = 2,
                CapacidadMaxima = 4,
                CargoPorPersonaExtra = 0
            };

            var fechaInicio = DateTime.Today.AddDays(1);
            var fechaFin = fechaInicio.AddDays(numeroNoches);

            // Act
            var resultado = await _service.CalcularPrecioHospedaje(
                hospedaje, 
                fechaInicio, 
                fechaFin, 
                cantidadAdultos: 2, 
                cantidadNinos: 0, 
                cantidadSeniors: 0
            );

            // Assert
            resultado.PrecioBase.Should().Be(precioBaseEsperado);
            resultado.Total.Should().Be(totalEsperado);
        }

        #endregion

        #region Tests de Tour

        [Fact]
        public async Task CalcularPrecioTour_SoloAdultos_CalculaCorrectamente()
        {
            // Arrange
            var tour = new Tour
            {
                Id = 1,
                Nombre = "Tour Test",
                Precio = 100,
                PrecioNino = 50,
                PrecioSenior = 75,
                DescuentoGrupo = 0
            };

            // Act
            var resultado = await _service.CalcularPrecioTour(
                tour, 
                cantidadAdultos: 2, 
                cantidadNinos: 0, 
                cantidadSeniors: 0
            );

            // Assert
            resultado.PrecioBase.Should().Be(200); // 2 * 100
            resultado.DescuentoGrupo.Should().Be(0);
            resultado.Subtotal.Should().Be(200);
            resultado.Impuestos.Should().Be(26); // 13% de 200
            resultado.Total.Should().Be(226);
        }

        [Fact]
        public async Task CalcularPrecioTour_ConNinosYSeniors_CalculaCorrectamente()
        {
            // Arrange
            var tour = new Tour
            {
                Id = 1,
                Nombre = "Tour Test",
                Precio = 100,
                PrecioNino = 50,
                PrecioSenior = 75,
                DescuentoGrupo = 0
            };

            // Act
            var resultado = await _service.CalcularPrecioTour(
                tour, 
                cantidadAdultos: 2,   // 2 * 100 = 200
                cantidadNinos: 2,     // 2 * 50 = 100
                cantidadSeniors: 1    // 1 * 75 = 75
            );

            // Assert
            resultado.PrecioBase.Should().Be(375); // 200 + 100 + 75
            resultado.Subtotal.Should().Be(375);
            resultado.Impuestos.Should().Be(48.75m); // 13% de 375
            resultado.Total.Should().Be(423.75m);
        }

        [Theory]
        [InlineData(10, 10, 1017.00)] // 10 personas, 10% descuento: 1000 - 100 = 900 + 117 IVA
        [InlineData(15, 15, 1440.75)] // 15 personas, 15% descuento: 1500 - 225 = 1275 + 165.75 IVA
        [InlineData(20, 20, 1808.00)] // 20 personas, 20% descuento: 2000 - 400 = 1600 + 208 IVA
        public async Task CalcularPrecioTour_ConGrupoGrande_AplicaDescuento(
            int cantidadAdultos, 
            int descuentoPorcentaje, 
            decimal totalEsperado)
        {
            // Arrange
            var tour = new Tour
            {
                Id = 1,
                Nombre = "Tour Test",
                Precio = 100,
                PrecioNino = 50,
                PrecioSenior = 75,
                DescuentoGrupo = descuentoPorcentaje
            };

            // Act
            var resultado = await _service.CalcularPrecioTour(
                tour, 
                cantidadAdultos: cantidadAdultos, 
                cantidadNinos: 0, 
                cantidadSeniors: 0
            );

            // Assert
            var precioBase = cantidadAdultos * 100;
            var descuentoEsperado = precioBase * (descuentoPorcentaje / 100m);
            
            resultado.PrecioBase.Should().Be(precioBase);
            resultado.DescuentoGrupo.Should().Be(descuentoEsperado);
            resultado.Total.Should().BeApproximately(totalEsperado, 0.01m);
        }

        [Fact]
        public async Task CalcularPrecioTour_ConMenosDe10Personas_NoAplicaDescuentoGrupo()
        {
            // Arrange
            var tour = new Tour
            {
                Id = 1,
                Nombre = "Tour Test",
                Precio = 100,
                PrecioNino = 50,
                PrecioSenior = 75,
                DescuentoGrupo = 15 // Tiene descuento configurado pero no se aplica
            };

            // Act
            var resultado = await _service.CalcularPrecioTour(
                tour, 
                cantidadAdultos: 5,  // Menos de 10 personas
                cantidadNinos: 0, 
                cantidadSeniors: 0
            );

            // Assert
            resultado.PrecioBase.Should().Be(500);
            resultado.DescuentoGrupo.Should().Be(0); // No se aplica el descuento
            resultado.Subtotal.Should().Be(500);
            resultado.Impuestos.Should().Be(65); // 13% de 500
            resultado.Total.Should().Be(565);
        }

        #endregion

        #region Tests de Validación

        [Fact]
        public async Task CalcularPrecioHospedaje_ConFechaFinAntesDeFechaInicio_DebeLanzarExcepcion()
        {
            // Arrange
            var hospedaje = new Hospedaje
            {
                Id = 1,
                Nombre = "Hotel Test",
                PrecioPorNoche = 100,
                PersonasIncluidasEnPrecio = 2,
                CapacidadMaxima = 4,
                CargoPorPersonaExtra = 20
            };

            var fechaInicio = DateTime.Today.AddDays(3);
            var fechaFin = DateTime.Today.AddDays(1); // Antes de inicio

            // Act & Assert
            var act = async () => await _service.CalcularPrecioHospedaje(
                hospedaje, 
                fechaInicio, 
                fechaFin, 
                cantidadAdultos: 2, 
                cantidadNinos: 0, 
                cantidadSeniors: 0
            );

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*fecha de fin debe ser posterior*");
        }

        [Fact]
        public async Task CalcularPrecioTour_SinPersonas_DebeLanzarExcepcion()
        {
            // Arrange
            var tour = new Tour
            {
                Id = 1,
                Nombre = "Tour Test",
                Precio = 100,
                PrecioNino = 50,
                PrecioSenior = 75,
                DescuentoGrupo = 0
            };

            // Act & Assert
            var act = async () => await _service.CalcularPrecioTour(
                tour, 
                cantidadAdultos: 0,  // Cero personas
                cantidadNinos: 0, 
                cantidadSeniors: 0
            );

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*debe haber al menos una persona*");
        }

        #endregion
    }
}
