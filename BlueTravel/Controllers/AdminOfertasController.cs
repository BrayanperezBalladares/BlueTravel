using BlueTravel.Data.Repositories;
using BlueTravel.Models;
using BlueTravel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueTravel.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Ofertas")]
    public class AdminOfertasController : Controller
    {
        private readonly IOfertaRepository _repository;
        private readonly ILogger<AdminOfertasController> _logger;
        private readonly ICacheService _cacheService;

        public AdminOfertasController(
            IOfertaRepository repository,
            ILogger<AdminOfertasController> logger,
            ICacheService cacheService)
        {
            _repository = repository;
            _logger = logger;
            _cacheService = cacheService;
        }

        // GET: Admin/Ofertas
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var ofertas = await _repository.GetAllAsync();
            return View(ofertas.OrderByDescending(o => o.FechaInicio));
        }

        // GET: Admin/Ofertas/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View(new Oferta
            {
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddMonths(1)
            });
        }

        // POST: Admin/Ofertas/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Oferta oferta)
        {
            if (!ModelState.IsValid)
            {
                return View(oferta);
            }

            // Validación adicional
            if (oferta.FechaFin <= oferta.FechaInicio)
            {
                ModelState.AddModelError("FechaFin", "La fecha de fin debe ser posterior a la de inicio.");
                return View(oferta);
            }

            try
            {
                await _repository.AddAsync(oferta);
                await _repository.SaveChangesAsync();

                // ?? INVALIDAR TODO EL CACHÉ DE OFERTAS
                _cacheService.RemoveByPattern("ofertas_activas");
                _cacheService.RemoveByPattern("details_oferta_");

                TempData["SuccessMessage"] = $"Oferta '{oferta.Titulo}' creada exitosamente.";
                _logger.LogInformation("Admin creó oferta: {Titulo}", oferta.Titulo);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear oferta");
                ModelState.AddModelError("", "Error al crear la oferta.");
                return View(oferta);
            }
        }

        // GET: Admin/Ofertas/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var oferta = await _repository.GetByIdAsync(id);
            if (oferta == null)
            {
                TempData["ErrorMessage"] = "Oferta no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            return View(oferta);
        }

        // POST: Admin/Ofertas/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Oferta oferta)
        {
            if (id != oferta.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(oferta);
            }

            try
            {
                await _repository.UpdateAsync(oferta);
                await _repository.SaveChangesAsync();

                // ?? INVALIDAR TODO EL CACHÉ DE OFERTAS
                _cacheService.RemoveByPattern("ofertas_activas");
                _cacheService.RemoveByPattern($"details_oferta_{id}");

                TempData["SuccessMessage"] = $"Oferta '{oferta.Titulo}' actualizada exitosamente.";
                _logger.LogInformation("Admin actualizó oferta: {Id}", id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar oferta {Id}", id);
                ModelState.AddModelError("", "Error al actualizar la oferta.");
                return View(oferta);
            }
        }

        // POST: Admin/Ofertas/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var oferta = await _repository.GetByIdAsync(id);
                if (oferta == null)
                {
                    TempData["ErrorMessage"] = "Oferta no encontrada.";
                    return RedirectToAction(nameof(Index));
                }

                await _repository.DeleteAsync(oferta);
                await _repository.SaveChangesAsync();

                // ?? INVALIDAR TODO EL CACHÉ DE OFERTAS
                _cacheService.RemoveByPattern("ofertas_activas");
                _cacheService.RemoveByPattern($"details_oferta_{id}");

                TempData["SuccessMessage"] = $"Oferta '{oferta.Titulo}' eliminada exitosamente.";
                _logger.LogInformation("Admin eliminó oferta: {Id}", id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar oferta {Id}", id);
                TempData["ErrorMessage"] = "Error al eliminar. Puede tener reservas asociadas.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
