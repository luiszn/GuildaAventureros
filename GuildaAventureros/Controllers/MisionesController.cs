using GuildaAventureros.Constants;
using GuildaAventureros.Data;
using GuildaAventureros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GuildaAventureros.Controllers;

public class MisionesController : Controller
{
    private readonly GuildaDbContext _db;

    public MisionesController(GuildaDbContext db) => _db = db;

    // READ: listado
    public async Task<IActionResult> Index()
    {
        var lista = await _db.Misiones
            .Include(m => m.Asignaciones)
            .OrderByDescending(m => m.FechaPublicacion)
            .AsNoTracking()
            .ToListAsync();
        return View(lista);
    }

    // READ: detalle
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();
        var mision = await _db.Misiones
            .Include(m => m.Asignaciones).ThenInclude(a => a.Aventurero)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.IdMision == id);
        return mision is null ? NotFound() : View(mision);
    }

    // CREATE
    public IActionResult Create()
    {
        CargarListas();
        return View(new Mision { Estado = EstadoMision.Disponible });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Titulo,Descripcion,Dificultad,Recompensa,Estado")] Mision mision)
    {
        if (!ModelState.IsValid)
        {
            CargarListas(mision.Dificultad, mision.Estado);
            return View(mision);
        }

        mision.FechaPublicacion = DateTime.Now;
        _db.Add(mision);
        await _db.SaveChangesAsync();
        TempData["Mensaje"] = "Misión registrada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // UPDATE
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        var mision = await _db.Misiones.FindAsync(id);
        if (mision is null) return NotFound();

        CargarListas(mision.Dificultad, mision.Estado);
        return View(mision);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("IdMision,Titulo,Descripcion,Dificultad,Recompensa,Estado")] Mision modelo)
    {
        if (id != modelo.IdMision) return NotFound();

        if (!ModelState.IsValid)
        {
            CargarListas(modelo.Dificultad, modelo.Estado);
            return View(modelo);
        }

        var mision = await _db.Misiones.FindAsync(id);
        if (mision is null) return NotFound();

        mision.Titulo = modelo.Titulo;
        mision.Descripcion = modelo.Descripcion;
        mision.Dificultad = modelo.Dificultad;
        mision.Recompensa = modelo.Recompensa;
        mision.Estado = modelo.Estado;   // FechaPublicacion no se modifica

        await _db.SaveChangesAsync();
        TempData["Mensaje"] = "Misión actualizada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // DELETE
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        var mision = await _db.Misiones
            .Include(m => m.Asignaciones)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.IdMision == id);
        return mision is null ? NotFound() : View(mision);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var mision = await _db.Misiones.FindAsync(id);
        if (mision is null) return RedirectToAction(nameof(Index));

        // Integridad: una misión con asignaciones (de cualquier estado) no se puede eliminar
        if (await _db.Asignaciones.AnyAsync(a => a.IdMision == id))
        {
            TempData["Error"] = "No se puede eliminar la misión: tiene asignaciones registradas.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _db.Misiones.Remove(mision);
            await _db.SaveChangesAsync();
            TempData["Mensaje"] = "Misión eliminada correctamente.";
        }
        catch (DbUpdateException)
        {
            // Red de seguridad: si la BD rechaza el borrado por la FK (Restrict)
            TempData["Error"] = "No se pudo eliminar la misión porque está siendo utilizada por otros registros.";
        }
        return RedirectToAction(nameof(Index));
    }

    private void CargarListas(string? dificultad = null, string? estado = null)
    {
        ViewBag.Dificultades = new SelectList(DificultadMision.Todas, dificultad);
        ViewBag.Estados = new SelectList(EstadoMision.Todos, estado);
    }
}
