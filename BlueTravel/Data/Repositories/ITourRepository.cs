using BlueTravel.Models;

namespace BlueTravel.Data.Repositories
{
    /// <summary>
    /// Interfaz específica para operaciones de Tours
    /// </summary>
    public interface ITourRepository : IRepository<Tour>
    {
        /// <summary>
        /// Obtiene tours por ubicación
        /// </summary>
        Task<IEnumerable<Tour>> GetByUbicacionAsync(string ubicacion);

        /// <summary>
        /// Obtiene tours disponibles (con cupos)
        /// </summary>
        Task<IEnumerable<Tour>> GetDisponiblesAsync();

        /// <summary>
        /// Obtiene tours por nivel de dificultad
        /// </summary>
        Task<IEnumerable<Tour>> GetByNivelDificultadAsync(string nivelDificultad);

        /// <summary>
        /// Obtiene tours próximos (fecha futura)
        /// </summary>
        Task<IEnumerable<Tour>> GetProximosAsync(int dias = 30);

        /// <summary>
        /// Obtiene tours con descuento de grupo
        /// </summary>
        Task<IEnumerable<Tour>> GetConDescuentoGrupoAsync();

        /// <summary>
        /// Obtiene tours por rango de precio
        /// </summary>
        Task<IEnumerable<Tour>> GetByRangoPrecioAsync(decimal precioMin, decimal precioMax);

        /// <summary>
        /// Verifica si hay cupos disponibles
        /// </summary>
        Task<bool> TieneCuposDisponiblesAsync(int tourId, int cantidadPersonas);

        /// <summary>
        /// Reserva cupos para un tour
        /// </summary>
        Task<bool> ReservarCuposAsync(int tourId, int cantidadPersonas);

        /// <summary>
        /// Libera cupos de un tour
        /// </summary>
        Task<bool> LiberarCuposAsync(int tourId, int cantidadPersonas);
    }
}
