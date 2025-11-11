using BlueTravel.Models;

namespace BlueTravel.Models
{
    /// <summary>
    /// Modelo para estadísticas del dashboard administrativo
    /// </summary>
    public class DashboardStats
    {
        // Métricas Generales
        public int TotalReservas { get; set; }
        public int ReservasActivas { get; set; }
        public int ReservasPendientes { get; set; }
        public int ReservasCanceladas { get; set; }
        public int ReservasCompletadas { get; set; }
        
        // Métricas Financieras
        public decimal IngresosTotales { get; set; }
        public decimal IngresosMesActual { get; set; }
        public decimal IngresosUltimos30Dias { get; set; }
        public decimal PromedioReserva { get; set; }
        
        // Métricas de Usuario
        public int UsuariosTotales { get; set; }
        public int UsuariosActivos { get; set; }
        
        // Reservas por Tipo
        public int ReservasHospedaje { get; set; }
        public int ReservasTour { get; set; }
        public int ReservasOferta { get; set; }
        
        // Tendencias
        public decimal CrecimientoMensual { get; set; }
        public decimal TasaCancelacion { get; set; }
        public decimal TasaConversion { get; set; }
        
        // Top Items
        public List<TopItemStats> TopHospedajes { get; set; } = new();
        public List<TopItemStats> TopTours { get; set; } = new();
        
        // Datos para Gráficas
        public List<MonthlyStats> ReservasPorMes { get; set; } = new();
        public List<MonthlyStats> IngresosPorMes { get; set; } = new();
        public Dictionary<string, int> ReservasPorEstado { get; set; } = new();
        public Dictionary<string, int> ReservasPorTipo { get; set; } = new();
        
        // Actividad Reciente
        public List<ReservaReciente> UltimasReservas { get; set; } = new();
    }
    
    /// <summary>
    /// Estadísticas de items más populares
    /// </summary>
    public class TopItemStats
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Ubicacion { get; set; } = string.Empty;
        public int CantidadReservas { get; set; }
        public decimal IngresoTotal { get; set; }
        public decimal PrecioPromedio { get; set; }
    }
    
    /// <summary>
    /// Estadísticas mensuales
    /// </summary>
    public class MonthlyStats
    {
        public string Mes { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Monto { get; set; }
    }
    
    /// <summary>
    /// Reservas recientes para actividad
    /// </summary>
    public class ReservaReciente
    {
        public int Id { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Item { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
