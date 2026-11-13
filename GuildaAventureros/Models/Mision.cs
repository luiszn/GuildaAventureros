using System.ComponentModel.DataAnnotations;
using GuildaAventureros.Constants;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GuildaAventureros.Models;

public class Mision
{
    [Key]
    public int IdMision { get; set; }

    [Required(ErrorMessage = "El título de la misión es obligatorio.")]
    [StringLength(120, ErrorMessage = "El título no puede superar los 120 caracteres.")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleccione la dificultad.")]
    [AllowedValues(DificultadMision.Facil, DificultadMision.Media, DificultadMision.Dificil,
        ErrorMessage = "La dificultad debe ser Fácil, Media o Difícil.")]
    public string Dificultad { get; set; } = string.Empty;

    [Required(ErrorMessage = "La recompensa es obligatoria.")]
    [Range(0.01, 99999999.99, ErrorMessage = "La recompensa debe ser mayor que cero y no superar 99 999 999,99.")]
    public decimal Recompensa { get; set; }

    [Required(ErrorMessage = "Seleccione el estado.")]
    [AllowedValues(EstadoMision.Disponible, EstadoMision.EnCurso, EstadoMision.Completada,
        ErrorMessage = "El estado debe ser Disponible, En curso o Completada.")]
    public string Estado { get; set; } = EstadoMision.Disponible;

    [Display(Name = "Fecha de publicación")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
    public DateTime FechaPublicacion { get; set; }

    [ValidateNever]
    public ICollection<Asignacion> Asignaciones { get; set; } = new List<Asignacion>();
}
