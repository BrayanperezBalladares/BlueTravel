using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BlueTravel.Models;

namespace BlueTravel.Data.Repositories
{
    /// <summary>
    /// Implementación genérica del patrón Repository
    /// Proporciona operaciones CRUD básicas para cualquier entidad
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;
        protected readonly ILogger<Repository<T>> _logger;

        public Repository(
            ApplicationDbContext context,
            ILogger<Repository<T>> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbSet = context.Set<T>();
        }

        // ? CONSULTAS

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await _dbSet.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las entidades de tipo {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la entidad {EntityType} con ID {Id}", typeof(T).Name, id);
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _dbSet
                    .Where(predicate)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar entidades de tipo {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _dbSet
                    .Where(predicate)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar la primera entidad de tipo {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _dbSet.AnyAsync(predicate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia de entidades de tipo {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            try
            {
                return predicate == null 
                    ? await _dbSet.CountAsync() 
                    : await _dbSet.CountAsync(predicate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al contar entidades de tipo {EntityType}", typeof(T).Name);
                throw;
            }
        }

        // ? OPERACIONES DE ESCRITURA

        public virtual async Task<T> AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                await _dbSet.AddAsync(entity);
                _logger.LogInformation("Entidad {EntityType} agregada", typeof(T).Name);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar entidad de tipo {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
                throw new ArgumentException("La colección de entidades no puede estar vacía", nameof(entities));

            try
            {
                await _dbSet.AddRangeAsync(entities);
                _logger.LogInformation("{Count} entidades de tipo {EntityType} agregadas", 
                    entities.Count(), typeof(T).Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar múltiples entidades de tipo {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual Task UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                _dbSet.Update(entity);
                _logger.LogInformation("Entidad {EntityType} actualizada", typeof(T).Name);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar entidad de tipo {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual Task DeleteAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                _dbSet.Remove(entity);
                _logger.LogInformation("Entidad {EntityType} eliminada", typeof(T).Name);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar entidad de tipo {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity != null)
                {
                    await DeleteAsync(entity);
                }
                else
                {
                    _logger.LogWarning("No se encontró la entidad {EntityType} con ID {Id} para eliminar", 
                        typeof(T).Name, id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar entidad {EntityType} con ID {Id}", typeof(T).Name, id);
                throw;
            }
        }

        public virtual Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
                throw new ArgumentException("La colección de entidades no puede estar vacía", nameof(entities));

            try
            {
                _dbSet.RemoveRange(entities);
                _logger.LogInformation("{Count} entidades de tipo {EntityType} eliminadas", 
                    entities.Count(), typeof(T).Name);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar múltiples entidades de tipo {EntityType}", typeof(T).Name);
                throw;
            }
        }

        // ? PERSISTENCIA

        public virtual async Task<int> SaveChangesAsync()
        {
            try
            {
                var changes = await _context.SaveChangesAsync();
                _logger.LogInformation("{Changes} cambios guardados en la base de datos", changes);
                return changes;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de actualización de base de datos");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar cambios en la base de datos");
                throw;
            }
        }

        // ? SEMANA 3: PAGINACIÓN

        public virtual async Task<PaginatedList<T>> GetPagedAsync(int pageIndex, int pageSize)
        {
            try
            {
                if (pageIndex < 1)
                    pageIndex = 1;

                if (pageSize < 1)
                    pageSize = 10;

                var count = await _dbSet.CountAsync();
                var items = await _dbSet
                    .AsNoTracking()
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("Obtenidos {Count} elementos de página {PageIndex} (tamaño {PageSize})",
                    items.Count, pageIndex, pageSize);

                return new PaginatedList<T>(items, count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener página {PageIndex} de entidades {EntityType}",
                    pageIndex, typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<PaginatedList<T>> GetPagedAsync(
            Expression<Func<T, bool>> predicate,
            int pageIndex,
            int pageSize)
        {
            try
            {
                if (pageIndex < 1)
                    pageIndex = 1;

                if (pageSize < 1)
                    pageSize = 10;

                var query = _dbSet.Where(predicate);
                var count = await query.CountAsync();
                var items = await query
                    .AsNoTracking()
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("Obtenidos {Count} elementos filtrados de página {PageIndex} (tamaño {PageSize})",
                    items.Count, pageIndex, pageSize);

                return new PaginatedList<T>(items, count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener página {PageIndex} filtrada de entidades {EntityType}",
                    pageIndex, typeof(T).Name);
                throw;
            }
        }
    }
}
