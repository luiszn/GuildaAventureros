using GuildaAventureros.Constants;
using GuildaAventureros.Data;
using GuildaAventureros.Services;
using GuildaAventureros.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuildaAventureros.Controllers;

public class UsuariosController : Controller
{
    private readonly GuildaDbContext _db;

    public UsuariosController(GuildaDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var usuarios = await _db.Usuarios
            .Include(u => u.Rol)
            .Include(u => u.Aventurero)
            .AsNoTracking()
            .OrderBy(u => u.IdUsuario)
            .ToListAsync();
        return View(usuarios);
    }
    public async Task<IActionResult> Integridad()
    {
        var roles = await _db.Roles.AsNoTracking().ToListAsync();
        var usuarios = await _db.Usuarios.Include(u => u.Rol).Include(u => u.Aventurero).AsNoTracking().ToListAsync();
        var vm = new IntegridadViewModel();

        var esperadosRoles = new[] { "Administrador", "Maestro de la Guilda", "Aventurero" };
        var faltantes = esperadosRoles.Except(roles.Select(r => r.Nombre)).ToList();
        vm.Verificaciones.Add(new("Los tres roles predeterminados están registrados",
            faltantes.Count == 0, faltantes.Count == 0 ? $"{roles.Count} roles registrados" : "Faltan: " + string.Join(", ", faltantes)));

        var perfiles = new[] { (RolIds.Administrador, "Administrador"), (RolIds.MaestroGuilda, "Maestro"), (RolIds.Aventurero, "Aventurero") };
        var sinUsuario = perfiles.Where(p => !usuarios.Any(u => u.IdRol == p.Item1)).Select(p => p.Item2).ToList();
        vm.Verificaciones.Add(new("Existen los tres usuarios predeterminados (uno por perfil)",
            sinUsuario.Count == 0, sinUsuario.Count == 0 ? $"{usuarios.Count} usuarios registrados" : "Sin usuario para: " + string.Join(", ", sinUsuario)));

        var sinRol = usuarios.Where(u => u.Rol is null).Select(u => u.NombreUsuario).ToList();
        vm.Verificaciones.Add(new("Cada usuario tiene un rol válido",
            sinRol.Count == 0, sinRol.Count == 0 ? "Todos los usuarios tienen rol" : "Sin rol: " + string.Join(", ", sinRol)));

        var dupUsuarios = usuarios.GroupBy(u => u.NombreUsuario, StringComparer.OrdinalIgnoreCase).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        var dupCorreos = usuarios.GroupBy(u => u.Correo, StringComparer.OrdinalIgnoreCase).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        vm.Verificaciones.Add(new("No existen nombres de usuario ni correos duplicados",
            dupUsuarios.Count == 0 && dupCorreos.Count == 0,
            dupUsuarios.Count + dupCorreos.Count == 0 ? "Sin duplicados" : "Duplicados: " + string.Join(", ", dupUsuarios.Concat(dupCorreos))));

        var sinHash = usuarios.Where(u => !u.PasswordHash.StartsWith(PasswordService.Prefix + "$")).Select(u => u.NombreUsuario).ToList();
        vm.Verificaciones.Add(new("Las contraseñas están almacenadas con hash seguro (PBKDF2)",
            sinHash.Count == 0, sinHash.Count == 0 ? "Todos los hashes tienen el formato esperado" : "Sin hash seguro: " + string.Join(", ", sinHash)));

        var aventureroSinVinculo = usuarios.Where(u => u.IdRol == RolIds.Aventurero && u.Aventurero is null).Select(u => u.NombreUsuario).ToList();
        vm.Verificaciones.Add(new("Los usuarios con rol Aventurero están vinculados a un Aventurero válido",
            aventureroSinVinculo.Count == 0, aventureroSinVinculo.Count == 0 ? "Vínculos correctos" : "Sin vínculo: " + string.Join(", ", aventureroSinVinculo)));

        return View(vm);
    }
}
