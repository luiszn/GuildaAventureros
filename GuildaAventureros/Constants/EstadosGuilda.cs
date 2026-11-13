namespace GuildaAventureros.Constants;

public static class DificultadMision
{
    public const string Facil = "Fácil";
    public const string Media = "Media";
    public const string Dificil = "Difícil";

    public static readonly string[] Todas = { Facil, Media, Dificil };
}

public static class EstadoMision
{
    public const string Disponible = "Disponible";
    public const string EnCurso = "En curso";
    public const string Completada = "Completada";

    public static readonly string[] Todos = { Disponible, EnCurso, Completada };
}

public static class EstadoAsignacion
{
    public const string Activa = "Activa";
    public const string Finalizada = "Finalizada";
    public const string Cancelada = "Cancelada";

    public static readonly string[] Todos = { Activa, Finalizada, Cancelada };
}
