using BlueTravel.Models;
using Microsoft.EntityFrameworkCore;

namespace BlueTravel.Data.Repositories
{
    /// <summary>
    /// Implementación del repositorio de Hospedajes
    /// Proporciona acceso a datos específico para hospedajes
    /// </summary>
    public class HospedajeRepository : Repository<Hospedaje>, IHospedajeRepository
    {
        public HospedajeRepository(
            ApplicationDbContext context,
            ILogger<Repository<Hospedaje>> logger)
            : base(context, logger)
        {
        }

        public async Task<IEnumerable<Hospedaje>> GetByUbicacionAsync(string ubicacion)
        {
            if (string.IsNullOrWhiteSpace(ubicacion))
                throw new ArgumentException("La ubicación no puede estar vacía", nameof(ubicacion));

            try
            {
                return await _dbSet
                    .Where(h => h.Ubicacion.Contains(ubicacion))
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar hospedajes por ubicación: {Ubicacion}", ubicacion);
                throw;
            }
        }

        public async Task<IEnumerable<Hospedaje>> GetDisponiblesAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                // Obtener IDs de hospedajes reservados en esas fechas
                var hospedajesReservados = await _context.Reservas
                    .Where(r => r.TipoReserva == "Hospedaje"
                             && r.Estado != "Cancelada"
                             && r.FechaInicio < fechaFin
                             && r.FechaFin > fechaInicio)
                    .Select(r => r.ItemId)
                    .Distinct()
                    .ToListAsync();

                // Obtener hospedajes que NO están en la lista de reservados
                return await _dbSet
                    .Where(h => !hospedajesReservados.Contains(h.Id))
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar hospedajes disponibles del {FechaInicio} al {FechaFin}", 
                    fechaInicio, fechaFin);
                throw;
            }
        }

        public async Task<IEnumerable<Hospedaje>> GetByRangoPrecioAsync(decimal precioMin, decimal precioMax)
        {
            if (precioMin < 0 || precioMax < 0)
                throw new ArgumentException("Los precios no pueden ser negativos");

            if (precioMin > precioMax)
                throw new ArgumentException("El precio mínimo no puede ser mayor que el máximo");

            try
            {
                return await _dbSet
                    .Where(h => h.PrecioPorNoche >= precioMin && h.PrecioPorNoche <= precioMax)
                    .OrderBy(h => h.PrecioPorNoche)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar hospedajes por rango de precio: {Min} - {Max}", 
                    precioMin, precioMax);
                throw;
            }
        }

        public async Task<IEnumerable<Hospedaje>> GetQuePermitenNinosAsync()
        {
            try
            {
                return await _dbSet
                    .Where(h => h.PermiteNinos)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar hospedajes que permiten niños");
                throw;
            }
        }

        public async Task<IEnumerable<Hospedaje>> GetQuePermitenMascotasAsync()
        {
            try
            {
                return await _dbSet
                    .Where(h => h.PermiteMascotas)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar hospedajes que permiten mascotas");
                throw;
            }
        }

        public async Task<IEnumerable<Hospedaje>> GetConCapacidadMinimaAsync(int personas)
        {
            if (personas <= 0)
                throw new ArgumentException("La cantidad de personas debe ser mayor a 0", nameof(personas));

            try
            {
                return await _dbSet
                    .Where(h => h.CapacidadMaxima >= personas)
                    .OrderBy(h => h.PrecioPorNoche)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar hospedajes con capacidad mínima de {Personas}", personas);
                throw;
            }
        }

        public async Task<bool> VerificarDisponibilidadAsync(int hospedajeId, DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                // Verificar que el hospedaje existe
                var hospedaje = await GetByIdAsync(hospedajeId);
                if (hospedaje == null)
                {
                    _logger.LogWarning("Hospedaje {HospedajeId} no encontrado", hospedajeId);
                    return false;
                }

                // Buscar conflictos de fechas
                var hayConflicto = await _context.Reservas
                    .AnyAsync(r => r.ItemId == hospedajeId
                                && r.TipoReserva == "Hospedaje"
                                && r.Estado != "Cancelada"
                                && r.FechaInicio < fechaFin
                                && r.FechaFin > fechaInicio);

                return !hayConflicto; // Retorna true si NO hay conflicto (está disponible)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar disponibilidad del hospedaje {HospedajeId}", hospedajeId);
                throw;
            }
        }
    }
}
