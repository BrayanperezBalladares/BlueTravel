using Microsoft.Extensions.Caching.Memory;

namespace BlueTravel.Services
{
    /// <summary>
    /// Interfaz para servicio de caché centralizado
    /// Proporciona métodos para cachear datos frecuentemente accedidos
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Obtiene un valor del cache o lo crea si no existe
        /// </summary>
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? absoluteExpiration = null);

        /// <summary>
        /// Obtiene un valor del cache
        /// </summary>
        T? Get<T>(string key);

        /// <summary>
        /// Establece un valor en el cache
        /// </summary>
        void Set<T>(string key, T value, TimeSpan? absoluteExpiration = null);

        /// <summary>
        /// Remueve un valor del cache
        /// </summary>
        void Remove(string key);

        /// <summary>
        /// Remueve múltiples valores del cache que coincidan con un patrón
        /// </summary>
        void RemoveByPattern(string pattern);

        /// <summary>
        /// Limpia todo el cache
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// Implementación del servicio de caché usando IMemoryCache
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CacheService> _logger;
        private readonly HashSet<string> _cacheKeys;
        private readonly object _lock = new();

        public CacheService(
            IMemoryCache memoryCache,
            ILogger<CacheService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _cacheKeys = new HashSet<string>();
        }

        public async Task<T> GetOrCreateAsync<T>(
            string key,
            Func<Task<T>> factory,
            TimeSpan? absoluteExpiration = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("La clave del cache no puede estar vacía", nameof(key));

            if (_memoryCache.TryGetValue(key, out T? cachedValue))
            {
                _logger.LogDebug("Cache HIT para clave: {Key}", key);
                return cachedValue!;
            }

            _logger.LogDebug("Cache MISS para clave: {Key}", key);

            var value = await factory();

            var cacheOptions = new MemoryCacheEntryOptions();

            if (absoluteExpiration.HasValue)
            {
                cacheOptions.SetAbsoluteExpiration(absoluteExpiration.Value);
            }
            else
            {
                // Por defecto: 10 minutos absoluto, 2 minutos sliding
                cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(2));
            }

            // Callback cuando el item es removido del cache
            cacheOptions.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                lock (_lock)
                {
                    _cacheKeys.Remove(key.ToString()!);
                }
                _logger.LogDebug("Item removido del cache: {Key}, Razón: {Reason}", key, reason);
            });

            _memoryCache.Set(key, value, cacheOptions);

            lock (_lock)
            {
                _cacheKeys.Add(key);
            }

            _logger.LogInformation("Item agregado al cache: {Key}", key);

            return value;
        }

        public T? Get<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("La clave del cache no puede estar vacía", nameof(key));

            if (_memoryCache.TryGetValue(key, out T? value))
            {
                _logger.LogDebug("Cache HIT para clave: {Key}", key);
                return value;
            }

            _logger.LogDebug("Cache MISS para clave: {Key}", key);
            return default;
        }

        public void Set<T>(string key, T value, TimeSpan? absoluteExpiration = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("La clave del cache no puede estar vacía", nameof(key));

            var cacheOptions = new MemoryCacheEntryOptions();

            if (absoluteExpiration.HasValue)
            {
                cacheOptions.SetAbsoluteExpiration(absoluteExpiration.Value);
            }
            else
            {
                cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(2));
            }

            cacheOptions.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                lock (_lock)
                {
                    _cacheKeys.Remove(key.ToString()!);
                }
            });

            _memoryCache.Set(key, value, cacheOptions);

            lock (_lock)
            {
                _cacheKeys.Add(key);
            }

            _logger.LogInformation("Item actualizado en cache: {Key}", key);
        }

        public void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("La clave del cache no puede estar vacía", nameof(key));

            _memoryCache.Remove(key);

            lock (_lock)
            {
                _cacheKeys.Remove(key);
            }

            _logger.LogInformation("Item removido del cache: {Key}", key);
        }

        public void RemoveByPattern(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentException("El patrón no puede estar vacío", nameof(pattern));

            List<string> keysToRemove;

            lock (_lock)
            {
                keysToRemove = _cacheKeys
                    .Where(k => k.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            foreach (var key in keysToRemove)
            {
                Remove(key);
            }

            _logger.LogInformation("Removidos {Count} items del cache con patrón: {Pattern}",
                keysToRemove.Count, pattern);
        }

        public void Clear()
        {
            List<string> keysToRemove;

            lock (_lock)
            {
                keysToRemove = _cacheKeys.ToList();
            }

            foreach (var key in keysToRemove)
            {
                _memoryCache.Remove(key);
            }

            lock (_lock)
            {
                _cacheKeys.Clear();
            }

            _logger.LogWarning("Cache completamente limpiado. {Count} items removidos", keysToRemove.Count);
        }
    }
}
