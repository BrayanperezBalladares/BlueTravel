using System.Linq.Expressions;
using BlueTravel.Models;

namespace BlueTravel.Data.Repositories
{
    /// <summary>
    /// Interfaz genérica para operaciones CRUD básicas
    /// Aplica el patrón Repository para separar la lógica de acceso a datos
    /// </summary>
    /// <typeparam name="T">Tipo de entidad del modelo</typeparam>
    public interface IRepository<T> where T : class
    {
        // ? CONSULTAS
        
        /// <summary>
        /// Obtiene todas las entidades
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Obtiene una entidad por su ID
        /// </summary>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Busca entidades que cumplan con un criterio
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Busca una entidad que cumpla con un criterio
        /// </summary>
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Verifica si existe una entidad que cumpla con un criterio
        /// </summary>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Cuenta las entidades que cumplen con un criterio
        /// </summary>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

        // ? OPERACIONES DE ESCRITURA

        /// <summary>
        /// Agrega una nueva entidad
        /// </summary>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Agrega múltiples entidades
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Actualiza una entidad existente
        /// </summary>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Elimina una entidad
        /// </summary>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Elimina una entidad por su ID
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Elimina múltiples entidades
        /// </summary>
        Task DeleteRangeAsync(IEnumerable<T> entities);

        // ? PERSISTENCIA

        /// <summary>
        /// Guarda los cambios en la base de datos
        /// </summary>
        Task<int> SaveChangesAsync();

        // ? SEMANA 3: PAGINACIÓN

        /// <summary>
        /// Obtiene una página de elementos
        /// </summary>
        Task<PaginatedList<T>> GetPagedAsync(int pageIndex, int pageSize);

        /// <summary>
        /// Obtiene una página de elementos que cumplan con un criterio
        /// </summary>
        Task<PaginatedList<T>> GetPagedAsync(
            Expression<Func<T, bool>> predicate,
            int pageIndex,
            int pageSize);
    }
}
