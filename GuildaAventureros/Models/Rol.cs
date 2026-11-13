using System.ComponentModel.DataAnnotations;

namespace GuildaAventureros.Models;

public class Rol
{
    [Key]
    public int IdRol { get; set; }

    [Required, StringLength(50)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Descripcion { get; set; }

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
