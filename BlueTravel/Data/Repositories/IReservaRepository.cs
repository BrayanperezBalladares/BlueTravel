using BlueTravel.Models;

namespace BlueTravel.Data.Repositories
{
    /// <summary>
    /// Interfaz específica para operaciones de Reservas
    /// </summary>
    public interface IReservaRepository : IRepository<Reserva>
    {
        /// <summary>
        /// Obtiene todas las reservas de un usuario
        /// </summary>
        Task<IEnumerable<Reserva>> GetByUsuarioIdAsync(string usuarioId);

        /// <summary>
        /// Obtiene reservas por estado
        /// </summary>
        Task<IEnumerable<Reserva>> GetByEstadoAsync(string estado);

        /// <summary>
        /// Obtiene reservas próximas de un usuario
        /// </summary>
        Task<IEnumerable<Reserva>> GetProximasAsync(string usuarioId, int dias = 30);

        /// <summary>
        /// Obtiene reservas por tipo (Hospedaje, Tour, Oferta)
        /// </summary>
        Task<IEnumerable<Reserva>> GetByTipoAsync(string tipoReserva);

        /// <summary>
        /// Obtiene reservas por rango de fechas
        /// </summary>
        Task<IEnumerable<Reserva>> GetByRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin);

        /// <summary>
        /// Obtiene reservas pendientes de confirmación
        /// </summary>
        Task<IEnumerable<Reserva>> GetPendientesConfirmacionAsync();

        /// <summary>
        /// Obtiene reservas sin pagar
        /// </summary>
        Task<IEnumerable<Reserva>> GetSinPagarAsync();

        /// <summary>
        /// Obtiene reservas de un item específico
        /// </summary>
        Task<IEnumerable<Reserva>> GetByItemAsync(string tipoReserva, int itemId);

        /// <summary>
        /// Verifica conflicto de fechas para un hospedaje
        /// </summary>
        Task<bool> VerificarConflictoFechasAsync(int itemId, DateTime fechaInicio, DateTime fechaFin, int? excluirReservaId = null);

        /// <summary>
        /// Obtiene estadísticas de reservas
        /// </summary>
        Task<Dictionary<string, int>> GetEstadisticasPorEstadoAsync();
    }
}
