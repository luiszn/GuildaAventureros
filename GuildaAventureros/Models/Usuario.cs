using System.ComponentModel.DataAnnotations;
using GuildaAventureros.Constants;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GuildaAventureros.Models;

public class Usuario : IValidatableObject
{
    [Key]
    public int IdUsuario { get; set; }

    [Required, StringLength(50)]
    [Display(Name = "Usuario")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required, StringLength(100), EmailAddress]
    public string Correo { get; set; } = string.Empty;

    [Required, StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Display(Name = "Rol")]
    public int IdRol { get; set; }

    [Display(Name = "Aventurero")]
    public int? IdAventurero { get; set; }

    public bool Estado { get; set; } = true;

    [ValidateNever]
    public Rol? Rol { get; set; }

    [ValidateNever]
    public Aventurero? Aventurero { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (IdRol == RolIds.Aventurero && IdAventurero is null)
        {
            yield return new ValidationResult(
                "Un usuario con rol Aventurero debe estar asociado a un registro de Aventurero.",
                new[] { nameof(IdAventurero) });
        }
    }
}
