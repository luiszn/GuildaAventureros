using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GuildaAventureros.Migrations
{
    /// <inheritdoc />
    public partial class Explorador_AgregarMisionesAsignaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Misiones",
                columns: table => new
                {
                    IdMision = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "varchar(120)", unicode: false, maxLength: 120, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    Dificultad = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    Recompensa = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Estado = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Misiones", x => x.IdMision);
                    table.CheckConstraint("CK_Misiones_Dificultad", "[Dificultad] IN ('Fácil', 'Media', 'Difícil')");
                    table.CheckConstraint("CK_Misiones_Estado", "[Estado] IN ('Disponible', 'En curso', 'Completada')");
                    table.CheckConstraint("CK_Misiones_Recompensa", "[Recompensa] > 0");
                });

            migrationBuilder.CreateTable(
                name: "Asignaciones",
                columns: table => new
                {
                    IdAsignacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAventurero = table.Column<int>(type: "int", nullable: false),
                    IdMision = table.Column<int>(type: "int", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    Estado = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asignaciones", x => x.IdAsignacion);
                    table.CheckConstraint("CK_Asignaciones_Estado", "[Estado] IN ('Activa', 'Finalizada', 'Cancelada')");
                    table.ForeignKey(
                        name: "FK_Asignaciones_Aventureros_IdAventurero",
                        column: x => x.IdAventurero,
                        principalTable: "Aventureros",
                        principalColumn: "IdAventurero",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asignaciones_Misiones_IdMision",
                        column: x => x.IdMision,
                        principalTable: "Misiones",
                        principalColumn: "IdMision",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asignaciones_IdMision",
                table: "Asignaciones",
                column: "IdMision");

            migrationBuilder.CreateIndex(
                name: "UX_Asignaciones_Aventurero_Mision",
                table: "Asignaciones",
                columns: new[] { "IdAventurero", "IdMision" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Asignaciones_UnaActivaPorAventurero",
                table: "Asignaciones",
                column: "IdAventurero",
                unique: true,
                filter: "[Estado] = 'Activa'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asignaciones");

            migrationBuilder.DropTable(
                name: "Misiones");
        }
    }
}
