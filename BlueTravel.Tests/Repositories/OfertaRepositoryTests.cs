using BlueTravel.Data;
using BlueTravel.Data.Repositories;
using BlueTravel.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BlueTravel.Tests.Repositories
{
    public class OfertaRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly OfertaRepository _repository;
        private readonly Mock<ILogger<Repository<Oferta>>> _mockLogger;

        public OfertaRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockLogger = new Mock<ILogger<Repository<Oferta>>>();
            _repository = new OfertaRepository(_context, _mockLogger.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var ofertas = new List<Oferta>
            {
                // Oferta vigente
                new Oferta
                {
                    Id = 1,
                    Titulo = "Super Promo Verano",
                    Descripcion = "50% descuento en hospedajes",
                    Precio = 100,
                    FechaInicio = DateTime.Today.AddDays(-5),
                    FechaFin = DateTime.Today.AddDays(10), // Vigente
                },
                // Oferta próxima a vencer
                new Oferta
                {
                    Id = 2,
                    Titulo = "Última Oportunidad",
                    Descripcion = "Tours con descuento",
                    Precio = 75,
                    FechaInicio = DateTime.Today.AddDays(-10),
                    FechaFin = DateTime.Today.AddDays(2), // Vence en 2 días
                },
                // Oferta vencida
                new Oferta
                {
                    Id = 3,
                    Titulo = "Oferta Pasada",
                    Descripcion = "Ya venció",
                    Precio = 50,
                    FechaInicio = DateTime.Today.AddDays(-20),
                    FechaFin = DateTime.Today.AddDays(-1), // Ya venció
                },
                // Oferta futura
                new Oferta
                {
                    Id = 4,
                    Titulo = "Próxima Promo",
                    Descripcion = "Inicia pronto",
                    Precio = 120,
                    FechaInicio = DateTime.Today.AddDays(5), // Aún no inicia
                    FechaFin = DateTime.Today.AddDays(20),
                },
                // Mejor oferta (mayor descuento implícito)
                new Oferta
                {
                    Id = 5,
                    Titulo = "Black Friday",
                    Descripcion = "Mega descuentos",
                    Precio = 30, // Precio más bajo
                    FechaInicio = DateTime.Today.AddDays(-3),
                    FechaFin = DateTime.Today.AddDays(7),
                }
            };

            _context.Ofertas.AddRange(ofertas);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetActivasAsync_DebeRetornarSoloOfertasVigentes()
        {
            // Act
            var resultado = await _repository.GetActivasAsync();

            // Assert
            resultado.Should().HaveCount(3); // Ofertas 1, 2 y 5 están vigentes
            resultado.Should().OnlyContain(o => 
                o.FechaInicio <= DateTime.Today && 
                o.FechaFin >= DateTime.Today);
            resultado.Should().NotContain(o => o.Id == 3); // Oferta vencida
            resultado.Should().NotContain(o => o.Id == 4); // Oferta futura
        }

        [Fact]
        public async Task GetProximasAVencerAsync_DebeRetornarEnOrdenCorrecto()
        {
            // Arrange
            int dias = 5;

            // Act
            var resultado = await _repository.GetProximasAVencerAsync(dias);

            // Assert
            resultado.Should().NotBeEmpty();
            resultado.Should().OnlyContain(o => 
                (o.FechaFin - DateTime.Today).Days <= dias &&
                o.FechaFin >= DateTime.Today);
            
            // Verificar orden ascendente por FechaFin
            var fechas = resultado.Select(o => o.FechaFin).ToList();
            fechas.Should().BeInAscendingOrder();
        }

        [Fact]
        public async Task EstaVigenteAsync_OfertaVigente_DebeRetornarTrue()
        {
            // Arrange
            int ofertaId = 1; // Oferta vigente

            // Act
            var resultado = await _repository.EstaVigenteAsync(ofertaId);

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public async Task EstaVigenteAsync_OfertaVencida_DebeRetornarFalse()
        {
            // Arrange
            int ofertaId = 3; // Oferta vencida

            // Act
            var resultado = await _repository.EstaVigenteAsync(ofertaId);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task EstaVigenteAsync_OfertaFutura_DebeRetornarFalse()
        {
            // Arrange
            int ofertaId = 4; // Oferta que aún no inicia

            // Act
            var resultado = await _repository.EstaVigenteAsync(ofertaId);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task EstaVigenteAsync_OfertaInexistente_DebeRetornarFalse()
        {
            // Arrange
            int ofertaId = 999; // No existe

            // Act
            var resultado = await _repository.EstaVigenteAsync(ofertaId);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task GetMejoresOfertasAsync_DebeRetornarLimiteCorrecto()
        {
            // Arrange
            int limite = 2;

            // Act
            var resultado = await _repository.GetMejoresOfertasAsync(limite);

            // Assert
            resultado.Should().HaveCount(2);
            
            // Verificar orden por precio (menor precio = mejor oferta)
            var precios = resultado.Select(o => o.Precio).ToList();
            precios.Should().BeInAscendingOrder();
            
            // La mejor oferta debe ser "Black Friday" con precio 30
            resultado.First().Titulo.Should().Be("Black Friday");
        }

        [Fact]
        public async Task GetMejoresOfertasAsync_SinLimite_DebeRetornarTodasLasVigentes()
        {
            // Act
            var resultado = await _repository.GetMejoresOfertasAsync();

            // Assert
            resultado.Should().HaveCount(3); // Solo las vigentes (1, 2, 5)
            // Las ofertas están ordenadas por precio (menor = mejor)
        }

        [Fact]
        public async Task GetByIdAsync_OfertaExistente_DebeRetornarOferta()
        {
            // Arrange
            int ofertaId = 1;

            // Act
            var resultado = await _repository.GetByIdAsync(ofertaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Titulo.Should().Be("Super Promo Verano");
            resultado.Precio.Should().Be(100);
        }

        [Fact]
        public async Task GetAllAsync_DebeRetornarTodasLasOfertas()
        {
            // Act
            var resultado = await _repository.GetAllAsync();

            // Assert
            resultado.Should().HaveCount(5);
            resultado.Should().Contain(o => o.Titulo == "Super Promo Verano");
            resultado.Should().Contain(o => o.Titulo == "Oferta Pasada");
        }
    }
}
