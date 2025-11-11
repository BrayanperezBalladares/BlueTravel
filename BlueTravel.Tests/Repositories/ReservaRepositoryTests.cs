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
    public class ReservaRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ReservaRepository _repository;
        private readonly Mock<ILogger<Repository<Reserva>>> _mockLogger;
        private const string USER_ID_1 = "user123";
        private const string USER_ID_2 = "user456";

        public ReservaRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockLogger = new Mock<ILogger<Repository<Reserva>>>();
            _repository = new ReservaRepository(_context, _mockLogger.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var reservas = new List<Reserva>
            {
                // Reservas de User 1
                new Reserva
                {
                    Id = 1,
                    UsuarioId = USER_ID_1,
                    TipoReserva = "Hospedaje",
                    ItemId = 1,
                    ItemNombre = "Hotel Arenal",
                    FechaInicio = DateTime.Today.AddDays(5),
                    FechaFin = DateTime.Today.AddDays(7),
                    CantidadAdultos = 2,
                    CantidadNinos = 1,
                    CantidadSeniors = 0,
                    PrecioTotal = 300,
                    Estado = "Confirmada",
                    FechaCreacion = DateTime.Today.AddDays(-2),
                    PagoId = 1 // Tiene pago asociado
                },
                new Reserva
                {
                    Id = 2,
                    UsuarioId = USER_ID_1,
                    TipoReserva = "Tour",
                    ItemId = 2,
                    ItemNombre = "Tour Volcán",
                    FechaInicio = DateTime.Today.AddDays(10),
                    FechaFin = DateTime.Today.AddDays(10),
                    CantidadAdultos = 3,
                    CantidadNinos = 0,
                    CantidadSeniors = 1,
                    PrecioTotal = 250,
                    Estado = "Pendiente",
                    FechaCreacion = DateTime.Today.AddDays(-1),
                    PagoId = null // Sin pago
                },
                new Reserva
                {
                    Id = 3,
                    UsuarioId = USER_ID_1,
                    TipoReserva = "Hospedaje",
                    ItemId = 3,
                    ItemNombre = "Cabina Playa",
                    FechaInicio = DateTime.Today.AddDays(-5),
                    FechaFin = DateTime.Today.AddDays(-3),
                    CantidadAdultos = 2,
                    CantidadNinos = 0,
                    CantidadSeniors = 0,
                    PrecioTotal = 150,
                    Estado = "Completada",
                    FechaCreacion = DateTime.Today.AddDays(-10),
                    PagoId = 2 // Tiene pago asociado
                },
                // Reservas de User 2
                new Reserva
                {
                    Id = 4,
                    UsuarioId = USER_ID_2,
                    TipoReserva = "Tour",
                    ItemId = 4,
                    ItemNombre = "Canopy",
                    FechaInicio = DateTime.Today.AddDays(15),
                    FechaFin = DateTime.Today.AddDays(15),
                    CantidadAdultos = 1,
                    CantidadNinos = 0,
                    CantidadSeniors = 0,
                    PrecioTotal = 90,
                    Estado = "Confirmada",
                    FechaCreacion = DateTime.Today.AddDays(-3),
                    PagoId = 3 // Tiene pago asociado
                },
                // Reserva cancelada
                new Reserva
                {
                    Id = 5,
                    UsuarioId = USER_ID_1,
                    TipoReserva = "Hospedaje",
                    ItemId = 5,
                    ItemNombre = "Hotel Cancelado",
                    FechaInicio = DateTime.Today.AddDays(20),
                    FechaFin = DateTime.Today.AddDays(22),
                    CantidadAdultos = 2,
                    CantidadNinos = 2,
                    CantidadSeniors = 0,
                    PrecioTotal = 400,
                    Estado = "Cancelada",
                    FechaCreacion = DateTime.Today.AddDays(-7),
                    PagoId = null // Sin pago
                }
            };

            _context.Reservas.AddRange(reservas);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetByUsuarioIdAsync_DebeRetornarReservasDelUsuario()
        {
            // Act
            var resultado = await _repository.GetByUsuarioIdAsync(USER_ID_1);

            // Assert
            resultado.Should().HaveCount(4); // User 1 tiene 4 reservas
            resultado.Should().OnlyContain(r => r.UsuarioId == USER_ID_1);
            resultado.Should().Contain(r => r.ItemNombre == "Hotel Arenal");
        }

        [Fact]
        public async Task GetByUsuarioIdAsync_UsuarioSinReservas_DebeRetornarVacio()
        {
            // Arrange
            string userId = "usuario_sin_reservas";

            // Act
            var resultado = await _repository.GetByUsuarioIdAsync(userId);

            // Assert
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByEstadoAsync_DebeRetornarReservasFiltradas()
        {
            // Act
            var confirmadas = await _repository.GetByEstadoAsync("Confirmada");

            // Assert
            confirmadas.Should().HaveCount(2); // Reservas 1 y 4
            confirmadas.Should().OnlyContain(r => r.Estado == "Confirmada");
        }

        [Fact]
        public async Task GetProximasAsync_DebeRetornarReservasFuturas()
        {
            // Arrange
            int dias = 7;

            // Act
            var resultado = await _repository.GetProximasAsync(USER_ID_1, dias);

            // Assert
            resultado.Should().NotBeEmpty();
            resultado.Should().OnlyContain(r => 
                r.FechaInicio >= DateTime.Today &&
                r.FechaInicio <= DateTime.Today.AddDays(dias) &&
                r.UsuarioId == USER_ID_1);
            
            // Verificar orden
            var fechas = resultado.Select(r => r.FechaInicio).ToList();
            fechas.Should().BeInAscendingOrder();
        }

        [Fact]
        public async Task GetProximasAsync_SinLimite_DebeRetornarTodasFuturas()
        {
            // Act
            var resultado = await _repository.GetProximasAsync(USER_ID_1);

            // Assert
            resultado.Should().HaveCount(2); // Reservas 1 y 2 de USER_ID_1 son futuras
            resultado.Should().OnlyContain(r => 
                r.FechaInicio >= DateTime.Today &&
                r.UsuarioId == USER_ID_1);
        }

        [Fact]
        public async Task VerificarConflictoFechasAsync_ConConflicto_DebeRetornarTrue()
        {
            // Arrange - Reserva 1: día 5 al 7
            int itemId = 1; // Mismo item
            DateTime inicio = DateTime.Today.AddDays(6); // Dentro del rango
            DateTime fin = DateTime.Today.AddDays(8);

            // Act
            var resultado = await _repository.VerificarConflictoFechasAsync(
                itemId, inicio, fin);

            // Assert
            resultado.Should().BeTrue(); // Hay conflicto con reserva 1
        }

        [Fact]
        public async Task VerificarConflictoFechasAsync_SinConflicto_DebeRetornarFalse()
        {
            // Arrange
            int itemId = 1;
            DateTime inicio = DateTime.Today.AddDays(10); // Después de reserva 1
            DateTime fin = DateTime.Today.AddDays(12);

            // Act
            var resultado = await _repository.VerificarConflictoFechasAsync(
                itemId, inicio, fin);

            // Assert
            resultado.Should().BeFalse(); // No hay conflicto
        }

        [Fact]
        public async Task VerificarConflictoFechasAsync_ExcluyendoReserva_DebeIgnorarla()
        {
            // Arrange - Mismas fechas que reserva 1
            int itemId = 1;
            DateTime inicio = DateTime.Today.AddDays(5);
            DateTime fin = DateTime.Today.AddDays(7);
            int excluirReservaId = 1; // Excluir la reserva 1

            // Act
            var resultado = await _repository.VerificarConflictoFechasAsync(
                itemId, inicio, fin, excluirReservaId);

            // Assert
            resultado.Should().BeFalse(); // No debe considerarse conflicto
        }

        [Fact]
        public async Task GetEstadisticasPorEstadoAsync_DebeRetornarConteos()
        {
            // Act
            var resultado = await _repository.GetEstadisticasPorEstadoAsync();

            // Assert
            resultado.Should().ContainKey("Confirmada");
            resultado.Should().ContainKey("Pendiente");
            resultado.Should().ContainKey("Completada");
            resultado.Should().ContainKey("Cancelada");
            
            resultado["Confirmada"].Should().Be(2);
            resultado["Pendiente"].Should().Be(1);
            resultado["Completada"].Should().Be(1);
            resultado["Cancelada"].Should().Be(1);
        }

        [Fact]
        public async Task GetSinPagarAsync_DebeRetornarSoloSinPagar()
        {
            // Act
            var resultado = await _repository.GetSinPagarAsync();

            // Assert
            resultado.Should().HaveCount(1); // Solo reserva 2 está sin pagar (5 está cancelada)
            resultado.Should().OnlyContain(r => r.EstaPagada == false);
            resultado.First().Id.Should().Be(2); // La reserva pendiente
        }

        [Fact]
        public async Task GetByTipoAsync_DebeRetornarFiltradas()
        {
            // Act
            var hospedajes = await _repository.GetByTipoAsync("Hospedaje");
            var tours = await _repository.GetByTipoAsync("Tour");

            // Assert
            hospedajes.Should().HaveCount(3);
            hospedajes.Should().OnlyContain(r => r.TipoReserva == "Hospedaje");
            
            tours.Should().HaveCount(2);
            tours.Should().OnlyContain(r => r.TipoReserva == "Tour");
        }

        [Fact]
        public async Task AddAsync_DebeAgregarReserva()
        {
            // Arrange
            var nuevaReserva = new Reserva
            {
                UsuarioId = USER_ID_1,
                TipoReserva = "Tour",
                ItemId = 10,
                ItemNombre = "Nuevo Tour",
                FechaInicio = DateTime.Today.AddDays(30),
                FechaFin = DateTime.Today.AddDays(30),
                CantidadAdultos = 2,
                PrecioTotal = 150,
                Estado = "Pendiente",
                FechaCreacion = DateTime.Today,
                PagoId = null // Sin pago
            };

            // Act
            await _repository.AddAsync(nuevaReserva);
            await _repository.SaveChangesAsync();

            // Assert
            var reservas = await _repository.GetAllAsync();
            reservas.Should().HaveCount(6);
            reservas.Should().Contain(r => r.ItemNombre == "Nuevo Tour");
        }

        [Fact]
        public async Task UpdateAsync_DebeActualizarReserva()
        {
            // Arrange
            var reserva = await _repository.GetByIdAsync(2);
            reserva!.Estado = "Confirmada";
            // Nota: EstaPagada es solo lectura, se actualiza en otra parte del sistema

            // Act
            await _repository.UpdateAsync(reserva);
            await _repository.SaveChangesAsync();

            // Assert
            var actualizada = await _repository.GetByIdAsync(2);
            actualizada!.Estado.Should().Be("Confirmada");
        }

        [Fact]
        public async Task DeleteAsync_DebeEliminarReserva()
        {
            // Arrange
            var reserva = await _repository.GetByIdAsync(5);

            // Act
            await _repository.DeleteAsync(reserva!);
            await _repository.SaveChangesAsync();

            // Assert
            var eliminada = await _repository.GetByIdAsync(5);
            eliminada.Should().BeNull();
            
            var todasReservas = await _repository.GetAllAsync();
            todasReservas.Should().HaveCount(4);
        }
    }
}
