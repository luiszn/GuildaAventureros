using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GuildaAventureros.Models;

public class Aventurero
{
    [Key]
    public int IdAventurero { get; set; }

    [Required(ErrorMessage = "El nombre del aventurero es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una especialidad válida.")]
    [Display(Name = "Especialidad")]
    public int IdEspecialidad { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El nivel debe ser mayor que cero.")]
    public int Nivel { get; set; } = 1;

    [Display(Name = "Activo")]
    public bool Estado { get; set; } = true;

    [Display(Name = "Fecha de registro")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
    public DateTime FechaRegistro { get; set; }

    [ValidateNever]
    public Especialidad? Especialidad { get; set; }

    [ValidateNever]
    public Usuario? Usuario { get; set; }
    [ValidateNever]
    public ICollection<Asignacion> Asignaciones { get; set; } = new List<Asignacion>();
}
