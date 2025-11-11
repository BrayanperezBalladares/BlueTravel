using BlueTravel.Models;
using Microsoft.EntityFrameworkCore;

namespace BlueTravel.Data.Repositories
{
    /// <summary>
    /// Implementación del repositorio de Ofertas
    /// </summary>
    public class OfertaRepository : Repository<Oferta>, IOfertaRepository
    {
        public OfertaRepository(
            ApplicationDbContext context,
            ILogger<Repository<Oferta>> logger)
            : base(context, logger)
        {
        }

        public async Task<IEnumerable<Oferta>> GetActivasAsync()
        {
            try
            {
                var ahora = DateTime.Now;  // ? USAR DateTime.Now en lugar de DateTime.Today

                return await _dbSet
                    .Where(o => o.FechaInicio <= ahora && o.FechaFin >= ahora)
                    .OrderBy(o => o.Precio)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar ofertas activas");
                throw;
            }
        }

        public async Task<IEnumerable<Oferta>> GetProximasAVencerAsync(int dias = 7)
        {
            if (dias <= 0)
                throw new ArgumentException("Los días deben ser mayor a 0", nameof(dias));

            try
            {
                var ahora = DateTime.Now;  // ? USAR DateTime.Now
                var fechaLimite = ahora.AddDays(dias);

                return await _dbSet
                    .Where(o => o.FechaFin >= ahora && o.FechaFin <= fechaLimite)
                    .OrderBy(o => o.FechaFin)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar ofertas próximas a vencer en {Dias} días", dias);
                throw;
            }
        }

        public async Task<IEnumerable<Oferta>> GetByRangoPrecioAsync(decimal precioMin, decimal precioMax)
        {
            if (precioMin < 0 || precioMax < 0)
                throw new ArgumentException("Los precios no pueden ser negativos");

            if (precioMin > precioMax)
                throw new ArgumentException("El precio mínimo no puede ser mayor que el máximo");

            try
            {
                var ahora = DateTime.Now;  // ? USAR DateTime.Now

                return await _dbSet
                    .Where(o => o.Precio >= precioMin 
                             && o.Precio <= precioMax
                             && o.FechaInicio <= ahora 
                             && o.FechaFin >= ahora)
                    .OrderBy(o => o.Precio)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar ofertas por rango de precio: {Min} - {Max}", 
                    precioMin, precioMax);
                throw;
            }
        }

        public async Task<bool> EstaVigenteAsync(int ofertaId)
        {
            try
            {
                var oferta = await GetByIdAsync(ofertaId);
                if (oferta == null)
                {
                    _logger.LogWarning("Oferta {OfertaId} no encontrada", ofertaId);
                    return false;
                }

                var ahora = DateTime.Now;  // ? USAR DateTime.Now
                return oferta.FechaInicio <= ahora && oferta.FechaFin >= ahora;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar vigencia de oferta {OfertaId}", ofertaId);
                throw;
            }
        }

        public async Task<IEnumerable<Oferta>> GetMejoresOfertasAsync(int cantidad = 10)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a 0", nameof(cantidad));

            try
            {
                var ahora = DateTime.Now;  // ? USAR DateTime.Now

                return await _dbSet
                    .Where(o => o.FechaInicio <= ahora && o.FechaFin >= ahora)
                    .OrderBy(o => o.Precio)
                    .Take(cantidad)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar las {Cantidad} mejores ofertas", cantidad);
                throw;
            }
        }
    }
}
