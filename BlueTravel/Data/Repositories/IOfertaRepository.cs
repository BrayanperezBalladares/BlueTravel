using BlueTravel.Models;

namespace BlueTravel.Data.Repositories
{
    /// <summary>
    /// Interfaz específica para operaciones de Ofertas
    /// </summary>
    public interface IOfertaRepository : IRepository<Oferta>
    {
        /// <summary>
        /// Obtiene ofertas activas (vigentes)
        /// </summary>
        Task<IEnumerable<Oferta>> GetActivasAsync();

        /// <summary>
        /// Obtiene ofertas próximas a vencer
        /// </summary>
        Task<IEnumerable<Oferta>> GetProximasAVencerAsync(int dias = 7);

        /// <summary>
        /// Obtiene ofertas por rango de precio
        /// </summary>
        Task<IEnumerable<Oferta>> GetByRangoPrecioAsync(decimal precioMin, decimal precioMax);

        /// <summary>
        /// Verifica si una oferta está vigente
        /// </summary>
        Task<bool> EstaVigenteAsync(int ofertaId);

        /// <summary>
        /// Obtiene las mejores ofertas (más baratas)
        /// </summary>
        Task<IEnumerable<Oferta>> GetMejoresOfertasAsync(int cantidad = 10);
    }
}
