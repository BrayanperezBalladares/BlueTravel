using BlueTravel.Models;
using Microsoft.EntityFrameworkCore;

namespace BlueTravel.Data.Repositories
{
    /// <summary>
    /// Implementación del repositorio de Reservas
    /// </summary>
    public class ReservaRepository : Repository<Reserva>, IReservaRepository
    {
        public ReservaRepository(
            ApplicationDbContext context,
            ILogger<Repository<Reserva>> logger)
            : base(context, logger)
        {
        }

        public async Task<IEnumerable<Reserva>> GetByUsuarioIdAsync(string usuarioId)
        {
            if (string.IsNullOrWhiteSpace(usuarioId))
                throw new ArgumentException("El ID de usuario no puede estar vacío", nameof(usuarioId));

            try
            {
                return await _dbSet
                    .Where(r => r.UsuarioId == usuarioId)
                    .OrderByDescending(r => r.FechaCreacion)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar reservas del usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task<IEnumerable<Reserva>> GetByEstadoAsync(string estado)
        {
            if (string.IsNullOrWhiteSpace(estado))
                throw new ArgumentException("El estado no puede estar vacío", nameof(estado));

            try
            {
                return await _dbSet
                    .Where(r => r.Estado == estado)
                    .OrderByDescending(r => r.FechaCreacion)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar reservas con estado {Estado}", estado);
                throw;
            }
        }

        public async Task<IEnumerable<Reserva>> GetProximasAsync(string usuarioId, int dias = 30)
        {
            if (string.IsNullOrWhiteSpace(usuarioId))
                throw new ArgumentException("El ID de usuario no puede estar vacío", nameof(usuarioId));

            if (dias <= 0)
                throw new ArgumentException("Los días deben ser mayor a 0", nameof(dias));

            try
            {
                var fechaLimite = DateTime.Today.AddDays(dias);

                return await _dbSet
                    .Where(r => r.UsuarioId == usuarioId
                             && r.FechaInicio >= DateTime.Today
                             && r.FechaInicio <= fechaLimite
                             && r.Estado != "Cancelada")
                    .OrderBy(r => r.FechaInicio)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar reservas próximas del usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task<IEnumerable<Reserva>> GetByTipoAsync(string tipoReserva)
        {
            if (string.IsNullOrWhiteSpace(tipoReserva))
                throw new ArgumentException("El tipo de reserva no puede estar vacío", nameof(tipoReserva));

            try
            {
                return await _dbSet
                    .Where(r => r.TipoReserva == tipoReserva)
                    .OrderByDescending(r => r.FechaCreacion)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar reservas de tipo {Tipo}", tipoReserva);
                throw;
            }
        }

        public async Task<IEnumerable<Reserva>> GetByRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                return await _dbSet
                    .Where(r => r.FechaInicio >= fechaInicio && r.FechaFin <= fechaFin)
                    .OrderBy(r => r.FechaInicio)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar reservas del {FechaInicio} al {FechaFin}", 
                    fechaInicio, fechaFin);
                throw;
            }
        }

        public async Task<IEnumerable<Reserva>> GetPendientesConfirmacionAsync()
        {
            try
            {
                return await _dbSet
                    .Where(r => r.RequiereConfirmacion && r.Estado == "Pendiente")
                    .OrderBy(r => r.FechaCreacion)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar reservas pendientes de confirmación");
                throw;
            }
        }

        public async Task<IEnumerable<Reserva>> GetSinPagarAsync()
        {
            try
            {
                return await _dbSet
                    .Where(r => r.PagoId == null 
                             && r.Estado != "Cancelada"
                             && r.FechaInicio >= DateTime.Today)
                    .OrderBy(r => r.FechaInicio)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar reservas sin pagar");
                throw;
            }
        }

        public async Task<IEnumerable<Reserva>> GetByItemAsync(string tipoReserva, int itemId)
        {
            if (string.IsNullOrWhiteSpace(tipoReserva))
                throw new ArgumentException("El tipo de reserva no puede estar vacío", nameof(tipoReserva));

            try
            {
                return await _dbSet
                    .Where(r => r.TipoReserva == tipoReserva && r.ItemId == itemId)
                    .OrderByDescending(r => r.FechaCreacion)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar reservas para {Tipo} ID {ItemId}", 
                    tipoReserva, itemId);
                throw;
            }
        }

        public async Task<bool> VerificarConflictoFechasAsync(
            int itemId, 
            DateTime fechaInicio, 
            DateTime fechaFin, 
            int? excluirReservaId = null)
        {
            try
            {
                var query = _dbSet.Where(r => r.ItemId == itemId
                                           && r.TipoReserva == "Hospedaje"
                                           && r.Estado != "Cancelada"
                                           && r.FechaInicio < fechaFin
                                           && r.FechaFin > fechaInicio);

                if (excluirReservaId.HasValue)
                {
                    query = query.Where(r => r.Id != excluirReservaId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar conflicto de fechas para item {ItemId}", itemId);
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetEstadisticasPorEstadoAsync()
        {
            try
            {
                return await _dbSet
                    .GroupBy(r => r.Estado)
                    .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
                    .ToDictionaryAsync(x => x.Estado, x => x.Cantidad);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de reservas por estado");
                throw;
            }
        }
    }
}
