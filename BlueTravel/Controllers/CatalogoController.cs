using BlueTravel.Data;
using BlueTravel.Data.Repositories;
using BlueTravel.Models;
using BlueTravel.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlueTravel.Controllers
{
    public class CatalogoController : Controller
    {
        // ✅ SEMANA 2: Usar repositorios en lugar de DbContext directo
        private readonly IHospedajeRepository _hospedajeRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IOfertaRepository _ofertaRepository;
        private readonly ICacheService _cacheService; // ✅ SEMANA 3
        private readonly ApplicationDbContext _context; // Mantener temporalmente para Restaurantes
        private readonly ILogger<CatalogoController> _logger;

        // ✅ SEMANA 3: Constantes para configuración de paginación
        private const int DEFAULT_PAGE_SIZE = 12; // 12 items por página (3x4 grid)
        private const string CACHE_KEY_HOSPEDAJES = "hospedajes_all";
        private const string CACHE_KEY_TOURS = "tours_all";
        private const string CACHE_KEY_OFERTAS = "ofertas_activas";

        public CatalogoController(
            IHospedajeRepository hospedajeRepository,
            ITourRepository tourRepository,
            IOfertaRepository ofertaRepository,
            ICacheService cacheService,
            ApplicationDbContext context,
            ILogger<CatalogoController> logger)
        {
            _hospedajeRepository = hospedajeRepository;
            _tourRepository = tourRepository;
            _ofertaRepository = ofertaRepository;
            _cacheService = cacheService;
            _context = context;
            _logger = logger;
        }

        // ✅ SEMANA 3: Paginación y Cache
        public async Task<IActionResult> Hospedajes(int pageNumber = 1, int pageSize = DEFAULT_PAGE_SIZE)
        {
            try
            {
                // Intentar obtener del cache
                var cacheKey = $"{CACHE_KEY_HOSPEDAJES}_page_{pageNumber}_size_{pageSize}";
                
                var paginatedHospedajes = await _cacheService.GetOrCreateAsync(
                    cacheKey,
                    async () =>
                    {
                        _logger.LogInformation("Cargando hospedajes desde BD (página {Page})", pageNumber);
                        return await _hospedajeRepository.GetPagedAsync(pageNumber, pageSize);
                    },
                    TimeSpan.FromMinutes(5) // Cache de 5 minutos
                );

                return View(paginatedHospedajes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener hospedajes");
                TempData["ErrorMessage"] = "Error al cargar los hospedajes. Por favor, intenta de nuevo.";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Tours(int pageNumber = 1, int pageSize = DEFAULT_PAGE_SIZE)
        {
            try
            {
                var cacheKey = $"{CACHE_KEY_TOURS}_page_{pageNumber}_size_{pageSize}";
                
                var paginatedTours = await _cacheService.GetOrCreateAsync(
                    cacheKey,
                    async () =>
                    {
                        _logger.LogInformation("Cargando tours desde BD (página {Page})", pageNumber);
                        return await _tourRepository.GetPagedAsync(pageNumber, pageSize);
                    },
                    TimeSpan.FromMinutes(5)
                );

                return View(paginatedTours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tours");
                TempData["ErrorMessage"] = "Error al cargar los tours. Por favor, intenta de nuevo.";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Restaurantes()
        {
            try
            {
                // TODO SEMANA 3: Crear RestauranteRepository y agregar paginación
                var restaurantes = await _cacheService.GetOrCreateAsync(
                    "restaurantes_all",
                    async () => await _context.Restaurantes.AsNoTracking().ToListAsync(),
                    TimeSpan.FromMinutes(10)
                );

                return View(restaurantes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener restaurantes");
                TempData["ErrorMessage"] = "Error al cargar los restaurantes. Por favor, intenta de nuevo.";
                return RedirectToAction("Index", "Home");
            }
        }
        
        // 🔥 TEMPORAL: Endpoint de diagnóstico para ofertas
        [HttpGet("Debug/OfertasCount")]
        public async Task<IActionResult> DebugOfertasCount()
        {
            try
            {
                var todasOfertas = await _context.Ofertas.ToListAsync();
                var ofertasActivas = await _ofertaRepository.GetActivasAsync();
                
                var resultado = new
                {
                    TotalEnBD = todasOfertas.Count,
                    Activas = ofertasActivas.Count(),
                    Hoy = DateTime.Today,
                    Ofertas = todasOfertas.Select(o => new
                    {
                        o.Id,
                        o.Titulo,
                        o.FechaInicio,
                        o.FechaFin,
                        EstaActiva = o.FechaInicio <= DateTime.Today && o.FechaFin >= DateTime.Today,
                        DiasRestantes = (o.FechaFin - DateTime.Today).Days
                    })
                };
                
                return Json(resultado);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public async Task<IActionResult> Ofertas(int pageNumber = 1, int pageSize = DEFAULT_PAGE_SIZE)
        {
            try
            {
                var cacheKey = $"{CACHE_KEY_OFERTAS}_page_{pageNumber}_size_{pageSize}";
                
                // Para ofertas, usamos un método específico que filtra las activas
                var ofertasActivas = await _cacheService.GetOrCreateAsync(
                    cacheKey,
                    async () =>
                    {
                        _logger.LogInformation("Cargando ofertas activas desde BD (página {Page})", pageNumber);
                        
                        var todasOfertas = await _ofertaRepository.GetActivasAsync();
                        
                        _logger.LogInformation("Ofertas activas encontradas: {Count}", todasOfertas.Count());
                        
                        // Crear paginación manual desde IEnumerable
                        return PaginatedList<Oferta>.Create(
                            todasOfertas,
                            pageNumber,
                            pageSize
                        );
                    },
                    TimeSpan.FromMinutes(2) // Ofertas con cache más corto (pueden cambiar rápido)
                );

                _logger.LogInformation("Ofertas en resultado paginado: {Count}", ofertasActivas.Items.Count());
                
                return View(ofertasActivas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ofertas");
                TempData["ErrorMessage"] = "Error al cargar las ofertas. Por favor, intenta de nuevo.";
                return RedirectToAction("Index", "Home");
            }
        }

        // 👇 NUEVO: Vista de detalles unificada con cache
        public async Task<IActionResult> Details(string tipo, int id)
        {
            try
            {
                ViewBag.Tipo = tipo;
                var cacheKey = $"details_{tipo}_{id}";

                switch (tipo?.ToLower())
                {
                    case "hospedaje":
                        var hospedaje = await _cacheService.GetOrCreateAsync(
                            cacheKey,
                            async () =>
                            {
                                var h = await _hospedajeRepository.GetByIdAsync(id);
                                if (h == null)
                                {
                                    _logger.LogWarning("Hospedaje {Id} no encontrado", id);
                                }
                                return h;
                            },
                            TimeSpan.FromMinutes(10)
                        );

                        if (hospedaje == null) return NotFound();
                        return View("HospedajeDetails", hospedaje);

                    case "tour":
                        var tour = await _cacheService.GetOrCreateAsync(
                            cacheKey,
                            async () =>
                            {
                                var t = await _tourRepository.GetByIdAsync(id);
                                if (t == null)
                                {
                                    _logger.LogWarning("Tour {Id} no encontrado", id);
                                }
                                return t;
                            },
                            TimeSpan.FromMinutes(10)
                        );

                        if (tour == null) return NotFound();
                        return View("TourDetails", tour);

                    case "restaurante":
                        var restaurante = await _cacheService.GetOrCreateAsync(
                            cacheKey,
                            async () => await _context.Restaurantes.FindAsync(id),
                            TimeSpan.FromMinutes(10)
                        );

                        if (restaurante == null)
                        {
                            _logger.LogWarning("Restaurante {Id} no encontrado", id);
                            return NotFound();
                        }
                        return View("RestauranteDetails", restaurante);

                    case "oferta":
                        var oferta = await _cacheService.GetOrCreateAsync(
                            cacheKey,
                            async () => await _ofertaRepository.GetByIdAsync(id),
                            TimeSpan.FromMinutes(5) // Cache más corto para ofertas
                        );

                        if (oferta == null)
                        {
                            _logger.LogWarning("Oferta {Id} no encontrada", id);
                            return NotFound();
                        }
                        
                        // Verificar que la oferta esté vigente
                        if (!await _ofertaRepository.EstaVigenteAsync(id))
                        {
                            _logger.LogWarning("Oferta {Id} ya no está vigente", id);
                            TempData["ErrorMessage"] = "Esta oferta ya no está disponible.";
                            return RedirectToAction("Ofertas");
                        }
                        
                        return View("OfertaDetails", oferta);

                    default:
                        _logger.LogWarning("Tipo de catálogo inválido: {Tipo}", tipo);
                        return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles de {Tipo} con ID {Id}", tipo, id);
                TempData["ErrorMessage"] = "Error al cargar los detalles. Por favor, intenta de nuevo.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}