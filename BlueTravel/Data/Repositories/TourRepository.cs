using BlueTravel.Models;
using Microsoft.EntityFrameworkCore;

namespace BlueTravel.Data.Repositories
{
    /// <summary>
    /// Implementación del repositorio de Tours
    /// </summary>
    public class TourRepository : Repository<Tour>, ITourRepository
    {
        public TourRepository(
            ApplicationDbContext context,
            ILogger<Repository<Tour>> logger)
            : base(context, logger)
        {
        }

        public async Task<IEnumerable<Tour>> GetByUbicacionAsync(string ubicacion)
        {
            if (string.IsNullOrWhiteSpace(ubicacion))
                throw new ArgumentException("La ubicación no puede estar vacía", nameof(ubicacion));

            try
            {
                return await _dbSet
                    .Where(t => t.Ubicacion.Contains(ubicacion))
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar tours por ubicación: {Ubicacion}", ubicacion);
                throw;
            }
        }

        public async Task<IEnumerable<Tour>> GetDisponiblesAsync()
        {
            try
            {
                return await _dbSet
                    .Where(t => t.CuposReservados < t.CuposTotales
                             && t.FechaDisponible >= DateTime.Today)
                    .OrderBy(t => t.FechaDisponible)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar tours disponibles");
                throw;
            }
        }

        public async Task<IEnumerable<Tour>> GetByNivelDificultadAsync(string nivelDificultad)
        {
            if (string.IsNullOrWhiteSpace(nivelDificultad))
                throw new ArgumentException("El nivel de dificultad no puede estar vacío", nameof(nivelDificultad));

            try
            {
                return await _dbSet
                    .Where(t => t.NivelDificultad == nivelDificultad)
                    .OrderBy(t => t.FechaDisponible)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar tours por nivel de dificultad: {Nivel}", nivelDificultad);
                throw;
            }
        }

        public async Task<IEnumerable<Tour>> GetProximosAsync(int dias = 30)
        {
            if (dias <= 0)
                throw new ArgumentException("Los días deben ser mayor a 0", nameof(dias));

            try
            {
                var fechaLimite = DateTime.Today.AddDays(dias);

                return await _dbSet
                    .Where(t => t.FechaDisponible >= DateTime.Today 
                             && t.FechaDisponible <= fechaLimite
                             && t.CuposReservados < t.CuposTotales)
                    .OrderBy(t => t.FechaDisponible)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar tours próximos ({Dias} días)", dias);
                throw;
            }
        }

        public async Task<IEnumerable<Tour>> GetConDescuentoGrupoAsync()
        {
            try
            {
                return await _dbSet
                    .Where(t => t.DescuentoGrupo > 0)
                    .OrderByDescending(t => t.DescuentoGrupo)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar tours con descuento de grupo");
                throw;
            }
        }

        public async Task<IEnumerable<Tour>> GetByRangoPrecioAsync(decimal precioMin, decimal precioMax)
        {
            if (precioMin < 0 || precioMax < 0)
                throw new ArgumentException("Los precios no pueden ser negativos");

            if (precioMin > precioMax)
                throw new ArgumentException("El precio mínimo no puede ser mayor que el máximo");

            try
            {
                return await _dbSet
                    .Where(t => t.Precio >= precioMin && t.Precio <= precioMax)
                    .OrderBy(t => t.Precio)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar tours por rango de precio: {Min} - {Max}", 
                    precioMin, precioMax);
                throw;
            }
        }

        public async Task<bool> TieneCuposDisponiblesAsync(int tourId, int cantidadPersonas)
        {
            if (cantidadPersonas <= 0)
                throw new ArgumentException("La cantidad de personas debe ser mayor a 0", nameof(cantidadPersonas));

            try
            {
                var tour = await GetByIdAsync(tourId);
                if (tour == null)
                {
                    _logger.LogWarning("Tour {TourId} no encontrado", tourId);
                    return false;
                }

                var cuposDisponibles = tour.CuposTotales - tour.CuposReservados;
                return cuposDisponibles >= cantidadPersonas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar cupos disponibles para tour {TourId}", tourId);
                throw;
            }
        }

        public async Task<bool> ReservarCuposAsync(int tourId, int cantidadPersonas)
        {
            if (cantidadPersonas <= 0)
                throw new ArgumentException("La cantidad de personas debe ser mayor a 0", nameof(cantidadPersonas));

            try
            {
                var tour = await _dbSet.FindAsync(tourId);
                if (tour == null)
                {
                    _logger.LogWarning("Tour {TourId} no encontrado para reservar cupos", tourId);
                    return false;
                }

                var cuposDisponibles = tour.CuposTotales - tour.CuposReservados;
                if (cuposDisponibles < cantidadPersonas)
                {
                    _logger.LogWarning("No hay suficientes cupos disponibles en tour {TourId}. Disponibles: {Disponibles}, Solicitados: {Solicitados}",
                        tourId, cuposDisponibles, cantidadPersonas);
                    return false;
                }

                tour.CuposReservados += cantidadPersonas;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Reservados {Cantidad} cupos para tour {TourId}. Total reservados: {Total}",
                    cantidadPersonas, tourId, tour.CuposReservados);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reservar cupos para tour {TourId}", tourId);
                throw;
            }
        }

        public async Task<bool> LiberarCuposAsync(int tourId, int cantidadPersonas)
        {
            if (cantidadPersonas <= 0)
                throw new ArgumentException("La cantidad de personas debe ser mayor a 0", nameof(cantidadPersonas));

            try
            {
                var tour = await _dbSet.FindAsync(tourId);
                if (tour == null)
                {
                    _logger.LogWarning("Tour {TourId} no encontrado para liberar cupos", tourId);
                    return false;
                }

                tour.CuposReservados = Math.Max(0, tour.CuposReservados - cantidadPersonas);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Liberados {Cantidad} cupos para tour {TourId}. Total reservados: {Total}",
                    cantidadPersonas, tourId, tour.CuposReservados);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al liberar cupos para tour {TourId}", tourId);
                throw;
            }
        }
    }
}
