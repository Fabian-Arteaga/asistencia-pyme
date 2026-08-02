using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AsistenciaPyme.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "administradores",
                columns: table => new
                {
                    id_administrador = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombres = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    apellidos = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    correo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    contrasena_hash = table.Column<string>(type: "text", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_actualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_administradores", x => x.id_administrador);
                });

            migrationBuilder.CreateTable(
                name: "cargos",
                columns: table => new
                {
                    id_cargo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    funciones = table.Column<string>(type: "text", nullable: false),
                    responsabilidades = table.Column<string>(type: "text", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_actualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cargos", x => x.id_cargo);
                });

            migrationBuilder.CreateTable(
                name: "tipos_deduccion",
                columns: table => new
                {
                    id_tipo_deduccion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    tipo_calculo = table.Column<int>(type: "integer", nullable: false),
                    valor_predeterminado = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_actualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipos_deduccion", x => x.id_tipo_deduccion);
                });

            migrationBuilder.CreateTable(
                name: "empleados",
                columns: table => new
                {
                    id_empleado = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_cargo = table.Column<int>(type: "integer", nullable: false),
                    codigo_empleado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    pin_hash = table.Column<string>(type: "text", nullable: false),
                    identificacion = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    nombres = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    apellidos = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    correo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    direccion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    fecha_contratacion = table.Column<DateOnly>(type: "date", nullable: false),
                    salario_base = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_actualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_empleados", x => x.id_empleado);
                    table.ForeignKey(
                        name: "FK_empleados_cargos_id_cargo",
                        column: x => x.id_cargo,
                        principalTable: "cargos",
                        principalColumn: "id_cargo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "asistencias",
                columns: table => new
                {
                    id_asistencia = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_empleado = table.Column<int>(type: "integer", nullable: false),
                    hora_entrada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    hora_salida = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    observacion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    corregida = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    motivo_correccion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    id_administrador = table.Column<int>(type: "integer", nullable: true),
                    fecha_correccion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_actualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asistencias", x => x.id_asistencia);
                    table.ForeignKey(
                        name: "FK_asistencias_administradores_id_administrador",
                        column: x => x.id_administrador,
                        principalTable: "administradores",
                        principalColumn: "id_administrador",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_asistencias_empleados_id_empleado",
                        column: x => x.id_empleado,
                        principalTable: "empleados",
                        principalColumn: "id_empleado",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "planillas",
                columns: table => new
                {
                    id_planilla = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_empleado = table.Column<int>(type: "integer", nullable: false),
                    id_administrador = table.Column<int>(type: "integer", nullable: false),
                    fecha_inicio_periodo = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_fin_periodo = table.Column<DateOnly>(type: "date", nullable: false),
                    salario_base_periodo = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    ingresos_adicionales = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0m),
                    total_deducciones = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0m),
                    salario_neto = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0m),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    fecha_generacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_actualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planillas", x => x.id_planilla);
                    table.ForeignKey(
                        name: "FK_planillas_administradores_id_administrador",
                        column: x => x.id_administrador,
                        principalTable: "administradores",
                        principalColumn: "id_administrador",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_planillas_empleados_id_empleado",
                        column: x => x.id_empleado,
                        principalTable: "empleados",
                        principalColumn: "id_empleado",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vacaciones",
                columns: table => new
                {
                    id_vacacion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_empleado = table.Column<int>(type: "integer", nullable: false),
                    id_administrador = table.Column<int>(type: "integer", nullable: false),
                    fecha_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_fin = table.Column<DateOnly>(type: "date", nullable: false),
                    motivo = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    observacion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    cancelada = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_actualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vacaciones", x => x.id_vacacion);
                    table.ForeignKey(
                        name: "FK_vacaciones_administradores_id_administrador",
                        column: x => x.id_administrador,
                        principalTable: "administradores",
                        principalColumn: "id_administrador",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vacaciones_empleados_id_empleado",
                        column: x => x.id_empleado,
                        principalTable: "empleados",
                        principalColumn: "id_empleado",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "deducciones_planilla",
                columns: table => new
                {
                    id_deduccion_planilla = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_planilla = table.Column<int>(type: "integer", nullable: false),
                    id_tipo_deduccion = table.Column<int>(type: "integer", nullable: false),
                    valor_aplicado = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    monto_calculado = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    observacion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deducciones_planilla", x => x.id_deduccion_planilla);
                    table.ForeignKey(
                        name: "FK_deducciones_planilla_planillas_id_planilla",
                        column: x => x.id_planilla,
                        principalTable: "planillas",
                        principalColumn: "id_planilla",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_deducciones_planilla_tipos_deduccion_id_tipo_deduccion",
                        column: x => x.id_tipo_deduccion,
                        principalTable: "tipos_deduccion",
                        principalColumn: "id_tipo_deduccion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_administradores_correo",
                table: "administradores",
                column: "correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_asistencias_id_administrador",
                table: "asistencias",
                column: "id_administrador");

            migrationBuilder.CreateIndex(
                name: "IX_asistencias_id_empleado_hora_entrada",
                table: "asistencias",
                columns: new[] { "id_empleado", "hora_entrada" });

            migrationBuilder.CreateIndex(
                name: "IX_cargos_nombre",
                table: "cargos",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_deducciones_planilla_id_planilla_id_tipo_deduccion",
                table: "deducciones_planilla",
                columns: new[] { "id_planilla", "id_tipo_deduccion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_deducciones_planilla_id_tipo_deduccion",
                table: "deducciones_planilla",
                column: "id_tipo_deduccion");

            migrationBuilder.CreateIndex(
                name: "IX_empleados_codigo_empleado",
                table: "empleados",
                column: "codigo_empleado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_empleados_correo",
                table: "empleados",
                column: "correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_empleados_id_cargo",
                table: "empleados",
                column: "id_cargo");

            migrationBuilder.CreateIndex(
                name: "IX_empleados_identificacion",
                table: "empleados",
                column: "identificacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_planillas_id_administrador",
                table: "planillas",
                column: "id_administrador");

            migrationBuilder.CreateIndex(
                name: "IX_planillas_id_empleado_fecha_inicio_periodo_fecha_fin_periodo",
                table: "planillas",
                columns: new[] { "id_empleado", "fecha_inicio_periodo", "fecha_fin_periodo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tipos_deduccion_nombre",
                table: "tipos_deduccion",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vacaciones_id_administrador",
                table: "vacaciones",
                column: "id_administrador");

            migrationBuilder.CreateIndex(
                name: "IX_vacaciones_id_empleado_fecha_inicio_fecha_fin",
                table: "vacaciones",
                columns: new[] { "id_empleado", "fecha_inicio", "fecha_fin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "asistencias");

            migrationBuilder.DropTable(
                name: "deducciones_planilla");

            migrationBuilder.DropTable(
                name: "vacaciones");

            migrationBuilder.DropTable(
                name: "planillas");

            migrationBuilder.DropTable(
                name: "tipos_deduccion");

            migrationBuilder.DropTable(
                name: "administradores");

            migrationBuilder.DropTable(
                name: "empleados");

            migrationBuilder.DropTable(
                name: "cargos");
        }
    }
}
