using GuildaAventureros.Constants;
using GuildaAventureros.Data;
using GuildaAventureros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GuildaAventureros.Controllers;

public class AsignacionesController : Controller
{
    private readonly GuildaDbContext _db;

    public AsignacionesController(GuildaDbContext db) => _db = db;

    // READ: listado, con filtro opcional por estado
    public async Task<IActionResult> Index(string? estado)
    {
        var consulta = _db.Asignaciones
            .Include(a => a.Aventurero)
            .Include(a => a.Mision)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(estado))
            consulta = consulta.Where(a => a.Estado == estado);

        var lista = await consulta.OrderByDescending(a => a.FechaAsignacion).ToListAsync();
        ViewBag.EstadoFiltro = estado;
        return View(lista);
    }

    // READ: detalle
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();
        var asignacion = await _db.Asignaciones
            .Include(a => a.Aventurero).ThenInclude(a => a!.Especialidad)
            .Include(a => a.Mision)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.IdAsignacion == id);
        return asignacion is null ? NotFound() : View(asignacion);
    }

    // CREATE
    public async Task<IActionResult> Create()
    {
        await CargarListas();
        return View(new Asignacion());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdAventurero,IdMision")] Asignacion asignacion)
    {
        await ValidarReglasDeNegocio(asignacion);

        if (!ModelState.IsValid)
        {
            await CargarListas(asignacion.IdAventurero, asignacion.IdMision);
            return View(asignacion);
        }

        asignacion.Estado = EstadoAsignacion.Activa;
        asignacion.FechaAsignacion = DateTime.Now;
        _db.Add(asignacion);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Red de seguridad: los índices únicos de la BD frenan duplicados o dos activas simultáneas
            ModelState.AddModelError(string.Empty,
                "No se pudo registrar la asignación: ya existe una asignación duplicada o el aventurero ya tiene una misión activa.");
            await CargarListas(asignacion.IdAventurero, asignacion.IdMision);
            return View(asignacion);
        }

        TempData["Mensaje"] = "Asignación registrada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // Cambios de estado: solo una asignación ACTIVA puede finalizarse o cancelarse
    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> Finalizar(int id) => CambiarEstado(id, EstadoAsignacion.Finalizada);

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> Cancelar(int id) => CambiarEstado(id, EstadoAsignacion.Cancelada);

    private async Task<IActionResult> CambiarEstado(int id, string nuevoEstado)
    {
        var asignacion = await _db.Asignaciones.FindAsync(id);
        if (asignacion is null) return NotFound();

        if (asignacion.Estado != EstadoAsignacion.Activa)
        {
            TempData["Error"] = $"Solo se pueden modificar asignaciones activas (estado actual: {asignacion.Estado}).";
            return RedirectToAction(nameof(Index));
        }

        asignacion.Estado = nuevoEstado;
        await _db.SaveChangesAsync();
        TempData["Mensaje"] = $"La asignación #{id} quedó como {nuevoEstado.ToLower()}.";
        return RedirectToAction(nameof(Index));
    }

    // Los desplegables muestran todos los registros; los no elegibles se marcan y el servidor los rechaza.
    private async Task CargarListas(int? idAventurero = null, int? idMision = null)
    {
        var aventureros = (await _db.Aventureros.AsNoTracking().OrderBy(a => a.Nombre).ToListAsync())
            .Select(a => new { a.IdAventurero, Texto = a.Estado ? a.Nombre : $"{a.Nombre} (inactivo)" });

        var misiones = (await _db.Misiones.AsNoTracking().OrderBy(m => m.Titulo).ToListAsync())
            .Select(m => new { m.IdMision, Texto = m.Estado == EstadoMision.Disponible ? m.Titulo : $"{m.Titulo} ({m.Estado})" });

        ViewBag.Aventureros = new SelectList(aventureros, "IdAventurero", "Texto", idAventurero);
        ViewBag.Misiones = new SelectList(misiones, "IdMision", "Texto", idMision);
    }

    // Reglas de negocio de la Actividad 6
    private async Task ValidarReglasDeNegocio(Asignacion a)
    {
        Aventurero? aventurero = null;
        Mision? mision = null;

        // 1) El aventurero debe existir y estar activo
        if (a.IdAventurero > 0)
        {
            aventurero = await _db.Aventureros.AsNoTracking().FirstOrDefaultAsync(x => x.IdAventurero == a.IdAventurero);
            if (aventurero is null)
                ModelState.AddModelError(nameof(a.IdAventurero), "El aventurero seleccionado no existe.");
            else if (!aventurero.Estado)
                ModelState.AddModelError(nameof(a.IdAventurero), "El aventurero está inactivo y no puede recibir misiones.");
        }

        // 2) La misión debe existir y estar disponible
        if (a.IdMision > 0)
        {
            mision = await _db.Misiones.AsNoTracking().FirstOrDefaultAsync(x => x.IdMision == a.IdMision);
            if (mision is null)
                ModelState.AddModelError(nameof(a.IdMision), "La misión seleccionada no existe.");
            else if (mision.Estado != EstadoMision.Disponible)
                ModelState.AddModelError(nameof(a.IdMision), $"La misión no está disponible (estado actual: {mision.Estado}).");
        }

        if (aventurero is null || mision is null) return;

        // 3) No puede existir una asignación duplicada (mismo aventurero y misma misión)
        if (await _db.Asignaciones.AnyAsync(x => x.IdAventurero == a.IdAventurero && x.IdMision == a.IdMision))
        {
            ModelState.AddModelError(string.Empty, "Ya existe una asignación de este aventurero a esta misión.");
            return;
        }

        // 4) Un aventurero no puede tener dos misiones activas a la vez
        //    (Finalizada y Cancelada no cuentan como activas)
        if (await _db.Asignaciones.AnyAsync(x => x.IdAventurero == a.IdAventurero && x.Estado == EstadoAsignacion.Activa))
            ModelState.AddModelError(nameof(a.IdAventurero), "El aventurero ya tiene una misión activa. Debe finalizarla o cancelarla antes de recibir otra.");
    }
}
