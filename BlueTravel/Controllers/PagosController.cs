using BlueTravel.Data;
using BlueTravel.Models;
using BlueTravel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlueTravel.Controllers
{
    [Authorize]
    public class PagosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<PagosController> _logger;
        private readonly INotificacionService _notificacionService;
        private readonly IStripeService _stripeService; // 👈 NUEVO

        public PagosController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            ILogger<PagosController> logger,
            INotificacionService notificacionService,
            IStripeService stripeService) // 👈 NUEVO
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _notificacionService = notificacionService;
            _stripeService = stripeService; // 👈 NUEVO
        }

        // GET: Pagos - Mis Pagos
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            if (User.IsInRole("Admin"))
            {
                // Admin redirige a AdminIndex
                return RedirectToAction(nameof(AdminIndex));
            }
            else
            {
                var misPagos = await _context.Pagos
                    .Include(p => p.Reserva)
                    .Where(p => p.UsuarioId == userId)
                    .OrderByDescending(p => p.FechaCreacion)
                    .ToListAsync();
                return View(misPagos);
            }
        }

        // GET: Pagos/AdminIndex - Vista de administrador
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex()
        {
            var todosPagos = await _context.Pagos
                .Include(p => p.Reserva)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();
            return View(todosPagos);
        }

        // GET: Pagos/Create?reservaId=5
        public async Task<IActionResult> Create(int? reservaId)
        {
            if (reservaId == null)
            {
                TempData["ErrorMessage"] = "Debes seleccionar una reserva para pagar.";
                return RedirectToAction("Index", "Reservas");
            }

            var reserva = await _context.Reservas.FindAsync(reservaId);
            if (reserva == null)
            {
                TempData["ErrorMessage"] = "Reserva no encontrada.";
                return RedirectToAction("Index", "Reservas");
            }

            var userId = _userManager.GetUserId(User);
            if (reserva.UsuarioId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (reserva.EstaPagada)
            {
                TempData["InfoMessage"] = "Esta reserva ya está pagada.";
                return RedirectToAction("Details", "Reservas", new { id = reservaId });
            }

            var pago = new Pago
            {
                ReservaId = reserva.Id,
                UsuarioId = userId!,
                MontoBase = reserva.PrecioBase,
                Impuestos = reserva.PrecioBase * 0.13m,
                Descuentos = reserva.DescuentoAplicado,
                CargosAdicionales = reserva.CargoPersonasExtra,
                TotalPagado = reserva.PrecioTotal,
                Estado = EstadoPago.Pendiente,
                Metodo = MetodoPago.TarjetaCredito
            };

            ViewBag.Reserva = reserva;
            return View(pago);
        }

        // POST: Pagos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservaId,Metodo,UltimosDigitosTarjeta,MarcaTarjeta,ReferenciaBancaria")] Pago pago)
        {
            var reserva = await _context.Reservas.FindAsync(pago.ReservaId);
            if (reserva == null)
            {
                TempData["ErrorMessage"] = "Reserva no encontrada.";
                return RedirectToAction("Index", "Reservas");
            }

            var userId = _userManager.GetUserId(User);
            if (reserva.UsuarioId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (reserva.EstaPagada)
            {
                TempData["ErrorMessage"] = "Esta reserva ya está pagada.";
                return RedirectToAction("Details", "Reservas", new { id = pago.ReservaId });
            }

            try
            {
                pago.UsuarioId = userId!;
                pago.MontoBase = reserva.PrecioBase;
                pago.Impuestos = reserva.PrecioBase * 0.13m;
                pago.Descuentos = reserva.DescuentoAplicado;
                pago.CargosAdicionales = reserva.CargoPersonasExtra;
                pago.TotalPagado = reserva.PrecioTotal;
                pago.FechaCreacion = DateTime.Now;
                pago.Estado = EstadoPago.Pendiente;

                var pagoExitoso = await ProcesarPago(pago);

                if (pagoExitoso)
                {
                    pago.Estado = EstadoPago.Aprobado;
                    pago.FechaAprobacion = DateTime.Now;
                    pago.TransaccionExternaId = $"TXN-{Guid.NewGuid().ToString()[..8].ToUpper()}";

                    _context.Pagos.Add(pago);
                    await _context.SaveChangesAsync();

                    reserva.PagoId = pago.Id;
                    reserva.Estado = "Confirmada";
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Pago #{Id} procesado exitosamente", pago.Id);

                    var user = await _userManager.GetUserAsync(User);
                    if (user?.Email != null)
                    {
                        await _notificacionService.EnviarComprobantePago(pago, user.Email);
                    }

                    TempData["SuccessMessage"] = $"¡Pago procesado exitosamente! ID: {pago.TransaccionExternaId}";
                    return RedirectToAction(nameof(Details), new { id = pago.Id });
                }
                else
                {
                    pago.Estado = EstadoPago.Rechazado;
                    _context.Pagos.Add(pago);
                    await _context.SaveChangesAsync();

                    TempData["ErrorMessage"] = "El pago fue rechazado. Intenta nuevamente.";
                    ViewBag.Reserva = reserva;
                    return View(pago);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar pago");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                ViewBag.Reserva = reserva;
                return View(pago);
            }
        }

        // GET: Pagos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var pago = await _context.Pagos
                .Include(p => p.Reserva)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pago == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (pago.UsuarioId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(pago);
        }

        // POST: Pagos/Reembolsar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reembolsar(int id, string motivo)
        {
            var pagoOriginal = await _context.Pagos
                .Include(p => p.Reserva)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pagoOriginal == null) return NotFound();

            if (pagoOriginal.Estado != EstadoPago.Aprobado)
            {
                TempData["ErrorMessage"] = "Solo se pueden reembolsar pagos aprobados.";
                return RedirectToAction(nameof(Details), new { id });
            }

            try
            {
                var reembolso = new Pago
                {
                    ReservaId = pagoOriginal.ReservaId,
                    UsuarioId = pagoOriginal.UsuarioId,
                    Metodo = pagoOriginal.Metodo,
                    MontoBase = -pagoOriginal.MontoBase,
                    Impuestos = -pagoOriginal.Impuestos,
                    TotalPagado = -pagoOriginal.TotalPagado,
                    Estado = EstadoPago.Reembolsado,
                    EsReembolso = true,
                    PagoOriginalId = pagoOriginal.Id,
                    FechaCreacion = DateTime.Now,
                    FechaAprobacion = DateTime.Now,
                    NotasInternas = $"Reembolso. Motivo: {motivo}",
                    TransaccionExternaId = $"REF-{Guid.NewGuid().ToString()[..8].ToUpper()}"
                };

                pagoOriginal.Estado = EstadoPago.Reembolsado;
                if (pagoOriginal.Reserva != null)
                {
                    pagoOriginal.Reserva.Estado = "Cancelada";
                    pagoOriginal.Reserva.MotivoRechazo = $"Reembolso: {motivo}";
                }

                _context.Pagos.Add(reembolso);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Reembolso procesado: {reembolso.TransaccionExternaId}";
                return RedirectToAction(nameof(Details), new { id = reembolso.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar reembolso");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // Método mejorado con Stripe real
        private async Task<bool> ProcesarPago(Pago pago)
        {
            _logger.LogInformation("🔄 Iniciando procesamiento de pago con Stripe...");

            try
            {
                // 1. Crear intención de pago en Stripe
                var resultado = await _stripeService.CrearIntencionPago(
                    pago.TotalPagado, 
                    "usd" // o "crc" para colones si Stripe lo soporta
                );

                if (!resultado.Success)
                {
                    _logger.LogError("❌ Error al crear intención de pago: {Error}", resultado.ErrorMessage);
                    return false;
                }

                // Guardar ID de transacción de Stripe
                pago.TransaccionExternaId = resultado.PaymentIntentId;

                if (resultado.Simulado)
                {
                    _logger.LogInformation("ℹ️ Procesando en modo SIMULACIÓN (Stripe no configurado)");
                    await Task.Delay(1000); // Simular procesamiento
                    
                    // 90% de éxito en simulación
                    var exito = new Random().Next(100) < 90;
                    
                    if (exito)
                    {
                        _logger.LogInformation("✅ Pago simulado APROBADO");
                        pago.NotasInternas = "Pago simulado - Stripe en modo TEST no configurado";
                    }
                    else
                    {
                        _logger.LogWarning("❌ Pago simulado RECHAZADO");
                    }
                    
                    return exito;
                }
                else
                {
                    // 2. Stripe REAL configurado
                    _logger.LogInformation("💳 Procesando con Stripe REAL (modo TEST)");
                    _logger.LogInformation("Payment Intent ID: {Id}", resultado.PaymentIntentId);
                    
                    // En una app real, aquí redirigirías al cliente a completar el pago
                    // con Stripe Elements (formulario de tarjeta)
                    // Por ahora, simulamos que el pago se completó
                    await Task.Delay(1500);
                    
                    // Verificar estado del pago
                    var confirmado = await _stripeService.ConfirmarPago(resultado.PaymentIntentId!);
                    
                    if (confirmado)
                    {
                        _logger.LogInformation("✅ Pago CONFIRMADO por Stripe");
                        pago.NotasInternas = $"Pago procesado con Stripe (TEST). PaymentIntent: {resultado.PaymentIntentId}";
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Pago NO confirmado por Stripe");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error inesperado al procesar pago");
                return false;
            }
        }
    }
}