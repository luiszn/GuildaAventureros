using System.ComponentModel.DataAnnotations;

namespace GuildaAventureros.Models;

public class Especialidad
{
    [Key]
    public int IdEspecialidad { get; set; }

    [Required, StringLength(50)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(150)]
    public string? Descripcion { get; set; }

    public ICollection<Aventurero> Aventureros { get; set; } = new List<Aventurero>();
}
