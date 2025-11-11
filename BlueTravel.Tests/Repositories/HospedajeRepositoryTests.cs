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
    public class HospedajeRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly HospedajeRepository _repository;
        private readonly Mock<ILogger<Repository<Hospedaje>>> _mockLogger;

        public HospedajeRepositoryTests()
        {
            // Configurar InMemory Database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockLogger = new Mock<ILogger<Repository<Hospedaje>>>();
            _repository = new HospedajeRepository(_context, _mockLogger.Object);

            // Seed data
            SeedTestData();
        }

        private void SeedTestData()
        {
            var hospedajes = new List<Hospedaje>
            {
                new Hospedaje
                {
                    Id = 1,
                    Nombre = "Hotel Playa Hermosa",
                    Ubicacion = "Guanacaste",
                    PrecioPorNoche = 100,
                    CapacidadMaxima = 4,
                    PersonasIncluidasEnPrecio = 2,
                    CargoPorPersonaExtra = 20,
                    PermiteNinos = true,
                    PermiteMascotas = false
                },
                new Hospedaje
                {
                    Id = 2,
                    Nombre = "Cabina Monteverde",
                    Ubicacion = "Puntarenas",
                    PrecioPorNoche = 80,
                    CapacidadMaxima = 6,
                    PersonasIncluidasEnPrecio = 4,
                    CargoPorPersonaExtra = 15,
                    PermiteNinos = true,
                    PermiteMascotas = true
                },
                new Hospedaje
                {
                    Id = 3,
                    Nombre = "Resort Premium",
                    Ubicacion = "Guanacaste",
                    PrecioPorNoche = 250,
                    CapacidadMaxima = 2,
                    PersonasIncluidasEnPrecio = 2,
                    CargoPorPersonaExtra = 0,
                    PermiteNinos = false,
                    PermiteMascotas = false
                }
            };

            _context.Hospedajes.AddRange(hospedajes);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region Tests Básicos

        [Fact]
        public async Task GetAllAsync_DebeRetornarTodosLosHospedajes()
        {
            // Act
            var resultado = await _repository.GetAllAsync();

            // Assert
            resultado.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetByIdAsync_ConIdValido_DebeRetornarHospedaje()
        {
            // Act
            var resultado = await _repository.GetByIdAsync(1);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Nombre.Should().Be("Hotel Playa Hermosa");
        }

        [Fact]
        public async Task GetByIdAsync_ConIdInvalido_DebeRetornarNull()
        {
            // Act
            var resultado = await _repository.GetByIdAsync(999);

            // Assert
            resultado.Should().BeNull();
        }

        #endregion

        #region Tests Específicos de Hospedaje

        [Fact]
        public async Task GetByUbicacionAsync_DebeRetornarHospedajesDeGuanacaste()
        {
            // Act
            var resultado = await _repository.GetByUbicacionAsync("Guanacaste");

            // Assert
            resultado.Should().HaveCount(2);
            resultado.Should().OnlyContain(h => h.Ubicacion.Contains("Guanacaste"));
        }

        [Fact]
        public async Task GetByRangoPrecioAsync_DebeRetornarHospedajesEnRango()
        {
            // Act
            var resultado = await _repository.GetByRangoPrecioAsync(50, 150);

            // Assert
            resultado.Should().HaveCount(2);
            resultado.Should().OnlyContain(h => h.PrecioPorNoche >= 50 && h.PrecioPorNoche <= 150);
        }

        [Fact]
        public async Task GetQuePermitenNinosAsync_DebeRetornarSoloLosQuePermitenNinos()
        {
            // Act
            var resultado = await _repository.GetQuePermitenNinosAsync();

            // Assert
            resultado.Should().HaveCount(2);
            resultado.Should().OnlyContain(h => h.PermiteNinos);
        }

        [Fact]
        public async Task GetQuePermitenMascotasAsync_DebeRetornarSoloLosQuePermitenMascotas()
        {
            // Act
            var resultado = await _repository.GetQuePermitenMascotasAsync();

            // Assert
            resultado.Should().HaveCount(1);
            resultado.First().Nombre.Should().Be("Cabina Monteverde");
        }

        [Fact]
        public async Task GetConCapacidadMinimaAsync_DebeRetornarHospedajesConCapacidadSuficiente()
        {
            // Act
            var resultado = await _repository.GetConCapacidadMinimaAsync(5);

            // Assert
            resultado.Should().HaveCount(1);
            resultado.First().CapacidadMaxima.Should().BeGreaterThanOrEqualTo(5);
        }

        [Fact]
        public async Task VerificarDisponibilidadAsync_SinReservas_DebeRetornarTrue()
        {
            // Arrange
            var fechaInicio = DateTime.Today.AddDays(1);
            var fechaFin = DateTime.Today.AddDays(3);

            // Act
            var resultado = await _repository.VerificarDisponibilidadAsync(1, fechaInicio, fechaFin);

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public async Task VerificarDisponibilidadAsync_ConReservaConflictiva_DebeRetornarFalse()
        {
            // Arrange
            var reserva = new Reserva
            {
                Id = 1,
                TipoReserva = "Hospedaje",
                ItemId = 1,
                ItemNombre = "Hotel Playa Hermosa",
                UsuarioId = "test-user",
                FechaInicio = DateTime.Today.AddDays(2),
                FechaFin = DateTime.Today.AddDays(5),
                Estado = "Confirmada",
                CantidadAdultos = 2,
                PrecioTotal = 300
            };

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            var fechaInicio = DateTime.Today.AddDays(1);
            var fechaFin = DateTime.Today.AddDays(3);

            // Act
            var resultado = await _repository.VerificarDisponibilidadAsync(1, fechaInicio, fechaFin);

            // Assert
            resultado.Should().BeFalse();
        }

        #endregion

        #region Tests de Validación

        [Fact]
        public async Task GetByUbicacionAsync_ConUbicacionVacia_DebeLanzarException()
        {
            // Act
            var act = async () => await _repository.GetByUbicacionAsync(string.Empty);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetByRangoPrecioAsync_ConPrecioNegativo_DebeLanzarException()
        {
            // Act
            var act = async () => await _repository.GetByRangoPrecioAsync(-10, 100);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetConCapacidadMinimaAsync_ConCapacidadCero_DebeLanzarException()
        {
            // Act
            var act = async () => await _repository.GetConCapacidadMinimaAsync(0);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        #endregion

        #region Tests de Operaciones CRUD

        [Fact]
        public async Task AddAsync_DebeAgregarHospedaje()
        {
            // Arrange
            var nuevoHospedaje = new Hospedaje
            {
                Nombre = "Nuevo Hotel",
                Ubicacion = "San José",
                PrecioPorNoche = 120,
                CapacidadMaxima = 4,
                PersonasIncluidasEnPrecio = 2,
                CargoPorPersonaExtra = 25
            };

            // Act
            await _repository.AddAsync(nuevoHospedaje);
            await _repository.SaveChangesAsync();

            // Assert
            var todos = await _repository.GetAllAsync();
            todos.Should().HaveCount(4);
        }

        [Fact]
        public async Task UpdateAsync_DebeActualizarHospedaje()
        {
            // Arrange
            var hospedaje = await _repository.GetByIdAsync(1);
            hospedaje!.PrecioPorNoche = 150;

            // Act
            await _repository.UpdateAsync(hospedaje);
            await _repository.SaveChangesAsync();

            // Assert
            var actualizado = await _repository.GetByIdAsync(1);
            actualizado!.PrecioPorNoche.Should().Be(150);
        }

        [Fact]
        public async Task DeleteAsync_DebeEliminarHospedaje()
        {
            // Act
            await _repository.DeleteAsync(3);
            await _repository.SaveChangesAsync();

            // Assert
            var todos = await _repository.GetAllAsync();
            todos.Should().HaveCount(2);
        }

        #endregion
    }
}
