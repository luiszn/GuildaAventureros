using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GuildaAventureros.Migrations
{
    /// <inheritdoc />
    public partial class CreacionDBGuilda16043 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Especialidades",
                columns: table => new
                {
                    IdEspecialidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especialidades", x => x.IdEspecialidad);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    IdRol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.IdRol);
                });

            migrationBuilder.CreateTable(
                name: "Aventureros",
                columns: table => new
                {
                    IdAventurero = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    IdEspecialidad = table.Column<int>(type: "int", nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aventureros", x => x.IdAventurero);
                    table.ForeignKey(
                        name: "FK_Aventureros_Especialidades_IdEspecialidad",
                        column: x => x.IdEspecialidad,
                        principalTable: "Especialidades",
                        principalColumn: "IdEspecialidad",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUsuario = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Correo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    IdRol = table.Column<int>(type: "int", nullable: false),
                    IdAventurero = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                    table.CheckConstraint("CK_Usuarios_RolAventurero_RequiereAventurero", "[IdRol] <> 3 OR [IdAventurero] IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_Usuarios_Aventureros_IdAventurero",
                        column: x => x.IdAventurero,
                        principalTable: "Aventureros",
                        principalColumn: "IdAventurero",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_IdRol",
                        column: x => x.IdRol,
                        principalTable: "Roles",
                        principalColumn: "IdRol",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Especialidades",
                columns: new[] { "IdEspecialidad", "Descripcion", "Nombre" },
                values: new object[,]
                {
                    { 1, "Combatiente cuerpo a cuerpo, experto en armas y resistencia física.", "Guerrero" },
                    { 2, "Maestro de las artes arcanas y de los hechizos.", "Mago" },
                    { 3, "Especialista en combate a distancia, precisión y sigilo.", "Arquero" },
                    { 4, "Guerrero sagrado que combina fuerza marcial y poder de sanación.", "Paladín" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "IdRol", "Descripcion", "Nombre" },
                values: new object[,]
                {
                    { 1, "Administra el sistema, los usuarios y la configuración general de la Guilda.", "Administrador" },
                    { 2, "Dirige la Guilda: gestiona aventureros, misiones y asignaciones.", "Maestro de la Guilda" },
                    { 3, "Miembro de la Guilda que consulta y realiza las misiones asignadas.", "Aventurero" }
                });

            migrationBuilder.InsertData(
                table: "Aventureros",
                columns: new[] { "IdAventurero", "Estado", "FechaRegistro", "IdEspecialidad", "Nivel", "Nombre" },
                values: new object[] { 1, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, "Aventurero Inicial" });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "IdUsuario", "Correo", "Estado", "IdAventurero", "IdRol", "NombreUsuario", "PasswordHash" },
                values: new object[,]
                {
                    { 1, "admin@guilda.local", true, null, 1, "admin", "PBKDF2-SHA256$600000$E55B0KJXkhnrRXas2lQOSQ==$Ga81qex3fF0xXxwe4CcFU+jmvlNA1lbjqjoNBmpd6Xg=" },
                    { 2, "maestro@guilda.local", true, null, 2, "maestro", "PBKDF2-SHA256$600000$zLF/h3RF7tA6W9193lrgPQ==$huvr8jqhosWswCS2B8ageG/NvKL8SUsnVEf1/e2fak0=" },
                    { 3, "aventurero@guilda.local", true, 1, 3, "aventurero", "PBKDF2-SHA256$600000$4b4ndGcwUu6YYzk4ZRBNGA==$dGEmtahTTxuOGLxopVTvFJmGuWqyOqv0y16SMP13hNE=" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aventureros_IdEspecialidad",
                table: "Aventureros",
                column: "IdEspecialidad");

            migrationBuilder.CreateIndex(
                name: "IX_Especialidades_Nombre",
                table: "Especialidades",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Nombre",
                table: "Roles",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdAventurero",
                table: "Usuarios",
                column: "IdAventurero",
                unique: true,
                filter: "[IdAventurero] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdRol",
                table: "Usuarios",
                column: "IdRol");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NombreUsuario",
                table: "Usuarios",
                column: "NombreUsuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Aventureros");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Especialidades");
        }
    }
}
