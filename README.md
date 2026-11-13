# La Guilda de Aventureros — ASP.NET Core MVC (.NET 10) + EF Core + SQL Server

Integra la **Misión 1 (Guardián)** y la **Misión 2 (Hechicero)**:

| Rol | Contenido implementado |
|---|---|
| Guardián | Proyecto MVC, cadena de conexión, EF Core, `Especialidad` (4 semillas), `Aventurero`, CRUD con lista desplegable y validaciones |
| Hechicero | `Rol` (3 semillas), `Usuario` (hash PBKDF2, únicos, rol obligatorio, vínculo opcional a Aventurero), 3 usuarios iniciales, verificación de integridad |
| D1 / D2 | `docs/D1-diagrama-de-componentes.md` (La Guilda necesita organizar sus recursos), `docs/D2-diagrama-de-clases.md` (El conocimiento de la Guilda debe estar organizado) (Mermaid + explicación borrador) |

## Requisitos
- .NET SDK 10
- SQL Server (LocalDB, Express o instancia completa)
- Herramienta EF: `dotnet tool install --global dotnet-ef`

## Puesta en marcha
```bash
cd GuildaAventureros
# 1. Ajusta la cadena "GuildaDb" en appsettings.json si no usas LocalDB
# 2. Crear migración inicial y la base de datos
dotnet ef migrations add Inicial
dotnet ef database update
# 3. Ejecutar
dotnet run
```
En Visual Studio (Package Manager Console): `Add-Migration Inicial` y `Update-Database`.

## Usuarios predeterminados (solo académico)
| Usuario | Contraseña | Rol |
|---|---|---|
| admin | Admin123* | Administrador |
| maestro | Maestro123* | Maestro de la Guilda |
| aventurero | Aventurero123* | Aventurero (vinculado a "Aventurero Inicial") |

Las contraseñas se almacenan como `PBKDF2-SHA256$iteraciones$sal$hash` (nunca en texto plano).

## Verificación sugerida (Actividad 6)
- `/Aventureros`: registrar, listar, editar, eliminar. Validaciones: nombre obligatorio, nivel > 0, especialidad válida.
- `/Usuarios`: roles, vínculo con Aventurero y hash.
- `/Usuarios/Integridad`: roles, usuarios, duplicados, hashes y vínculos.
- Índices únicos (`NombreUsuario`, `Correo`) y CHECK (`rol Aventurero` ⇒ `IdAventurero` no nulo) en la base de datos.
