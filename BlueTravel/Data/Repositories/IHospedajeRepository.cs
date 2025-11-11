using BlueTravel.Models;

namespace BlueTravel.Data.Repositories
{
    /// <summary>
    /// Interfaz específica para operaciones de Hospedajes
    /// Extiende el repositorio genérico con métodos especializados
    /// </summary>
    public interface IHospedajeRepository : IRepository<Hospedaje>
    {
        /// <summary>
        /// Obtiene hospedajes por ubicación
        /// </summary>
        Task<IEnumerable<Hospedaje>> GetByUbicacionAsync(string ubicacion);

        /// <summary>
        /// Obtiene hospedajes disponibles para un rango de fechas
        /// </summary>
        Task<IEnumerable<Hospedaje>> GetDisponiblesAsync(DateTime fechaInicio, DateTime fechaFin);

        /// <summary>
        /// Obtiene hospedajes por rango de precio
        /// </summary>
        Task<IEnumerable<Hospedaje>> GetByRangoPrecioAsync(decimal precioMin, decimal precioMax);

        /// <summary>
        /// Obtiene hospedajes que permiten niños
        /// </summary>
        Task<IEnumerable<Hospedaje>> GetQuePermitenNinosAsync();

        /// <summary>
        /// Obtiene hospedajes que permiten mascotas
        /// </summary>
        Task<IEnumerable<Hospedaje>> GetQuePermitenMascotasAsync();

        /// <summary>
        /// Obtiene hospedajes con capacidad mínima
        /// </summary>
        Task<IEnumerable<Hospedaje>> GetConCapacidadMinimaAsync(int personas);

        /// <summary>
        /// Verifica disponibilidad de un hospedaje para fechas específicas
        /// </summary>
        Task<bool> VerificarDisponibilidadAsync(int hospedajeId, DateTime fechaInicio, DateTime fechaFin);
    }
}
