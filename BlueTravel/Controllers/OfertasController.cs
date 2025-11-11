using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlueTravel.Data;
using BlueTravel.Models;

namespace BlueTravel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OfertasController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OfertasController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index() => View(await _context.Ofertas.ToListAsync());
        public async Task<IActionResult> Details(int? id) { if (id == null) return NotFound(); var item = await _context.Ofertas.FirstOrDefaultAsync(m => m.Id == id); if (item == null) return NotFound(); return View(item); }
        public IActionResult Create() => View();
        [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Create(Oferta model) { if (!ModelState.IsValid) return View(model); _context.Add(model); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        public async Task<IActionResult> Edit(int? id) { if (id == null) return NotFound(); var item = await _context.Ofertas.FindAsync(id); if (item == null) return NotFound(); return View(item); }
        [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Edit(int id, Oferta model) { if (id != model.Id) return NotFound(); if (!ModelState.IsValid) return View(model); _context.Update(model); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        public async Task<IActionResult> Delete(int? id) { if (id == null) return NotFound(); var item = await _context.Ofertas.FirstOrDefaultAsync(m => m.Id == id); if (item == null) return NotFound(); return View(item); }
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken] public async Task<IActionResult> DeleteConfirmed(int id) { var item = await _context.Ofertas.FindAsync(id); if (item != null) { _context.Ofertas.Remove(item); await _context.SaveChangesAsync(); } return RedirectToAction(nameof(Index)); }
    }
}