using System.ComponentModel.DataAnnotations;
using GuildaAventureros.Constants;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GuildaAventureros.Models;

public class Asignacion
{
    [Key]
    public int IdAsignacion { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un aventurero.")]
    [Display(Name = "Aventurero")]
    public int IdAventurero { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una misión.")]
    [Display(Name = "Misión")]
    public int IdMision { get; set; }

    [Display(Name = "Fecha de asignación")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
    public DateTime FechaAsignacion { get; set; }

    // Valor por defecto = Activa: el formulario de registro no envía el estado, lo fija el servidor.
    [Required]
    [AllowedValues(EstadoAsignacion.Activa, EstadoAsignacion.Finalizada, EstadoAsignacion.Cancelada,
        ErrorMessage = "El estado debe ser Activa, Finalizada o Cancelada.")]
    public string Estado { get; set; } = EstadoAsignacion.Activa;

    [ValidateNever]
    public Aventurero? Aventurero { get; set; }

    [ValidateNever]
    public Mision? Mision { get; set; }
}
