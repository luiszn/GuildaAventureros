# D1. La Guilda necesita organizar sus recursos — Diagrama de componentes (Guardián)

```mermaid
flowchart LR
    U([Navegador / Usuario])

    subgraph WEB["Aplicación web ASP.NET Core MVC (.NET 10)"]
        direction TB
        subgraph AV["Módulo Aventureros"]
            AVC[AventurerosController]
            AVM[Aventurero / Especialidad]
            AVV[Vistas Aventureros]
        end
        subgraph MI["Módulo Misiones (planificado)"]
            MIC[MisionesController]
            MIM[Mision]
        end
        subgraph AS["Módulo Asignaciones (planificado)"]
            ASC[AsignacionesController]
            ASM[Asignacion]
        end
        subgraph SE["Módulo Seguridad"]
            SEC[UsuariosController]
            SEM[Usuario / Rol]
            SES[PasswordService]
        end
        DB[GuildaDbContext<br/>Entity Framework Core]
    end

    SQL[(SQL Server)]

    U -->|HTTP| AVC & MIC & ASC & SEC
    AVC --> AVM & AVV
    SEC --> SEM & SES
    AVC & MIC & ASC & SEC --> DB
    DB -->|SQL / Migraciones| SQL

    ASC -.->|usa| AVM
    ASC -.->|usa| MIM
    SEM -.->|vincula (opcional)| AVM
```

## Explicación individual (borrador — adáptalo con tus palabras)

- **Controladores:** reciben las solicitudes HTTP, validan datos y coordinan modelos y vistas de cada módulo.
- **Modelos:** representan las entidades del dominio (Aventurero, Especialidad, Usuario, Rol; Misión y Asignación en la expansión) con sus reglas de validación.
- **Vistas (Razor):** presentan la información y los formularios (p. ej. lista desplegable de especialidades).
- **GuildaDbContext (EF Core):** única puerta de acceso a datos; mapea los modelos a tablas, define relaciones, datos semilla y migraciones contra **SQL Server**.
- **Módulo Seguridad:** gestiona usuarios y roles; `PasswordService` genera y verifica hashes PBKDF2.
- **Dependencias entre módulos:** Asignaciones depende de Aventureros y Misiones (une ambos); Seguridad se vincula opcionalmente con Aventureros (usuario ↔ aventurero).
