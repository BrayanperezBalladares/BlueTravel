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
    public class TourRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly TourRepository _repository;
        private readonly Mock<ILogger<Repository<Tour>>> _mockLogger;

        public TourRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockLogger = new Mock<ILogger<Repository<Tour>>>();
            _repository = new TourRepository(_context, _mockLogger.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var tours = new List<Tour>
            {
                new Tour
                {
                    Id = 1,
                    Nombre = "Tour Volcán Arenal",
                    Descripcion = "Aventura al volcán más activo",
                    Ubicacion = "La Fortuna",
                    Precio = 75,
                    PrecioNino = 50,
                    PrecioSenior = 60,
                    CuposTotales = 20,
                    CuposReservados = 5,
                    Duracion = 8,
                    NivelDificultad = "Moderado",
                    FechaDisponible = DateTime.Today.AddDays(5),
                    DescuentoGrupo = 10,
                    EdadMinima = 8,
                    EdadMaxima = 70,
                    RequiereConfirmacion = false
                },
                new Tour
                {
                    Id = 2,
                    Nombre = "Canopy Adventure",
                    Descripcion = "Tirolesa extrema",
                    Ubicacion = "Monteverde",
                    Precio = 90,
                    PrecioNino = 70,
                    PrecioSenior = 80,
                    CuposTotales = 15,
                    CuposReservados = 14, // Casi lleno
                    Duracion = 4,
                    NivelDificultad = "Difícil",
                    FechaDisponible = DateTime.Today.AddDays(10),
                    DescuentoGrupo = 15,
                    EdadMinima = 12,
                    EdadMaxima = 65,
                    RequiereConfirmacion = true
                },
                new Tour
                {
                    Id = 3,
                    Nombre = "Playa Snorkeling",
                    Descripcion = "Snorkel en aguas cristalinas",
                    Ubicacion = "Tamarindo",
                    Precio = 60,
                    PrecioNino = 40,
                    PrecioSenior = 50,
                    CuposTotales = 25,
                    CuposReservados = 0,
                    Duracion = 5,
                    NivelDificultad = "Fácil",
                    FechaDisponible = DateTime.Today.AddDays(2),
                    DescuentoGrupo = 0,
                    EdadMinima = 5,
                    EdadMaxima = null,
                    RequiereConfirmacion = false
                },
                new Tour
                {
                    Id = 4,
                    Nombre = "Rafting Río Pacuare",
                    Descripcion = "Rafting clase IV",
                    Ubicacion = "Turrialba",
                    Precio = 120,
                    PrecioNino = 0, // No permite niños
                    PrecioSenior = 100,
                    CuposTotales = 12,
                    CuposReservados = 12, // Completamente lleno
                    Duracion = 6,
                    NivelDificultad = "Difícil",
                    FechaDisponible = DateTime.Today.AddDays(15),
                    DescuentoGrupo = 20,
                    EdadMinima = 16,
                    EdadMaxima = 60,
                    RequiereConfirmacion = true
                }
            };

            _context.Tours.AddRange(tours);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetAllAsync_DebeRetornarTodosLosTours()
        {
            // Act
            var resultado = await _repository.GetAllAsync();

            // Assert
            resultado.Should().HaveCount(4);
            resultado.Should().Contain(t => t.Nombre == "Tour Volcán Arenal");
        }

        [Fact]
        public async Task GetDisponiblesAsync_DebeRetornarSoloToursConCupos()
        {
            // Act
            var resultado = await _repository.GetDisponiblesAsync();

            // Assert
            resultado.Should().HaveCount(3); // Tours 1, 2 y 3 tienen cupos disponibles
            resultado.Should().OnlyContain(t => t.CuposReservados < t.CuposTotales);
            resultado.Should().NotContain(t => t.Id == 4); // Tour 4 está lleno
        }

        [Fact]
        public async Task GetByNivelDificultadAsync_DebeRetornarToursFiltrados()
        {
            // Act
            var resultado = await _repository.GetByNivelDificultadAsync("Moderado");

            // Assert
            resultado.Should().HaveCount(1);
            resultado.First().Nombre.Should().Be("Tour Volcán Arenal");
            resultado.First().NivelDificultad.Should().Be("Moderado");
        }

        [Fact]
        public async Task GetProximosAsync_DebeRetornarToursEnRango()
        {
            // Act
            var resultado = await _repository.GetProximosAsync(7); // Próximos 7 días

            // Assert
            resultado.Should().HaveCount(2); // Tours 1 (día 5) y 3 (día 2)
            resultado.Should().OnlyContain(t => 
                t.FechaDisponible >= DateTime.Today && 
                t.FechaDisponible <= DateTime.Today.AddDays(7));
        }

        [Fact]
        public async Task TieneCuposDisponiblesAsync_ConCuposSuficientes_DebeRetornarTrue()
        {
            // Arrange
            int tourId = 1; // Tour con 15 cupos disponibles (20 - 5)
            int personasSolicitadas = 10;

            // Act
            var resultado = await _repository.TieneCuposDisponiblesAsync(tourId, personasSolicitadas);

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public async Task TieneCuposDisponiblesAsync_SinCuposSuficientes_DebeRetornarFalse()
        {
            // Arrange
            int tourId = 2; // Tour con solo 1 cupo (15 - 14)
            int personasSolicitadas = 5;

            // Act
            var resultado = await _repository.TieneCuposDisponiblesAsync(tourId, personasSolicitadas);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task ReservarCuposAsync_DebeReducirCuposDisponibles()
        {
            // Arrange
            int tourId = 1;
            int cuposAReservar = 5;

            // Act
            var resultado = await _repository.ReservarCuposAsync(tourId, cuposAReservar);

            // Assert
            resultado.Should().BeTrue();
            
            var tour = await _repository.GetByIdAsync(tourId);
            tour!.CuposReservados.Should().Be(10); // Era 5, ahora 10
            tour.CuposDisponibles.Should().Be(10); // Era 15, ahora 10
        }

        [Fact]
        public async Task ReservarCuposAsync_ExcediendoCapacidad_DebeRetornarFalse()
        {
            // Arrange
            int tourId = 2; // Solo 1 cupo disponible
            int cuposAReservar = 5;

            // Act
            var resultado = await _repository.ReservarCuposAsync(tourId, cuposAReservar);

            // Assert
            resultado.Should().BeFalse();
            
            // Verificar que no se modificaron los cupos
            var tour = await _repository.GetByIdAsync(tourId);
            tour!.CuposReservados.Should().Be(14); // No cambió
        }

        [Fact]
        public async Task LiberarCuposAsync_DebeAumentarCuposDisponibles()
        {
            // Arrange
            int tourId = 1;
            int cuposALiberar = 3;

            // Act
            var resultado = await _repository.LiberarCuposAsync(tourId, cuposALiberar);

            // Assert
            resultado.Should().BeTrue();
            
            var tour = await _repository.GetByIdAsync(tourId);
            tour!.CuposReservados.Should().Be(2); // Era 5, liberó 3
            tour.CuposDisponibles.Should().Be(18); // Era 15, ahora 18
        }

        [Fact]
        public async Task LiberarCuposAsync_MasQueReservados_DebeLimitarACero()
        {
            // Arrange
            int tourId = 1; // 5 cupos reservados
            int cuposALiberar = 10; // Más de los reservados

            // Act
            var resultado = await _repository.LiberarCuposAsync(tourId, cuposALiberar);

            // Assert
            resultado.Should().BeTrue();
            
            var tour = await _repository.GetByIdAsync(tourId);
            tour!.CuposReservados.Should().Be(0); // No puede ser negativo
        }

        [Fact]
        public async Task GetConDescuentoGrupoAsync_DebeRetornarSoloConDescuento()
        {
            // Act
            var resultado = await _repository.GetConDescuentoGrupoAsync();

            // Assert
            resultado.Should().HaveCount(3); // Tours 1, 2 y 4 tienen descuento
            resultado.Should().OnlyContain(t => t.DescuentoGrupo > 0);
            resultado.Should().NotContain(t => t.Id == 3); // Tour 3 no tiene descuento
        }

        [Fact]
        public async Task GetByUbicacionAsync_DebeRetornarToursFiltrados()
        {
            // Act
            var resultado = await _repository.GetByUbicacionAsync("La Fortuna");

            // Assert
            resultado.Should().HaveCount(1);
            resultado.First().Nombre.Should().Be("Tour Volcán Arenal");
        }
    }
}
