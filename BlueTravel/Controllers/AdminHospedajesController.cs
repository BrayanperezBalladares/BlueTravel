using BlueTravel.Data.Repositories;
using BlueTravel.Models;
using BlueTravel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueTravel.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Hospedajes")]
    public class AdminHospedajesController : Controller
    {
        private readonly IHospedajeRepository _repository;
        private readonly ILogger<AdminHospedajesController> _logger;
        private readonly ICacheService _cacheService;

        public AdminHospedajesController(
            IHospedajeRepository repository,
            ILogger<AdminHospedajesController> logger,
            ICacheService cacheService)
        {
            _repository = repository;
            _logger = logger;
            _cacheService = cacheService;
        }

        // GET: Admin/Hospedajes
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var hospedajes = await _repository.GetAllAsync();
            return View(hospedajes.OrderBy(h => h.Nombre));
        }

        // GET: Admin/Hospedajes/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Hospedajes/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hospedaje hospedaje)
        {
            if (!ModelState.IsValid)
            {
                return View(hospedaje);
            }

            try
            {
                await _repository.AddAsync(hospedaje);
                await _repository.SaveChangesAsync();

                // ?? INVALIDAR TODO EL CACHÉ DE HOSPEDAJES (todas las páginas)
                _cacheService.RemoveByPattern("hospedajes_all");
                _cacheService.RemoveByPattern("details_hospedaje_");

                TempData["SuccessMessage"] = $"Hospedaje '{hospedaje.Nombre}' creado exitosamente.";
                _logger.LogInformation("Admin creó hospedaje: {Nombre}", hospedaje.Nombre);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear hospedaje");
                ModelState.AddModelError("", "Error al crear el hospedaje. Intenta nuevamente.");
                return View(hospedaje);
            }
        }

        // GET: Admin/Hospedajes/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var hospedaje = await _repository.GetByIdAsync(id);
            if (hospedaje == null)
            {
                TempData["ErrorMessage"] = "Hospedaje no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(hospedaje);
        }

        // POST: Admin/Hospedajes/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Hospedaje hospedaje)
        {
            if (id != hospedaje.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(hospedaje);
            }

            try
            {
                await _repository.UpdateAsync(hospedaje);
                await _repository.SaveChangesAsync();

                // ?? INVALIDAR TODO EL CACHÉ DE HOSPEDAJES
                _cacheService.RemoveByPattern("hospedajes_all");
                _cacheService.RemoveByPattern($"details_hospedaje_{id}");

                TempData["SuccessMessage"] = $"Hospedaje '{hospedaje.Nombre}' actualizado exitosamente.";
                _logger.LogInformation("Admin actualizó hospedaje: {Id}", id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar hospedaje {Id}", id);
                ModelState.AddModelError("", "Error al actualizar el hospedaje.");
                return View(hospedaje);
            }
        }

        // POST: Admin/Hospedajes/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var hospedaje = await _repository.GetByIdAsync(id);
                if (hospedaje == null)
                {
                    TempData["ErrorMessage"] = "Hospedaje no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                await _repository.DeleteAsync(hospedaje);
                await _repository.SaveChangesAsync();

                // ?? INVALIDAR TODO EL CACHÉ DE HOSPEDAJES
                _cacheService.RemoveByPattern("hospedajes_all");
                _cacheService.RemoveByPattern($"details_hospedaje_{id}");

                TempData["SuccessMessage"] = $"Hospedaje '{hospedaje.Nombre}' eliminado exitosamente.";
                _logger.LogInformation("Admin eliminó hospedaje: {Id}", id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar hospedaje {Id}", id);
                TempData["ErrorMessage"] = "Error al eliminar. Puede tener reservas asociadas.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
