using GuildaAventureros.Data;
using GuildaAventureros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GuildaAventureros.Controllers;

public class AventurerosController : Controller
{
    private readonly GuildaDbContext _db;

    public AventurerosController(GuildaDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var lista = await _db.Aventureros
            .Include(a => a.Especialidad)
            .OrderBy(a => a.Nombre)
            .AsNoTracking()
            .ToListAsync();
        return View(lista);
    }
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();
        var aventurero = await _db.Aventureros
            .Include(a => a.Especialidad)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.IdAventurero == id);
        return aventurero is null ? NotFound() : View(aventurero);
    }
    public async Task<IActionResult> Create()
    {
        await CargarEspecialidades();
        return View(new Aventurero { Nivel = 1, Estado = true });
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombre,IdEspecialidad,Nivel,Estado")] Aventurero aventurero)
    {
        await ValidarEspecialidad(aventurero.IdEspecialidad);

        if (!ModelState.IsValid)
        {
            await CargarEspecialidades(aventurero.IdEspecialidad);
            return View(aventurero);
        }

        aventurero.FechaRegistro = DateTime.Now;
        _db.Add(aventurero);
        await _db.SaveChangesAsync();
        TempData["Mensaje"] = "Aventurero registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        var aventurero = await _db.Aventureros.FindAsync(id);
        if (aventurero is null) return NotFound();

        await CargarEspecialidades(aventurero.IdEspecialidad);
        return View(aventurero);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("IdAventurero,Nombre,IdEspecialidad,Nivel,Estado")] Aventurero modelo)
    {
        if (id != modelo.IdAventurero) return NotFound();

        await ValidarEspecialidad(modelo.IdEspecialidad);

        if (!ModelState.IsValid)
        {
            await CargarEspecialidades(modelo.IdEspecialidad);
            return View(modelo);
        }

        var aventurero = await _db.Aventureros.FindAsync(id);
        if (aventurero is null) return NotFound();

        aventurero.Nombre = modelo.Nombre;
        aventurero.IdEspecialidad = modelo.IdEspecialidad;
        aventurero.Nivel = modelo.Nivel;
        aventurero.Estado = modelo.Estado;

        await _db.SaveChangesAsync();
        TempData["Mensaje"] = "Aventurero actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        var aventurero = await _db.Aventureros
            .Include(a => a.Especialidad)
            .Include(a => a.Usuario)
            .Include(a => a.Asignaciones)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.IdAventurero == id);
        return aventurero is null ? NotFound() : View(aventurero);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var aventurero = await _db.Aventureros.FindAsync(id);
        if (aventurero is null) return RedirectToAction(nameof(Index));

        if (await _db.Usuarios.AnyAsync(u => u.IdAventurero == id))
        {
            TempData["Error"] = "No se puede eliminar: el aventurero tiene una cuenta de usuario asociada.";
            return RedirectToAction(nameof(Index));
        }

        if (await _db.Asignaciones.AnyAsync(x => x.IdAventurero == id))
        {
            TempData["Error"] = "No se puede eliminar: el aventurero tiene asignaciones de misiones registradas.";
            return RedirectToAction(nameof(Index));
        }

        _db.Aventureros.Remove(aventurero);
        await _db.SaveChangesAsync();
        TempData["Mensaje"] = "Aventurero eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarEspecialidades(int? seleccionada = null)
    {
        var especialidades = await _db.Especialidades.AsNoTracking().OrderBy(e => e.Nombre).ToListAsync();
        ViewBag.Especialidades = new SelectList(especialidades, "IdEspecialidad", "Nombre", seleccionada);
    }

    private async Task ValidarEspecialidad(int idEspecialidad)
    {
        if (idEspecialidad > 0 && !await _db.Especialidades.AnyAsync(e => e.IdEspecialidad == idEspecialidad))
            ModelState.AddModelError(nameof(Aventurero.IdEspecialidad), "Debe seleccionar una especialidad válida.");
    }
}