using BlueTravel.Data;
using BlueTravel.Models;
using BlueTravel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlueTravel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;
        private readonly IReporteService _reporteService;
        private readonly IDashboardService _dashboardService; // ? SEMANA 5: NUEVO

        public AdminController(
            ApplicationDbContext context, 
            ILogger<AdminController> logger,
            IReporteService reporteService,
            IDashboardService dashboardService) // ? SEMANA 5: NUEVO
        {
            _context = context;
            _logger = logger;
            _reporteService = reporteService;
            _dashboardService = dashboardService; // ? SEMANA 5: NUEVO
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                // ? SEMANA 5: Usar servicio de dashboard mejorado
                var stats = await _dashboardService.GetStatsAsync();
                
                _logger.LogInformation("Dashboard cargado: {TotalReservas} reservas", stats.TotalReservas);
                
                return View(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar dashboard");
                TempData["ErrorMessage"] = "Error al cargar estadísticas del dashboard";
                return View(new BlueTravel.Models.DashboardStats());
            }
        }

        // GET: Admin/Reportes
        public IActionResult Reportes()
        {
            return View();
        }
    }
}
