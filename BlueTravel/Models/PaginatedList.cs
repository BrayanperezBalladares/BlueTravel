namespace BlueTravel.Models
{
    /// <summary>
    /// Modelo genérico para representar una lista paginada de elementos
    /// Incluye metadatos de paginación para la UI
    /// </summary>
    /// <typeparam name="T">Tipo de elementos en la lista</typeparam>
    public class PaginatedList<T>
    {
        /// <summary>
        /// Lista de elementos de la página actual
        /// </summary>
        public List<T> Items { get; }

        /// <summary>
        /// Número de página actual (base 1)
        /// </summary>
        public int PageIndex { get; }

        /// <summary>
        /// Total de páginas disponibles
        /// </summary>
        public int TotalPages { get; }

        /// <summary>
        /// Total de elementos en toda la colección
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// Cantidad de elementos por página
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Constructor de lista paginada
        /// </summary>
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            Items = items;
            TotalCount = count;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        /// <summary>
        /// Indica si existe una página anterior
        /// </summary>
        public bool HasPreviousPage => PageIndex > 1;

        /// <summary>
        /// Indica si existe una página siguiente
        /// </summary>
        public bool HasNextPage => PageIndex < TotalPages;

        /// <summary>
        /// Obtiene el número del primer elemento de la página actual
        /// </summary>
        public int FirstItemIndex => (PageIndex - 1) * PageSize + 1;

        /// <summary>
        /// Obtiene el número del último elemento de la página actual
        /// </summary>
        public int LastItemIndex => Math.Min(PageIndex * PageSize, TotalCount);

        /// <summary>
        /// Crea una lista paginada desde un IQueryable
        /// </summary>
        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, 
            int pageIndex, 
            int pageSize)
        {
            var count = await Task.Run(() => source.Count());
            var items = await Task.Run(() => source
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList());

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }

        /// <summary>
        /// Crea una lista paginada desde un IEnumerable (ya cargado en memoria)
        /// </summary>
        public static PaginatedList<T> Create(
            IEnumerable<T> source,
            int pageIndex,
            int pageSize)
        {
            var sourceList = source.ToList();
            var count = sourceList.Count;
            var items = sourceList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }

        /// <summary>
        /// Obtiene un rango de páginas para mostrar en la paginación
        /// (útil para crear botones de navegación)
        /// </summary>
        /// <param name="maxPagesToShow">Máximo de páginas a mostrar en el rango</param>
        public List<int> GetPageRange(int maxPagesToShow = 5)
        {
            var startPage = Math.Max(1, PageIndex - maxPagesToShow / 2);
            var endPage = Math.Min(TotalPages, startPage + maxPagesToShow - 1);

            // Ajustar si estamos cerca del final
            if (endPage - startPage < maxPagesToShow - 1)
            {
                startPage = Math.Max(1, endPage - maxPagesToShow + 1);
            }

            return Enumerable.Range(startPage, endPage - startPage + 1).ToList();
        }
    }
}
