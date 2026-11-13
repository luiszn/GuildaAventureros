using GuildaAventureros.Constants;
using GuildaAventureros.Models;
using Microsoft.EntityFrameworkCore;

namespace GuildaAventureros.Data;

public class GuildaDbContext : DbContext
{
    public GuildaDbContext(DbContextOptions<GuildaDbContext> options) : base(options) { }

    public DbSet<Especialidad> Especialidades => Set<Especialidad>();
    public DbSet<Aventurero> Aventureros => Set<Aventurero>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Mision> Misiones => Set<Mision>();
    public DbSet<Asignacion> Asignaciones => Set<Asignacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Especialidad>(e =>
        {
            e.Property(x => x.Nombre).HasMaxLength(50).IsUnicode(false).IsRequired();
            e.Property(x => x.Descripcion).HasMaxLength(150).IsUnicode(false);
            e.HasIndex(x => x.Nombre).IsUnique();

            e.HasData(
                new Especialidad { IdEspecialidad = 1, Nombre = "Guerrero", Descripcion = "Combatiente cuerpo a cuerpo, experto en armas y resistencia física." },
                new Especialidad { IdEspecialidad = 2, Nombre = "Mago", Descripcion = "Maestro de las artes arcanas y de los hechizos." },
                new Especialidad { IdEspecialidad = 3, Nombre = "Arquero", Descripcion = "Especialista en combate a distancia, precisión y sigilo." },
                new Especialidad { IdEspecialidad = 4, Nombre = "Paladín", Descripcion = "Guerrero sagrado que combina fuerza marcial y poder de sanación." });
        });

        modelBuilder.Entity<Aventurero>(e =>
        {
            e.Property(x => x.Nombre).HasMaxLength(100).IsUnicode(false).IsRequired();
            e.Property(x => x.Estado).HasColumnType("bit");
            e.Property(x => x.FechaRegistro).HasColumnType("datetime");

            e.HasOne(x => x.Especialidad)
             .WithMany(x => x.Aventureros)
             .HasForeignKey(x => x.IdEspecialidad)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasData(new Aventurero
            {
                IdAventurero = 1,
                Nombre = "Aventurero Inicial",
                IdEspecialidad = 1,
                Nivel = 1,
                Estado = true,
                FechaRegistro = new DateTime(2026, 1, 1, 0, 0, 0)
            });
        });

        modelBuilder.Entity<Rol>(e =>
        {
            e.Property(x => x.Nombre).HasMaxLength(50).IsUnicode(false).IsRequired();
            e.Property(x => x.Descripcion).HasMaxLength(200).IsUnicode(false);
            e.HasIndex(x => x.Nombre).IsUnique();

            e.HasData(
                new Rol { IdRol = RolIds.Administrador, Nombre = "Administrador", Descripcion = "Administra el sistema, los usuarios y la configuración general de la Guilda." },
                new Rol { IdRol = RolIds.MaestroGuilda, Nombre = "Maestro de la Guilda", Descripcion = "Dirige la Guilda: gestiona aventureros, misiones y asignaciones." },
                new Rol { IdRol = RolIds.Aventurero, Nombre = "Aventurero", Descripcion = "Miembro de la Guilda que consulta y realiza las misiones asignadas." });
        });

        modelBuilder.Entity<Usuario>(e =>
        {
            e.Property(x => x.NombreUsuario).HasMaxLength(50).IsUnicode(false).IsRequired();
            e.Property(x => x.Correo).HasMaxLength(100).IsUnicode(false).IsRequired();
            e.Property(x => x.PasswordHash).HasMaxLength(255).IsUnicode(false).IsRequired();
            e.Property(x => x.Estado).HasColumnType("bit");

            e.HasIndex(x => x.NombreUsuario).IsUnique();
            e.HasIndex(x => x.Correo).IsUnique();

            e.HasOne(x => x.Rol)
             .WithMany(x => x.Usuarios)
             .HasForeignKey(x => x.IdRol)
             .IsRequired()
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Aventurero)
             .WithOne(x => x.Usuario)
             .HasForeignKey<Usuario>(x => x.IdAventurero)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.Restrict);

            e.ToTable(t => t.HasCheckConstraint(
                "CK_Usuarios_RolAventurero_RequiereAventurero",
                $"[IdRol] <> {RolIds.Aventurero} OR [IdAventurero] IS NOT NULL"));

