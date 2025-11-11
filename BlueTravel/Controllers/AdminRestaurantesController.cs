using BlueTravel.Data;
using BlueTravel.Models;
using BlueTravel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlueTravel.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Restaurantes")]
    public class AdminRestaurantesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminRestaurantesController> _logger;
        private readonly ICacheService _cacheService;

        public AdminRestaurantesController(
            ApplicationDbContext context,
            ILogger<AdminRestaurantesController> logger,
            ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }

        // GET: Admin/Restaurantes
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var restaurantes = await _context.Restaurantes
                .OrderBy(r => r.Nombre)
                .ToListAsync();
            return View(restaurantes);
        }

        // GET: Admin/Restaurantes/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Restaurantes/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Restaurante restaurante)
        {
            if (!ModelState.IsValid)
            {
                return View(restaurante);
            }

            try
            {
                _context.Restaurantes.Add(restaurante);
                await _context.SaveChangesAsync();

                // ?? INVALIDAR CACHÉ DE RESTAURANTES
                _cacheService.Remove("restaurantes_all");
                _cacheService.RemoveByPattern("details_restaurante_");

                TempData["SuccessMessage"] = $"Restaurante '{restaurante.Nombre}' creado exitosamente.";
                _logger.LogInformation("Admin creó restaurante: {Nombre}", restaurante.Nombre);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear restaurante");
                ModelState.AddModelError("", "Error al crear el restaurante.");
                return View(restaurante);
            }
        }

        // GET: Admin/Restaurantes/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var restaurante = await _context.Restaurantes.FindAsync(id);
            if (restaurante == null)
            {
                TempData["ErrorMessage"] = "Restaurante no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(restaurante);
        }

        // POST: Admin/Restaurantes/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Restaurante restaurante)
        {
            if (id != restaurante.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(restaurante);
            }

            try
            {
                _context.Restaurantes.Update(restaurante);
                await _context.SaveChangesAsync();

                // ?? INVALIDAR CACHÉ
                _cacheService.Remove("restaurantes_all");
                _cacheService.RemoveByPattern($"details_restaurante_{id}");

                TempData["SuccessMessage"] = $"Restaurante '{restaurante.Nombre}' actualizado exitosamente.";
                _logger.LogInformation("Admin actualizó restaurante: {Id}", id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar restaurante {Id}", id);
                ModelState.AddModelError("", "Error al actualizar el restaurante.");
                return View(restaurante);
            }
        }

        // POST: Admin/Restaurantes/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var restaurante = await _context.Restaurantes.FindAsync(id);
                if (restaurante == null)
                {
                    TempData["ErrorMessage"] = "Restaurante no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Restaurantes.Remove(restaurante);
                await _context.SaveChangesAsync();

                // ?? INVALIDAR CACHÉ
                _cacheService.Remove("restaurantes_all");
                _cacheService.RemoveByPattern($"details_restaurante_{id}");

                TempData["SuccessMessage"] = $"Restaurante '{restaurante.Nombre}' eliminado exitosamente.";
                _logger.LogInformation("Admin eliminó restaurante: {Id}", id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar restaurante {Id}", id);
                TempData["ErrorMessage"] = "Error al eliminar el restaurante.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
