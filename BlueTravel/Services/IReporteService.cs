using BlueTravel.Data;
using BlueTravel.Models;
using Microsoft.EntityFrameworkCore;

namespace BlueTravel.Services
{
    public interface IReporteService
    {
        Task<DashboardStats> ObtenerEstadisticasDashboard();
        Task<List<ReservaPorMes>> ObtenerReservasPorMes(int anio);
        Task<List<IngresoPorCategoria>> ObtenerIngresosPorCategoria(DateTime desde, DateTime hasta);
        Task<List<TopDestino>> ObtenerTopDestinos(int limite = 10);
    }

    public class ReporteService : IReporteService
    {
        private readonly ApplicationDbContext _context;

        public ReporteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStats> ObtenerEstadisticasDashboard()
        {
            var hoy = DateTime.Today;
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
            var finMes = inicioMes.AddMonths(1).AddDays(-1);

            return new DashboardStats
            {
                TotalReservasHoy = await _context.Reservas
                    .CountAsync(r => r.FechaCreacion.Date == hoy),

                TotalReservasMes = await _context.Reservas
                    .CountAsync(r => r.FechaCreacion >= inicioMes && r.FechaCreacion <= finMes),

                ReservasPendientes = await _context.Reservas
                    .CountAsync(r => r.Estado == "Pendiente"),

                IngresosMes = await _context.Reservas
                    .Where(r => r.FechaCreacion >= inicioMes && r.FechaCreacion <= finMes && r.Estado != "Cancelada")
                    .SumAsync(r => r.PrecioTotal),

                TasaOcupacionHospedajes = await CalcularTasaOcupacion(),

                ProximasReservas = await _context.Reservas
                    .Where(r => r.FechaInicio >= hoy && r.FechaInicio <= hoy.AddDays(7))
                    .OrderBy(r => r.FechaInicio)
                    .Take(5)
                    .ToListAsync()
            };
        }

        public async Task<List<ReservaPorMes>> ObtenerReservasPorMes(int anio)
        {
            return await _context.Reservas
                .Where(r => r.FechaCreacion.Year == anio)
                .GroupBy(r => r.FechaCreacion.Month)
                .Select(g => new ReservaPorMes
                {
                    Mes = g.Key,
                    NombreMes = new DateTime(anio, g.Key, 1).ToString("MMMM"),
                    TotalReservas = g.Count(),
                    TotalIngresos = g.Where(r => r.Estado != "Cancelada").Sum(r => r.PrecioTotal)
                })
                .OrderBy(r => r.Mes)
                .ToListAsync();
        }

        public async Task<List<IngresoPorCategoria>> ObtenerIngresosPorCategoria(DateTime desde, DateTime hasta)
        {
            return await _context.Reservas
                .Where(r => r.FechaCreacion >= desde && r.FechaCreacion <= hasta && r.Estado != "Cancelada")
                .GroupBy(r => r.TipoReserva)
                .Select(g => new IngresoPorCategoria
                {
                    Categoria = g.Key,
                    TotalIngresos = g.Sum(r => r.PrecioTotal),
                    CantidadReservas = g.Count()
                })
                .ToListAsync();
        }

        public async Task<List<TopDestino>> ObtenerTopDestinos(int limite = 10)
        {
            return await _context.Reservas
                .Where(r => r.Estado != "Cancelada")
                .GroupBy(r => r.ItemNombre)
                .Select(g => new TopDestino
                {
                    Nombre = g.Key,
                    TotalReservas = g.Count(),
                    TotalIngresos = g.Sum(r => r.PrecioTotal)
                })
                .OrderByDescending(d => d.TotalReservas)
                .Take(limite)
                .ToListAsync();
        }

        private async Task<decimal> CalcularTasaOcupacion()
        {
            var totalHospedajes = await _context.Hospedajes.CountAsync();
            if (totalHospedajes == 0) return 0;

            var hospedajesOcupados = await _context.Reservas
                .Where(r => r.TipoReserva == "Hospedaje" 
                         && r.Estado == "Confirmada"
                         && r.FechaInicio <= DateTime.Today
                         && r.FechaFin >= DateTime.Today)
                .Select(r => r.ItemId)
                .Distinct()
                .CountAsync();

            return (decimal)hospedajesOcupados / totalHospedajes * 100;
        }
    }

    // DTOs para reportes
    public class DashboardStats
    {
        public int TotalReservasHoy { get; set; }
        public int TotalReservasMes { get; set; }
        public int ReservasPendientes { get; set; }
        public decimal IngresosMes { get; set; }
        public decimal TasaOcupacionHospedajes { get; set; }
        public List<Reserva> ProximasReservas { get; set; } = new();
    }

    public class ReservaPorMes
    {
        public int Mes { get; set; }
        public string NombreMes { get; set; } = string.Empty;
        public int TotalReservas { get; set; }
        public decimal TotalIngresos { get; set; }
    }

    public class IngresoPorCategoria
    {
        public string Categoria { get; set; } = string.Empty;
        public decimal TotalIngresos { get; set; }
        public int CantidadReservas { get; set; }
    }

    public class TopDestino
    {
        public string Nombre { get; set; } = string.Empty;
        public int TotalReservas { get; set; }
        public decimal TotalIngresos { get; set; }
    }
}
