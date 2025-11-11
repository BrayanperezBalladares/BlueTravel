namespace BlueTravel.Services
{
    /// <summary>
    /// Interfaz para servicios de dashboard administrativo
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// Obtiene todas las estadísticas del dashboard
        /// </summary>
        Task<Models.DashboardStats> GetStatsAsync();
        
        /// <summary>
        /// Obtiene estadísticas de un rango de fechas específico
        /// </summary>
        Task<Models.DashboardStats> GetStatsByDateRangeAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}
