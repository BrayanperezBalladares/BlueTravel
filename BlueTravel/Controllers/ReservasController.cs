using BlueTravel.Data;
using BlueTravel.Data.Repositories;
using BlueTravel.Models;
using BlueTravel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlueTravel.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        // ✅ SEMANA 5: Usar repositorios en lugar de DbContext directo
        private readonly IReservaRepository _reservaRepository;
        private readonly IHospedajeRepository _hospedajeRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IOfertaRepository _ofertaRepository;
        private readonly ICacheService _cacheService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ReservasController> _logger;
        private readonly IPrecioService _precioService;
        private readonly INotificacionService _notificacionService;

        public ReservasController(
            IReservaRepository reservaRepository,
            IHospedajeRepository hospedajeRepository,
            ITourRepository tourRepository,
            IOfertaRepository ofertaRepository,
            ICacheService cacheService,
            UserManager<IdentityUser> userManager,
            ILogger<ReservasController> logger,
            IPrecioService precioService,
            INotificacionService notificacionService)
        {
            _reservaRepository = reservaRepository;
            _hospedajeRepository = hospedajeRepository;
            _tourRepository = tourRepository;
            _ofertaRepository = ofertaRepository;
            _cacheService = cacheService;
            _userManager = userManager;
            _logger = logger;
            _precioService = precioService;
            _notificacionService = notificacionService;
        }

        // GET: Reservas - Mis Reservas (para clientes)
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var userId = _userManager.GetUserId(User);
            
            if (User.IsInRole("Admin"))
            {
                // Admin redirige a AdminIndex
                return RedirectToAction(nameof(AdminIndex), new { pageNumber, pageSize });
            }
            else
            {
                // Cliente ve sus reservas con cache y paginación
                var cacheKey = $"reservas_user_{userId}_page_{pageNumber}_size_{pageSize}";
                
                var paginatedReservas = await _cacheService.GetOrCreateAsync(
                    cacheKey,
                    async () =>
                    {
                        _logger.LogInformation("Cargando reservas del usuario {UserId} desde BD", userId);
                        var reservas = await _reservaRepository.GetByUsuarioIdAsync(userId);
                        return PaginatedList<Reserva>.Create(reservas, pageNumber, pageSize);
                    },
                    TimeSpan.FromMinutes(5) // Cache de 5 minutos
                );
                
                return View(paginatedReservas);
            }
        }

        // GET: Reservas/AdminIndex - Vista de administrador
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex(int pageNumber = 1, int pageSize = 20)
        {
            var cacheKey = $"reservas_admin_page_{pageNumber}_size_{pageSize}";
            
            var paginatedReservas = await _cacheService.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    _logger.LogInformation("Cargando todas las reservas para admin (página {PageNumber})", pageNumber);
                    var reservas = await _reservaRepository.GetAllAsync();
                    return PaginatedList<Reserva>.Create(reservas.OrderByDescending(r => r.FechaCreacion), pageNumber, pageSize);
                },
                TimeSpan.FromMinutes(3) // Cache de 3 minutos
            );
            
            return View(paginatedReservas);
        }

        // GET: Reservas/CreateHospedaje?id=1
        public async Task<IActionResult> CreateHospedaje(int id)
        {
            var hospedaje = await _hospedajeRepository.GetByIdAsync(id);
            if (hospedaje == null)
            {
                TempData["ErrorMessage"] = "Hospedaje no encontrado.";
                return RedirectToAction("Hospedajes", "Catalogo");
            }

            var reserva = new Reserva
            {
                TipoReserva = "Hospedaje",
                ItemId = id,
                FechaInicio = DateTime.Today.AddDays(1),
                FechaFin = DateTime.Today.AddDays(2),
                CantidadAdultos = 1
            };

            ViewBag.Hospedaje = hospedaje;
            return View(reserva);
        }

        // POST: Reservas/CreateHospedaje
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateHospedaje([Bind("TipoReserva,ItemId,FechaInicio,FechaFin,CantidadAdultos,CantidadNinos,CantidadSeniors,Comentarios")] Reserva reserva)
        {
            ModelState.Remove("UsuarioId");
            ModelState.Remove("PrecioTotal");
            ModelState.Remove("PrecioBase");
            ModelState.Remove("CargoPersonasExtra");
            ModelState.Remove("DescuentoAplicado");
            ModelState.Remove("Estado");
            ModelState.Remove("FechaCreacion");
            ModelState.Remove("ItemNombre");

            if (!ModelState.IsValid)
            {
                var hospedaje = await _hospedajeRepository.GetByIdAsync(reserva.ItemId);
                ViewBag.Hospedaje = hospedaje;
                return View(reserva);
            }

            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["ErrorMessage"] = "Debes iniciar sesión para hacer una reserva.";
                    return RedirectToAction("Login", "Account");
                }

                var hospedaje = await _hospedajeRepository.GetByIdAsync(reserva.ItemId);
                if (hospedaje == null)
                {
                    TempData["ErrorMessage"] = "Hospedaje no encontrado.";
                    return RedirectToAction("Hospedajes", "Catalogo");
                }

                var totalPersonas = reserva.CantidadPersonas;

                // Validar capacidad
                if (totalPersonas > hospedaje.CapacidadMaxima)
                {
                    ModelState.AddModelError(string.Empty, $"Capacidad máxima: {hospedaje.CapacidadMaxima} personas");
                    ViewBag.Hospedaje = hospedaje;
                    return View(reserva);
                }

                // Validar restricciones
                if (reserva.CantidadNinos > 0 && !hospedaje.PermiteNinos)
                {
                    ModelState.AddModelError(string.Empty, "Este hospedaje no permite niños.");
                    ViewBag.Hospedaje = hospedaje;
                    return View(reserva);
                }

                // Validar disponibilidad
                bool disponible = await _hospedajeRepository.VerificarDisponibilidadAsync(
                    hospedaje.Id, reserva.FechaInicio, reserva.FechaFin);
                    
                if (!disponible)
                {
                    ModelState.AddModelError(string.Empty, "El hospedaje NO está disponible para estas fechas.");
                    ViewBag.Hospedaje = hospedaje;
                    return View(reserva);
                }

                // Asignar datos
                reserva.UsuarioId = userId;
                reserva.ItemNombre = hospedaje.Nombre;
                reserva.FechaCreacion = DateTime.Now;
                reserva.Estado = "Pendiente";

                // Calcular precio
                var resultadoCalculo = await _precioService.CalcularPrecioHospedaje(
                    hospedaje, 
                    reserva.FechaInicio, 
                    reserva.FechaFin,
                    reserva.CantidadAdultos,
                    reserva.CantidadNinos,
                    reserva.CantidadSeniors);

                reserva.PrecioBase = resultadoCalculo.PrecioBase;
                reserva.CargoPersonasExtra = resultadoCalculo.CargoPersonasExtra;
                reserva.DescuentoAplicado = resultadoCalculo.DescuentoPromocional;
                reserva.PrecioTotal = resultadoCalculo.Total;

                // Guardar
                await _reservaRepository.AddAsync(reserva);
                await _reservaRepository.SaveChangesAsync();

                // Limpiar cache
                _cacheService.RemoveByPattern($"reservas_user_{userId}");

                // Notificaciones
                var user = await _userManager.GetUserAsync(User);
                if (user?.Email != null)
                {
                    await _notificacionService.EnviarConfirmacionReserva(reserva, user.Email);
                }

                TempData["SuccessMessage"] = $"¡Reserva creada! Total: {reserva.PrecioTotal:C}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear reserva de hospedaje");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                var hospedaje = await _hospedajeRepository.GetByIdAsync(reserva.ItemId);
                ViewBag.Hospedaje = hospedaje;
                return View(reserva);
            }
        }

        // GET: Reservas/CreateTour?id=1
        public async Task<IActionResult> CreateTour(int id)
        {
            var tour = await _tourRepository.GetByIdAsync(id);
            if (tour == null)
            {
                TempData["ErrorMessage"] = "Tour no encontrado.";
                return RedirectToAction("Tours", "Catalogo");
            }

            var reserva = new Reserva
            {
                TipoReserva = "Tour",
                ItemId = id,
                FechaInicio = tour.FechaDisponible,
                FechaFin = tour.FechaDisponible.AddDays(tour.Duracion),
                CantidadAdultos = 1
            };

            ViewBag.Tour = tour;
            ViewBag.CuposDisponibles = tour.CuposDisponibles;
            return View(reserva);
        }

        // POST: Reservas/CreateTour
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTour([Bind("TipoReserva,ItemId,FechaInicio,CantidadAdultos,CantidadNinos,CantidadSeniors,Comentarios")] Reserva reserva)
        {
            ModelState.Remove("UsuarioId");
            ModelState.Remove("PrecioTotal");
            ModelState.Remove("PrecioBase");
            ModelState.Remove("CargoPersonasExtra");
            ModelState.Remove("DescuentoAplicado");
            ModelState.Remove("Estado");
            ModelState.Remove("FechaCreacion");
            ModelState.Remove("ItemNombre");
            ModelState.Remove("FechaFin");

            if (!ModelState.IsValid)
            {
                var tour = await _tourRepository.GetByIdAsync(reserva.ItemId);
                ViewBag.Tour = tour;
                ViewBag.CuposDisponibles = tour?.CuposDisponibles;
                return View(reserva);
            }

            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["ErrorMessage"] = "Debes iniciar sesión para hacer una reserva.";
                    return RedirectToAction("Login", "Account");
                }

                var tour = await _tourRepository.GetByIdAsync(reserva.ItemId);
                if (tour == null)
                {
                    TempData["ErrorMessage"] = "Tour no encontrado.";
                    return RedirectToAction("Tours", "Catalogo");
                }

                var totalPersonas = reserva.CantidadPersonas;

                // Validar cupos
                if (totalPersonas > tour.CuposDisponibles)
                {
                    ModelState.AddModelError(string.Empty, $"Cupos disponibles: {tour.CuposDisponibles}");
                    ViewBag.Tour = tour;
                    ViewBag.CuposDisponibles = tour.CuposDisponibles;
                    return View(reserva);
                }

                // Validar restricciones de edad
                if (reserva.CantidadNinos > 0 && tour.EdadMinima > 12)
                {
                    ModelState.AddModelError(string.Empty, $"Este tour requiere edad mínima de {tour.EdadMinima} años.");
                    ViewBag.Tour = tour;
                    ViewBag.CuposDisponibles = tour.CuposDisponibles;
                    return View(reserva);
                }

                // Asignar datos
                reserva.UsuarioId = userId;
                reserva.ItemNombre = tour.Nombre;
                reserva.FechaFin = tour.FechaDisponible.AddDays(tour.Duracion);
                reserva.FechaCreacion = DateTime.Now;
                reserva.Estado = tour.RequiereConfirmacion ? "Pendiente de Confirmación" : "Confirmada";
                reserva.RequiereConfirmacion = tour.RequiereConfirmacion;

                // Calcular precio
                var resultadoCalculo = await _precioService.CalcularPrecioTour(
                    tour,
                    reserva.CantidadAdultos,
                    reserva.CantidadNinos,
                    reserva.CantidadSeniors);

                reserva.PrecioBase = resultadoCalculo.PrecioBase;
                reserva.CargoPersonasExtra = 0;
                reserva.DescuentoAplicado = resultadoCalculo.DescuentoGrupo;
                reserva.PrecioTotal = resultadoCalculo.Total;

                // Guardar
                await _reservaRepository.AddAsync(reserva);
                await _reservaRepository.SaveChangesAsync();

                // Reservar cupos
                await _tourRepository.ReservarCuposAsync(tour.Id, totalPersonas);

                // Limpiar cache
                _cacheService.RemoveByPattern($"reservas_user_{userId}");

                // Notificaciones
                var user = await _userManager.GetUserAsync(User);
                if (user?.Email != null)
                {
                    await _notificacionService.EnviarConfirmacionReserva(reserva, user.Email);
                }

                if (tour.RequiereConfirmacion)
                {
                    await _notificacionService.NotificarAdminNuevaReserva(reserva);
                }

                string mensaje = $"¡Reserva creada! Total: {reserva.PrecioTotal:C}";
                if (tour.RequiereConfirmacion)
                {
                    mensaje += " (Requiere confirmación del administrador)";
                }
                TempData["SuccessMessage"] = mensaje;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear reserva de tour");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                var tour = await _tourRepository.GetByIdAsync(reserva.ItemId);
                ViewBag.Tour = tour;
                ViewBag.CuposDisponibles = tour?.CuposDisponibles;
                return View(reserva);
            }
        }

        // GET: Reservas/Create?tipo=Hospedaje&id=1
        public async Task<IActionResult> Create(string tipo, int id)
        {
            if (string.IsNullOrEmpty(tipo) || id <= 0)
            {
                TempData["ErrorMessage"] = "Parámetros inválidos para crear reserva.";
                return RedirectToAction("Index", "Home");
            }

            var reserva = new Reserva
            {
                TipoReserva = tipo,
                ItemId = id,
                FechaInicio = DateTime.Today.AddDays(1),
                FechaFin = DateTime.Today.AddDays(2),
                CantidadAdultos = 1
            };

            // ✅ SEMANA 5: Usar repositorios en lugar de DbContext
            if (tipo == "Hospedaje")
            {
                var hospedaje = await _hospedajeRepository.GetByIdAsync(id);
                if (hospedaje == null)
                {
                    TempData["ErrorMessage"] = "Hospedaje no encontrado.";
                    return RedirectToAction("Hospedajes", "Catalogo");
                }
                reserva.ItemNombre = hospedaje.Nombre;
                ViewBag.PrecioPorNoche = hospedaje.PrecioPorNoche;
                ViewBag.CapacidadMaxima = hospedaje.CapacidadMaxima;
                ViewBag.PersonasIncluidas = hospedaje.PersonasIncluidasEnPrecio;
                ViewBag.CargoPorPersonaExtra = hospedaje.CargoPorPersonaExtra;
                ViewBag.ItemInfo = $"{hospedaje.Nombre} - {hospedaje.Ubicacion} ({hospedaje.InfoCapacidad})";
            }
            else if (tipo == "Tour")
            {
                var tour = await _tourRepository.GetByIdAsync(id);
                if (tour == null)
                {
                    TempData["ErrorMessage"] = "Tour no encontrado.";
                    return RedirectToAction("Tours", "Catalogo");
                }
                reserva.ItemNombre = tour.Nombre;
                ViewBag.PrecioAdulto = tour.Precio;
                ViewBag.PrecioNino = tour.PrecioNino;
                ViewBag.PrecioSenior = tour.PrecioSenior;
                ViewBag.CuposDisponibles = tour.CuposDisponibles;
                ViewBag.DescuentoGrupo = tour.DescuentoGrupo;
                ViewBag.ItemInfo = $"{tour.Nombre} - {tour.Ubicacion} ({tour.EstadoDisponibilidad})";
                reserva.FechaInicio = tour.FechaDisponible;
                reserva.FechaFin = tour.FechaDisponible.AddDays(tour.Duracion);
                reserva.RequiereConfirmacion = tour.RequiereConfirmacion;
            }
            else if (tipo == "Oferta")
            {
                var oferta = await _ofertaRepository.GetByIdAsync(id);
                if (oferta == null)
                {
                    TempData["ErrorMessage"] = "Oferta no encontrada.";
                    return RedirectToAction("Ofertas", "Catalogo");
                }
                reserva.ItemNombre = oferta.Titulo;
                ViewBag.PrecioOferta = oferta.Precio;
                ViewBag.ItemInfo = $"{oferta.Titulo} (Válido hasta {oferta.FechaFin:dd/MM/yyyy})";
                ViewBag.ItemNombre = oferta.Titulo;
                reserva.FechaInicio = oferta.FechaInicio;
                reserva.FechaFin = oferta.FechaFin;
                reserva.CantidadAdultos = 1;
                
                _logger.LogInformation("✅ GET Create - Oferta cargada: {Titulo}, ItemNombre: {ItemNombre}, ItemId: {ItemId}", 
                    oferta.Titulo, reserva.ItemNombre, reserva.ItemId);
            }

            return View(reserva);
        }

        // POST: Reservas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TipoReserva,ItemId,ItemNombre,FechaInicio,FechaFin,CantidadAdultos,CantidadNinos,CantidadSeniors,Comentarios")] Reserva reserva)
        {
            _logger.LogInformation("🔍 POST Create - Datos recibidos: TipoReserva={Tipo}, ItemId={ItemId}, ItemNombre='{ItemNombre}'", 
                reserva.TipoReserva, reserva.ItemId, reserva.ItemNombre ?? "NULL");

            // Remover validación de campos calculados
            ModelState.Remove("UsuarioId");
            ModelState.Remove("PrecioTotal");
            ModelState.Remove("PrecioBase");
            ModelState.Remove("CargoPersonasExtra");
            ModelState.Remove("DescuentoAplicado");
            ModelState.Remove("Estado");
            ModelState.Remove("FechaCreacion");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("⚠️ ModelState inválido:");
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        _logger.LogWarning("   Error en {Key}: {ErrorMessage}", 
                            modelState.Key, error.ErrorMessage);
                    }
                }
                
                await RecargarInformacionItem(reserva);
                return View(reserva);
            }

            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("Usuario no autenticado");
                    TempData["ErrorMessage"] = "Debes iniciar sesión para hacer una reserva.";
                    return RedirectToAction("Login", "Account");
                }

                // Asignar campos básicos
                reserva.UsuarioId = userId;
                reserva.FechaCreacion = DateTime.Now;
                reserva.Estado = "Pendiente";

                if (string.IsNullOrEmpty(reserva.ItemNombre))
                {
                    _logger.LogError("ItemNombre está vacío");
                    ModelState.AddModelError(string.Empty, "El nombre del item no puede estar vacío.");
                    await RecargarInformacionItem(reserva);
                    return View(reserva);
                }

                var totalPersonas = reserva.CantidadPersonas;

                if (reserva.TipoReserva == "Hospedaje")
                {
                    var hospedaje = await _hospedajeRepository.GetByIdAsync(reserva.ItemId);
                    if (hospedaje == null)
                    {
                        _logger.LogError("Hospedaje no encontrado: {Id}", reserva.ItemId);
                        ModelState.AddModelError(string.Empty, "Hospedaje no encontrado.");
                        await RecargarInformacionItem(reserva);
                        return View(reserva);
                    }

                    // Validar capacidad
                    if (totalPersonas > hospedaje.CapacidadMaxima)
                    {
                        ModelState.AddModelError(string.Empty, 
                            $"El hospedaje tiene capacidad máxima de {hospedaje.CapacidadMaxima} personas. Has seleccionado {totalPersonas}.");
                        await RecargarInformacionItem(reserva);
                        return View(reserva);
                    }

                    // Validar restricciones
                    var (restrictionesValidas, mensajeRestriccion) = await ValidarRestriccionesHospedaje(hospedaje, reserva);
                    if (!restrictionesValidas)
                    {
                        ModelState.AddModelError(string.Empty, mensajeRestriccion);
                        await RecargarInformacionItem(reserva);
                        return View(reserva);
                    }

                    // ✅ SEMANA 5: Usar repositorio para validar disponibilidad
                    bool disponible = await _hospedajeRepository.VerificarDisponibilidadAsync(
                        hospedaje.Id, 
                        reserva.FechaInicio, 
                        reserva.FechaFin);
                        
                    if (!disponible)
                    {
                        ModelState.AddModelError(string.Empty, 
                            $"El hospedaje NO está disponible para estas fechas.");
                        await RecargarInformacionItem(reserva);
                        return View(reserva);
                    }

                    if (string.IsNullOrEmpty(reserva.ItemNombre))
                    {
                        reserva.ItemNombre = hospedaje.Nombre;
                    }

                    var resultadoCalculo = await _precioService.CalcularPrecioHospedaje(
                        hospedaje, 
                        reserva.FechaInicio, 
                        reserva.FechaFin,
                        reserva.CantidadAdultos,
                        reserva.CantidadNinos,
                        reserva.CantidadSeniors);

                    reserva.PrecioBase = resultadoCalculo.PrecioBase;
                    reserva.CargoPersonasExtra = resultadoCalculo.CargoPersonasExtra;
                    reserva.DescuentoAplicado = resultadoCalculo.DescuentoPromocional;
                    reserva.PrecioTotal = resultadoCalculo.Total;

                    _logger.LogInformation("Precio calculado: {Desglose}", resultadoCalculo.Desglose);
                }
                else if (reserva.TipoReserva == "Tour")
                {
                    var tour = await _tourRepository.GetByIdAsync(reserva.ItemId);
                    if (tour == null)
                    {
                        _logger.LogError("Tour no encontrado: {Id}", reserva.ItemId);
                        ModelState.AddModelError(string.Empty, "Tour no encontrado.");
                        await RecargarInformacionItem(reserva);
                        return View(reserva);
                    }

                    // Validar cupos disponibles
                    if (totalPersonas > tour.CuposDisponibles)
                    {
                        ModelState.AddModelError(string.Empty, 
                            $"El tour solo tiene {tour.CuposDisponibles} cupos disponibles. Has seleccionado {totalPersonas} personas.");
                        await RecargarInformacionItem(reserva);
                        return View(reserva);
                    }

                    // Validar restricciones
                    var (restrictionesValidas, mensajeRestriccion) = await ValidarRestriccionesTour(tour, reserva);
                    if (!restrictionesValidas)
                    {
                        ModelState.AddModelError(string.Empty, mensajeRestriccion);
                        await RecargarInformacionItem(reserva);
                        return View(reserva);
                    }

                    if (string.IsNullOrEmpty(reserva.ItemNombre))
                    {
                        reserva.ItemNombre = tour.Nombre;
                    }

                    reserva.RequiereConfirmacion = tour.RequiereConfirmacion;

                    var resultadoCalculo = await _precioService.CalcularPrecioTour(
                        tour,
                        reserva.CantidadAdultos,
                        reserva.CantidadNinos,
                        reserva.CantidadSeniors);

                    reserva.PrecioBase = resultadoCalculo.PrecioBase;
                    reserva.CargoPersonasExtra = 0;
                    reserva.DescuentoAplicado = resultadoCalculo.DescuentoGrupo;
                    reserva.PrecioTotal = resultadoCalculo.Total;

                    _logger.LogInformation("Precio calculado: {Desglose}", resultadoCalculo.Desglose);

                    // ✅ SEMANA 5: Reservar cupos usando repositorio
                    await _tourRepository.ReservarCuposAsync(tour.Id, totalPersonas);
                }
                else if (reserva.TipoReserva == "Oferta")
                {
                    var oferta = await _ofertaRepository.GetByIdAsync(reserva.ItemId);
                    if (oferta == null)
                    {
                        _logger.LogError("Oferta no encontrada: {Id}", reserva.ItemId);
                        ModelState.AddModelError(string.Empty, "Oferta no encontrada.");
                        await RecargarInformacionItem(reserva);
                        return View(reserva);
                    }

                    // Validar que la oferta siga siendo válida
                    if (DateTime.Now > oferta.FechaFin)
                    {
                        ModelState.AddModelError(string.Empty, 
                            $"La oferta expiró el {oferta.FechaFin:dd/MM/yyyy}.");
                        await RecargarInformacionItem(reserva);
                        return View(reserva);
                    }

                    if (string.IsNullOrEmpty(reserva.ItemNombre))
                    {
                        reserva.ItemNombre = oferta.Titulo;
                    }

                    reserva.PrecioBase = oferta.Precio;
                    reserva.CargoPersonasExtra = 0;
                    reserva.DescuentoAplicado = 0;
                    reserva.PrecioTotal = oferta.Precio;

                    _logger.LogInformation("Oferta seleccionada: {Titulo}, Precio: {Precio}", 
                        oferta.Titulo, oferta.Precio);
                }

                // ✅ SEMANA 5: Guardar usando repositorio
                await _reservaRepository.AddAsync(reserva);
                await _reservaRepository.SaveChangesAsync();

                // ✅ Limpiar cache del usuario
                _cacheService.RemoveByPattern($"reservas_user_{userId}");

                _logger.LogInformation("Reserva creada exitosamente: ID {Id}, Total: {Total}", 
                    reserva.Id, reserva.PrecioTotal);

                // 👇 ENVIAR NOTIFICACIÓN AL CLIENTE
                var user = await _userManager.GetUserAsync(User);
                if (user?.Email != null)
                {
                    await _notificacionService.EnviarConfirmacionReserva(reserva, user.Email);
                }

                // 👇 NOTIFICAR A ADMIN SI REQUIERE CONFIRMACIÓN
                if (reserva.RequiereConfirmacion)
                {
                    await _notificacionService.NotificarAdminNuevaReserva(reserva);
                }
                
                string mensaje = $"¡Reserva creada exitosamente! Total: {reserva.PrecioTotal:C} para {totalPersonas} persona(s)";
                if (reserva.RequiereConfirmacion)
                {
                    mensaje += " (Requiere confirmación del administrador)";
                }
                TempData["SuccessMessage"] = mensaje;
                
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                _logger.LogError(dbEx, "Error de base de datos al crear reserva. InnerException: {InnerMessage}", innerMessage);
                ModelState.AddModelError(string.Empty, $"Error de base de datos: {innerMessage}");
                await RecargarInformacionItem(reserva);
                return View(reserva);
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                _logger.LogError(ex, "Error al crear reserva. InnerException: {InnerMessage}", innerMessage);
                ModelState.AddModelError(string.Empty, $"Error: {innerMessage}");
                await RecargarInformacionItem(reserva);
                return View(reserva);
            }
        }

        // Método auxiliar para recargar información del item
        private async Task RecargarInformacionItem(Reserva reserva)
        {
            // ✅ SEMANA 5: Usar repositorios
            if (reserva.TipoReserva == "Hospedaje")
            {
                var hospedaje = await _hospedajeRepository.GetByIdAsync(reserva.ItemId);
                if (hospedaje != null)
                {
                    ViewBag.PrecioPorNoche = hospedaje.PrecioPorNoche;
                    ViewBag.CapacidadMaxima = hospedaje.CapacidadMaxima;
                    ViewBag.PersonasIncluidas = hospedaje.PersonasIncluidasEnPrecio;
                    ViewBag.CargoPorPersonaExtra = hospedaje.CargoPorPersonaExtra;
                    ViewBag.ItemInfo = $"{hospedaje.Nombre} - {hospedaje.Ubicacion} ({hospedaje.InfoCapacidad})";
                }
            }
            else if (reserva.TipoReserva == "Tour")
            {
                var tour = await _tourRepository.GetByIdAsync(reserva.ItemId);
                if (tour != null)
                {
                    ViewBag.PrecioAdulto = tour.Precio;
                    ViewBag.PrecioNino = tour.PrecioNino;
                    ViewBag.PrecioSenior = tour.PrecioSenior;
                    ViewBag.CuposDisponibles = tour.CuposDisponibles;
                    ViewBag.DescuentoGrupo = tour.DescuentoGrupo;
                    ViewBag.ItemInfo = $"{tour.Nombre} - {tour.Ubicacion} ({tour.EstadoDisponibilidad})";
                }
            }
            else if (reserva.TipoReserva == "Oferta")
            {
                var oferta = await _ofertaRepository.GetByIdAsync(reserva.ItemId);
                if (oferta != null)
                {
                    ViewBag.PrecioOferta = oferta.Precio;
                    ViewBag.ItemInfo = $"{oferta.Titulo} (Válido hasta {oferta.FechaFin:dd/MM/yyyy})";
                }
            }
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // ✅ SEMANA 5: Usar repositorio
            var reserva = await _reservaRepository.GetByIdAsync(id.Value);
            if (reserva == null) return NotFound();

            // Verificar que el usuario sea dueño o admin
            var userId = _userManager.GetUserId(User);
            if (reserva.UsuarioId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(reserva);
        }

        // POST: Reservas/CambiarEstado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, string nuevoEstado)
        {
            // ✅ SEMANA 5: Usar repositorio
            var reserva = await _reservaRepository.GetByIdAsync(id);
            if (reserva == null) return NotFound();

            // Solo admin o el usuario dueño pueden cambiar estado
            var userId = _userManager.GetUserId(User);
            var esAdmin = User.IsInRole("Admin");
            var esDueno = reserva.UsuarioId == userId;

            if (!esAdmin && !esDueno)
            {
                return Forbid();
            }

            // Los clientes solo pueden cancelar
            if (!esAdmin && nuevoEstado != "Cancelada")
            {
                TempData["ErrorMessage"] = "Solo puedes cancelar tus reservas.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var estadoAnterior = reserva.Estado;
                reserva.EstadoAnterior = estadoAnterior;
                reserva.FechaCambioEstado = DateTime.Now;
                reserva.ModificadoPor = userId;

                // ✅ SEMANA 5: Liberar cupos usando repositorio
                if (nuevoEstado == "Cancelada" && reserva.TipoReserva == "Tour")
                {
                    if (reserva.Estado == "Pendiente" || reserva.Estado == "Confirmada")
                    {
                        await _tourRepository.LiberarCuposAsync(reserva.ItemId, reserva.CantidadPersonas);
                        
                        _logger.LogInformation("Cupos devueltos - Tour: {TourId}, Personas: {Personas}",
                            reserva.ItemId, reserva.CantidadPersonas);
                    }
                }

                reserva.Estado = nuevoEstado;
                reserva.FechaModificacion = DateTime.Now;

                // ✅ SEMANA 5: Actualizar usando repositorio
                await _reservaRepository.UpdateAsync(reserva);
                await _reservaRepository.SaveChangesAsync();

                // ✅ Limpiar cache
                _cacheService.RemoveByPattern($"reservas_user_{reserva.UsuarioId}");

                _logger.LogInformation("Estado de reserva {Id} cambiado de {EstadoAnterior} a {EstadoNuevo}", 
                    id, estadoAnterior, nuevoEstado);

                var user = await _userManager.FindByIdAsync(reserva.UsuarioId);
                if (user?.Email != null)
                {
                    await _notificacionService.EnviarCambioEstado(reserva, estadoAnterior, user.Email);
                }

                TempData["SuccessMessage"] = $"Estado actualizado a: {nuevoEstado}";
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado de reserva {Id}", id);
                TempData["ErrorMessage"] = $"Error al actualizar estado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // ✅ SEMANA 5: Usar repositorio
            var reserva = await _reservaRepository.GetByIdAsync(id);
            if (reserva != null)
            {
                await _reservaRepository.DeleteAsync(reserva);
                await _reservaRepository.SaveChangesAsync();
                
                // Limpiar cache
                _cacheService.RemoveByPattern($"reservas_user_{reserva.UsuarioId}");
                
                TempData["SuccessMessage"] = "Reserva eliminada exitosamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        // Métodos auxiliares para validación (mantener como están)
        private Task<(bool valido, string mensaje)> ValidarRestriccionesHospedaje(Hospedaje hospedaje, Reserva reserva)
        {
            if (reserva.CantidadNinos > 0 && !hospedaje.PermiteNinos)
            {
                return Task.FromResult((false, "Este hospedaje NO permite niños. Has indicado que asistirán niños."));
            }
            
            return Task.FromResult((true, ""));
        }

        private Task<(bool valido, string mensaje)> ValidarRestriccionesTour(Tour tour, Reserva reserva)
        {
            if (reserva.CantidadNinos > 0 && tour.EdadMinima > 12)
            {
                return Task.FromResult((false, $"Este tour requiere edad mínima de {tour.EdadMinima} años. Los niños no pueden participar."));
            }

            if (reserva.CantidadSeniors > 0 && tour.EdadMaxima.HasValue && tour.EdadMaxima < 65)
            {
                return Task.FromResult((false, $"Este tour tiene edad máxima recomendada de {tour.EdadMaxima} años."));
            }

            return Task.FromResult((true, ""));
        }
    }
}
