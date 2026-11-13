namespace GuildaAventureros.ViewModels;

public record VerificacionItem(string Descripcion, bool Correcto, string Detalle);

public class IntegridadViewModel
{
    public List<VerificacionItem> Verificaciones { get; init; } = new();
    public bool TodoCorrecto => Verificaciones.All(v => v.Correcto);
}
