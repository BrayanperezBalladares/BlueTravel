using BlueTravel.Data.Repositories;
using BlueTravel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BlueTravel.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly IHospedajeRepository _hospedajeRepository;
        private readonly ITourRepository _tourRepository;
        private readonly ICacheService _cacheService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            IReservaRepository reservaRepository,
            IHospedajeRepository hospedajeRepository,
            ITourRepository tourRepository,
            ICacheService cacheService,
            UserManager<IdentityUser> userManager,
            ILogger<DashboardService> logger)
        {
            _reservaRepository = reservaRepository;
            _hospedajeRepository = hospedajeRepository;
            _tourRepository = tourRepository;
            _cacheService = cacheService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Models.DashboardStats> GetStatsAsync()
        {
            return await _cacheService.GetOrCreateAsync(
                "dashboard_stats",
                async () => await CalcularEstadisticasAsync(),
                TimeSpan.FromMinutes(10)
            );
        }

        public async Task<Models.DashboardStats> GetStatsByDateRangeAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var cacheKey = $"dashboard_stats_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}";
            
            return await _cacheService.GetOrCreateAsync(
                cacheKey,
                async () => await CalcularEstadisticasAsync(fechaInicio, fechaFin),
                TimeSpan.FromMinutes(30)
            );
        }

        private async Task<Models.DashboardStats> CalcularEstadisticasAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            try
            {
                var stats = new Models.DashboardStats();

                // Obtener todas las reservas
                var reservas = await _reservaRepository.GetAllAsync();

                // Filtrar por rango de fechas si se proporciona
                if (fechaInicio.HasValue && fechaFin.HasValue)
                {
                    reservas = reservas.Where(r => 
                        r.FechaCreacion >= fechaInicio.Value && 
                        r.FechaCreacion <= fechaFin.Value).ToList();
                }

                // ===== MÉTRICAS GENERALES =====
                stats.TotalReservas = reservas.Count();
                stats.ReservasActivas = reservas.Count(r => r.Estado == "Confirmada");
                stats.ReservasPendientes = reservas.Count(r => r.Estado == "Pendiente");
                stats.ReservasCanceladas = reservas.Count(r => r.Estado == "Cancelada");
                stats.ReservasCompletadas = reservas.Count(r => r.Estado == "Completada");

                // ===== MÉTRICAS FINANCIERAS =====
                var reservasPagadas = reservas.Where(r => r.EstaPagada).ToList();
                stats.IngresosTotales = reservasPagadas.Sum(r => r.PrecioTotal);
                stats.PromedioReserva = reservasPagadas.Any() 
                    ? reservasPagadas.Average(r => r.PrecioTotal) 
                    : 0;

                var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                stats.IngresosMesActual = reservasPagadas
                    .Where(r => r.FechaCreacion >= inicioMes)
                    .Sum(r => r.PrecioTotal);

                var hace30Dias = DateTime.Now.AddDays(-30);
                stats.IngresosUltimos30Dias = reservasPagadas
                    .Where(r => r.FechaCreacion >= hace30Dias)
                    .Sum(r => r.PrecioTotal);

                // ===== MÉTRICAS DE USUARIO =====
                stats.UsuariosTotales = await _userManager.Users.CountAsync();
                
                // ? CORRECCIÓN: Traer IDs de usuarios con reservas primero
                var usuariosConReservasIds = reservas.Select(r => r.UsuarioId).Distinct().ToList();
                stats.UsuariosActivos = usuariosConReservasIds.Count;

                // ===== RESERVAS POR TIPO =====
                stats.ReservasHospedaje = reservas.Count(r => r.TipoReserva == "Hospedaje");
                stats.ReservasTour = reservas.Count(r => r.TipoReserva == "Tour");
                stats.ReservasOferta = reservas.Count(r => r.TipoReserva == "Oferta");

                // ===== TENDENCIAS =====
                var mesAnterior = DateTime.Now.AddMonths(-1);
                var inicioMesAnterior = new DateTime(mesAnterior.Year, mesAnterior.Month, 1);
                var finMesAnterior = inicioMesAnterior.AddMonths(1).AddDays(-1);

                var reservasMesAnterior = reservas.Count(r => 
                    r.FechaCreacion >= inicioMesAnterior && 
                    r.FechaCreacion <= finMesAnterior);

                var reservasMesActual = reservas.Count(r => r.FechaCreacion >= inicioMes);

                if (reservasMesAnterior > 0)
                {
                    stats.CrecimientoMensual = ((reservasMesActual - reservasMesAnterior) / (decimal)reservasMesAnterior) * 100;
                }

                stats.TasaCancelacion = stats.TotalReservas > 0
                    ? (stats.ReservasCanceladas / (decimal)stats.TotalReservas) * 100
                    : 0;

                stats.TasaConversion = stats.TotalReservas > 0
                    ? (reservasPagadas.Count / (decimal)stats.TotalReservas) * 100
                    : 0;

                // ===== TOP HOSPEDAJES =====
                var reservasHospedaje = reservas.Where(r => r.TipoReserva == "Hospedaje").ToList();
                stats.TopHospedajes = reservasHospedaje
                    .GroupBy(r => new { r.ItemId, r.ItemNombre })
                    .Select(g => new TopItemStats
                    {
                        Id = g.Key.ItemId,
                        Nombre = g.Key.ItemNombre,
                        CantidadReservas = g.Count(),
                        IngresoTotal = g.Where(r => r.EstaPagada).Sum(r => r.PrecioTotal),
                        PrecioPromedio = g.Average(r => r.PrecioTotal)
                    })
                    .OrderByDescending(t => t.CantidadReservas)
                    .Take(5)
                    .ToList();

                // Completar ubicaciones de hospedajes
                foreach (var top in stats.TopHospedajes)
                {
                    var hospedaje = await _hospedajeRepository.GetByIdAsync(top.Id);
                    if (hospedaje != null)
                    {
                        top.Ubicacion = hospedaje.Ubicacion;
                    }
                }

                // ===== TOP TOURS =====
                var reservasTour = reservas.Where(r => r.TipoReserva == "Tour").ToList();
                stats.TopTours = reservasTour
                    .GroupBy(r => new { r.ItemId, r.ItemNombre })
                    .Select(g => new TopItemStats
                    {
                        Id = g.Key.ItemId,
                        Nombre = g.Key.ItemNombre,
                        CantidadReservas = g.Count(),
                        IngresoTotal = g.Where(r => r.EstaPagada).Sum(r => r.PrecioTotal),
                        PrecioPromedio = g.Average(r => r.PrecioTotal)
                    })
                    .OrderByDescending(t => t.CantidadReservas)
                    .Take(5)
                    .ToList();

                // Completar ubicaciones de tours
                foreach (var top in stats.TopTours)
                {
                    var tour = await _tourRepository.GetByIdAsync(top.Id);
                    if (tour != null)
                    {
                        top.Ubicacion = tour.Ubicacion;
                    }
                }

                // ===== RESERVAS POR MES (Últimos 6 meses) =====
                var culture = new CultureInfo("es-ES");
                stats.ReservasPorMes = Enumerable.Range(0, 6)
                    .Select(i =>
                    {
                        var mes = DateTime.Now.AddMonths(-5 + i);
                        var inicioMesTemp = new DateTime(mes.Year, mes.Month, 1);
                        var finMesTemp = inicioMesTemp.AddMonths(1).AddDays(-1);

                        var reservasMes = reservas.Where(r =>
                            r.FechaCreacion >= inicioMesTemp &&
                            r.FechaCreacion <= finMesTemp).ToList();

                        return new MonthlyStats
                        {
                            Mes = culture.DateTimeFormat.GetMonthName(mes.Month) + " " + mes.Year,
                            Cantidad = reservasMes.Count,
                            Monto = reservasMes.Where(r => r.EstaPagada).Sum(r => r.PrecioTotal)
                        };
                    })
                    .ToList();

                stats.IngresosPorMes = stats.ReservasPorMes; // Mismos datos

                // ===== RESERVAS POR ESTADO =====
                stats.ReservasPorEstado = reservas
                    .GroupBy(r => r.Estado)
                    .ToDictionary(g => g.Key, g => g.Count());

                // ===== RESERVAS POR TIPO =====
                stats.ReservasPorTipo = reservas
                    .GroupBy(r => r.TipoReserva)
                    .ToDictionary(g => g.Key, g => g.Count());

                // ===== ACTIVIDAD RECIENTE =====
                stats.UltimasReservas = reservas
                    .OrderByDescending(r => r.FechaCreacion)
                    .Take(10)
                    .Select(r => new ReservaReciente
                    {
                        Id = r.Id,
                        Usuario = r.UsuarioId,
                        Item = r.ItemNombre,
                        Tipo = r.TipoReserva,
                        Total = r.PrecioTotal,
                        Fecha = r.FechaCreacion,
                        Estado = r.Estado
                    })
                    .ToList();

                _logger.LogInformation("Estadísticas del dashboard calculadas: {TotalReservas} reservas", stats.TotalReservas);

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calcular estadísticas del dashboard");
                throw;
            }
        }
    }
}
