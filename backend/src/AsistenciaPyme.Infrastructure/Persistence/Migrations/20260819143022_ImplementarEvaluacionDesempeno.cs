using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AsistenciaPyme.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ImplementarEvaluacionDesempeno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "id_jefe_directo",
                table: "empleados",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "categorias_evaluacion",
                columns: table => new
                {
                    id_categoria_evaluacion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ponderacion = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_actualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias_evaluacion", x => x.id_categoria_evaluacion);
                });

            migrationBuilder.CreateTable(
                name: "periodos_evaluacion",
                columns: table => new
                {
                    id_periodo_evaluacion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fecha_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_fin = table.Column<DateOnly>(type: "date", nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_actualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_periodos_evaluacion", x => x.id_periodo_evaluacion);
                });

            migrationBuilder.CreateTable(
                name: "criterios_evaluacion",
                columns: table => new
                {
                    id_criterio_evaluacion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_categoria_evaluacion = table.Column<int>(type: "integer", nullable: false),
                    texto = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_actualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_criterios_evaluacion", x => x.id_criterio_evaluacion);
                    table.ForeignKey(
                        name: "FK_criterios_evaluacion_categorias_evaluacion_id_categoria_eva~",
                        column: x => x.id_categoria_evaluacion,
                        principalTable: "categorias_evaluacion",
                        principalColumn: "id_categoria_evaluacion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "evaluaciones_desempeno",
                columns: table => new
                {
                    id_evaluacion_desempeno = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_periodo_evaluacion = table.Column<int>(type: "integer", nullable: false),
                    id_empleado_evaluado = table.Column<int>(type: "integer", nullable: false),
                    id_evaluador = table.Column<int>(type: "integer", nullable: false),
                    tipo_evaluador = table.Column<int>(type: "integer", nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    puntaje_final = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    observaciones_generales = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    fecha_asignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_completada = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_actualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evaluaciones_desempeno", x => x.id_evaluacion_desempeno);
                    table.ForeignKey(
                        name: "FK_evaluaciones_desempeno_empleados_id_empleado_evaluado",
                        column: x => x.id_empleado_evaluado,
                        principalTable: "empleados",
                        principalColumn: "id_empleado",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_evaluaciones_desempeno_empleados_id_evaluador",
                        column: x => x.id_evaluador,
                        principalTable: "empleados",
                        principalColumn: "id_empleado",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_evaluaciones_desempeno_periodos_evaluacion_id_periodo_evalu~",
                        column: x => x.id_periodo_evaluacion,
                        principalTable: "periodos_evaluacion",
                        principalColumn: "id_periodo_evaluacion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "detalles_evaluacion_desempeno",
                columns: table => new
                {
                    id_detalle_evaluacion_desempeno = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_evaluacion_desempeno = table.Column<int>(type: "integer", nullable: false),
                    id_criterio_evaluacion = table.Column<int>(type: "integer", nullable: false),
                    puntuacion = table.Column<int>(type: "integer", nullable: false),
                    comentario = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ponderacion_categoria_historica = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    nombre_categoria_historica = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    texto_criterio_historico = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detalles_evaluacion_desempeno", x => x.id_detalle_evaluacion_desempeno);
                    table.ForeignKey(
                        name: "FK_detalles_evaluacion_desempeno_criterios_evaluacion_id_crite~",
                        column: x => x.id_criterio_evaluacion,
                        principalTable: "criterios_evaluacion",
                        principalColumn: "id_criterio_evaluacion",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_detalles_evaluacion_desempeno_evaluaciones_desempeno_id_eva~",
                        column: x => x.id_evaluacion_desempeno,
                        principalTable: "evaluaciones_desempeno",
                        principalColumn: "id_evaluacion_desempeno",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "categorias_evaluacion",
                columns: new[] { "id_categoria_evaluacion", "activo", "descripcion", "fecha_actualizacion", "fecha_creacion", "nombre", "orden", "ponderacion" },
                values: new object[,]
                {
                    { 1, true, "Evaluación de la productividad, calidad del trabajo y cumplimiento en tiempos establecidos.", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rendimiento", 1, 20.00m },
                    { 2, true, "Grado de consecución de metas individuales y alineación con objetivos del negocio.", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Objetivos", 2, 15.00m },
                    { 3, true, "Compromiso con las normas, puntualidad, asistencia y cuidado de recursos de la empresa.", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Responsabilidad", 3, 15.00m },
                    { 4, true, "Conocimientos técnicos, habilidades específicas del puesto y resolución de problemas.", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Competencias", 4, 15.00m },
                    { 5, true, "Trabajo en equipo, comunicación asertiva, empatía y adaptabilidad a cambios.", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Aptitudes", 5, 10.00m },
                    { 6, true, "Proactividad, autonomía para proponer mejoras y anticipación a requerimientos.", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Iniciativa", 6, 15.00m },
                    { 7, true, "Aporte de ideas innovadoras y alternativas eficaces para optimizar procesos.", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Creatividad", 7, 10.00m }
                });

            migrationBuilder.InsertData(
                table: "criterios_evaluacion",
                columns: new[] { "id_criterio_evaluacion", "activo", "descripcion", "fecha_actualizacion", "fecha_creacion", "id_categoria_evaluacion", "orden", "texto" },
                values: new object[,]
                {
                    { 1, true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, "Cumple adecuadamente con sus funciones y tareas asignadas." },
                    { 2, true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2, "Mantiene un nivel de productividad y calidad adecuado." },
                    { 3, true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 3, "Entrega su trabajo en los tiempos establecidos." },
                    { 4, true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 1, "Alcanza los objetivos y metas planteados para el período." },
                    { 5, true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 2, "Alinea sus actividades diarias con las prioridades del negocio." },
                    { 6, true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 1, "Demuestra puntualidad, asistencia y cumplimiento de horarios." },
                    { 7, true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 2, "Cuida adecuadamente las herramientas y recursos de la empresa." },
                    { 8, true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 3, "Asume la responsabilidad de sus resultados y decisiones." },
                    { 9, true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 1, "Demuestra dominio técnico en los conocimientos de su cargo." },
                    { 10, true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 2, "Aplica procedimientos y buenas prácticas en su área." },
                    { 11, true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, 1, "Trabaja en equipo y colabora activamente con sus compañeros." },
                    { 12, true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, 2, "Se comunica de forma asertiva, respetuosa y clara." },
                    { 13, true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, 1, "Actúa con autonomía y proactividad sin esperar indicaciones constantes." },
                    { 14, true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, 2, "Propone soluciones cuando se presentan problemas o dificultades." },
                    { 15, true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, 1, "Aporta ideas novedosas para simplificar o mejorar procesos." },
                    { 16, true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, 2, "Muestra flexibilidad e ingenio ante nuevos retos de trabajo." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_empleados_id_jefe_directo",
                table: "empleados",
                column: "id_jefe_directo");

            migrationBuilder.CreateIndex(
                name: "IX_categorias_evaluacion_nombre",
                table: "categorias_evaluacion",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_criterios_evaluacion_id_categoria_evaluacion",
                table: "criterios_evaluacion",
                column: "id_categoria_evaluacion");

            migrationBuilder.CreateIndex(
                name: "IX_detalles_evaluacion_desempeno_id_criterio_evaluacion",
                table: "detalles_evaluacion_desempeno",
                column: "id_criterio_evaluacion");

            migrationBuilder.CreateIndex(
                name: "IX_detalles_evaluacion_desempeno_id_evaluacion_desempeno_id_cr~",
                table: "detalles_evaluacion_desempeno",
                columns: new[] { "id_evaluacion_desempeno", "id_criterio_evaluacion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_evaluaciones_desempeno_id_empleado_evaluado",
                table: "evaluaciones_desempeno",
                column: "id_empleado_evaluado");

            migrationBuilder.CreateIndex(
                name: "IX_evaluaciones_desempeno_id_evaluador",
                table: "evaluaciones_desempeno",
                column: "id_evaluador");

            migrationBuilder.CreateIndex(
                name: "IX_evaluaciones_desempeno_id_periodo_evaluacion_id_empleado_ev~",
                table: "evaluaciones_desempeno",
                columns: new[] { "id_periodo_evaluacion", "id_empleado_evaluado", "id_evaluador", "tipo_evaluador" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_periodos_evaluacion_nombre",
                table: "periodos_evaluacion",
                column: "nombre",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_empleados_empleados_id_jefe_directo",
                table: "empleados",
                column: "id_jefe_directo",
                principalTable: "empleados",
                principalColumn: "id_empleado",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_empleados_empleados_id_jefe_directo",
                table: "empleados");

            migrationBuilder.DropTable(
                name: "detalles_evaluacion_desempeno");

            migrationBuilder.DropTable(
                name: "criterios_evaluacion");

            migrationBuilder.DropTable(
                name: "evaluaciones_desempeno");

            migrationBuilder.DropTable(
                name: "categorias_evaluacion");

            migrationBuilder.DropTable(
                name: "periodos_evaluacion");

            migrationBuilder.DropIndex(
                name: "IX_empleados_id_jefe_directo",
                table: "empleados");

            migrationBuilder.DropColumn(
                name: "id_jefe_directo",
                table: "empleados");
        }
    }
}
