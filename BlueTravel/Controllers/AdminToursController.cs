using BlueTravel.Data.Repositories;
using BlueTravel.Models;
using BlueTravel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueTravel.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Tours")]
    public class AdminToursController : Controller
    {
        private readonly ITourRepository _repository;
        private readonly ILogger<AdminToursController> _logger;
        private readonly ICacheService _cacheService;

        public AdminToursController(
            ITourRepository repository,
            ILogger<AdminToursController> logger,
            ICacheService cacheService)
        {
            _repository = repository;
            _logger = logger;
            _cacheService = cacheService;
        }

        // GET: Admin/Tours
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var tours = await _repository.GetAllAsync();
            return View(tours.OrderBy(t => t.Nombre));
        }

        // GET: Admin/Tours/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View(new Tour 
            { 
                FechaDisponible = DateTime.Now.AddDays(7),
                CupoMaximo = 20,
                Duracion = 1
            });
        }

        // POST: Admin/Tours/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tour tour)
        {
            if (!ModelState.IsValid)
            {
                return View(tour);
            }

            try
            {
                tour.CuposReservados = 0;
                await _repository.AddAsync(tour);
                await _repository.SaveChangesAsync();

                // ?? INVALIDAR TODO EL CACHÉ DE TOURS (todas las páginas)
                _cacheService.RemoveByPattern("tours_all");
                _cacheService.RemoveByPattern("details_tour_");

                TempData["SuccessMessage"] = $"Tour '{tour.Nombre}' creado exitosamente.";
                _logger.LogInformation("Admin creó tour: {Nombre}", tour.Nombre);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear tour");
                ModelState.AddModelError("", "Error al crear el tour.");
                return View(tour);
            }
        }

        // GET: Admin/Tours/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var tour = await _repository.GetByIdAsync(id);
            if (tour == null)
            {
                TempData["ErrorMessage"] = "Tour no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(tour);
        }

        // POST: Admin/Tours/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tour tour)
        {
            if (id != tour.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(tour);
            }

            try
            {
                await _repository.UpdateAsync(tour);
                await _repository.SaveChangesAsync();

                // ?? INVALIDAR TODO EL CACHÉ DE TOURS
                _cacheService.RemoveByPattern("tours_all");
                _cacheService.RemoveByPattern($"details_tour_{id}");

                TempData["SuccessMessage"] = $"Tour '{tour.Nombre}' actualizado exitosamente.";
                _logger.LogInformation("Admin actualizó tour: {Id}", id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar tour {Id}", id);
                ModelState.AddModelError("", "Error al actualizar el tour.");
                return View(tour);
            }
        }

        // POST: Admin/Tours/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var tour = await _repository.GetByIdAsync(id);
                if (tour == null)
                {
                    TempData["ErrorMessage"] = "Tour no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                await _repository.DeleteAsync(tour);
                await _repository.SaveChangesAsync();

                // ?? INVALIDAR TODO EL CACHÉ DE TOURS
                _cacheService.RemoveByPattern("tours_all");
                _cacheService.RemoveByPattern($"details_tour_{id}");

                TempData["SuccessMessage"] = $"Tour '{tour.Nombre}' eliminado exitosamente.";
                _logger.LogInformation("Admin eliminó tour: {Id}", id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar tour {Id}", id);
                TempData["ErrorMessage"] = "Error al eliminar. Puede tener reservas asociadas.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