            // Contraseñas iniciales (solo entorno académico):
            //   admin / Admin123*   maestro / Maestro123*   aventurero / Aventurero123*
            e.HasData(
                new Usuario
                {
                    IdUsuario = 1,
                    NombreUsuario = "admin",
                    Correo = "admin@guilda.local",
                    PasswordHash = "PBKDF2-SHA256$600000$E55B0KJXkhnrRXas2lQOSQ==$Ga81qex3fF0xXxwe4CcFU+jmvlNA1lbjqjoNBmpd6Xg=",
                    IdRol = RolIds.Administrador,
                    IdAventurero = null,
                    Estado = true
                },
                new Usuario
                {
                    IdUsuario = 2,
                    NombreUsuario = "maestro",
                    Correo = "maestro@guilda.local",
                    PasswordHash = "PBKDF2-SHA256$600000$zLF/h3RF7tA6W9193lrgPQ==$huvr8jqhosWswCS2B8ageG/NvKL8SUsnVEf1/e2fak0=",
                    IdRol = RolIds.MaestroGuilda,
                    IdAventurero = null,
                    Estado = true
                },
                new Usuario
                {
                    IdUsuario = 3,
                    NombreUsuario = "aventurero",
                    Correo = "aventurero@guilda.local",
                    PasswordHash = "PBKDF2-SHA256$600000$4b4ndGcwUu6YYzk4ZRBNGA==$dGEmtahTTxuOGLxopVTvFJmGuWqyOqv0y16SMP13hNE=",
                    IdRol = RolIds.Aventurero,
                    IdAventurero = 1,
                    Estado = true
                });
        });

        // ---------- Misión 3: Misiones y Asignaciones ----------
        modelBuilder.Entity<Mision>(e =>
        {
            e.Property(x => x.Titulo).HasMaxLength(120).IsUnicode(false).IsRequired();
            e.Property(x => x.Descripcion).HasMaxLength(500).IsUnicode(false).IsRequired();
            e.Property(x => x.Dificultad).HasMaxLength(30).IsUnicode(false).IsRequired();
            e.Property(x => x.Recompensa).HasColumnType("decimal(10,2)");
            e.Property(x => x.Estado).HasMaxLength(30).IsUnicode(false).IsRequired();
            e.Property(x => x.FechaPublicacion).HasColumnType("datetime");

            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Misiones_Dificultad",
                    $"[Dificultad] IN ('{DificultadMision.Facil}', '{DificultadMision.Media}', '{DificultadMision.Dificil}')");
                t.HasCheckConstraint("CK_Misiones_Estado",
                    $"[Estado] IN ('{EstadoMision.Disponible}', '{EstadoMision.EnCurso}', '{EstadoMision.Completada}')");
                t.HasCheckConstraint("CK_Misiones_Recompensa", "[Recompensa] > 0");
            });
        });

        modelBuilder.Entity<Asignacion>(e =>
        {
            e.Property(x => x.Estado).HasMaxLength(30).IsUnicode(false).IsRequired();
            e.Property(x => x.FechaAsignacion).HasColumnType("datetime");

            // Aventurero 1 — * Asignación
            e.HasOne(x => x.Aventurero)
             .WithMany(x => x.Asignaciones)
             .HasForeignKey(x => x.IdAventurero)
             .IsRequired()
             .OnDelete(DeleteBehavior.Restrict);

            // Misión 1 — * Asignación
            e.HasOne(x => x.Mision)
             .WithMany(x => x.Asignaciones)
             .HasForeignKey(x => x.IdMision)
             .IsRequired()
             .OnDelete(DeleteBehavior.Restrict);

            // Regla: no puede haber asignaciones duplicadas (mismo aventurero y misma misión)
            e.HasIndex(x => new { x.IdAventurero, x.IdMision })
             .IsUnique()
             .HasDatabaseName("UX_Asignaciones_Aventurero_Mision");

            // Regla: un aventurero solo puede tener UNA asignación activa (índice único filtrado)
            e.HasIndex(x => x.IdAventurero)
             .IsUnique()
             .HasFilter($"[Estado] = '{EstadoAsignacion.Activa}'")
             .HasDatabaseName("UX_Asignaciones_UnaActivaPorAventurero");

            e.ToTable(t => t.HasCheckConstraint("CK_Asignaciones_Estado",
                $"[Estado] IN ('{EstadoAsignacion.Activa}', '{EstadoAsignacion.Finalizada}', '{EstadoAsignacion.Cancelada}')"));
        });
    }
}
