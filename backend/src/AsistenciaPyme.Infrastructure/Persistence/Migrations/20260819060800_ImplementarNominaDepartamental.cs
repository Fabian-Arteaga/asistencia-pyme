using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AsistenciaPyme.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ImplementarNominaDepartamental : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_planillas_id_empleado_fecha_inicio_periodo_fecha_fin_periodo",
                table: "planillas");

            migrationBuilder.AlterColumn<decimal>(
                name: "total_deducciones",
                table: "planillas",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "salario_neto",
                table: "planillas",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "salario_base_periodo",
                table: "planillas",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "ingresos_adicionales",
                table: "planillas",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "id_empleado",
                table: "planillas",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "id_administrador",
                table: "planillas",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "CantidadEmpleados",
                table: "planillas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CerradoPor",
                table: "planillas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCierre",
                table: "planillas",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id_departamento",
                table: "planillas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "total_horas_extras",
                table: "planillas",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "total_ingresos",
                table: "planillas",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "total_neto",
                table: "planillas",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "total_salario_base",
                table: "planillas",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "salario_base",
                table: "empleados",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2);

            migrationBuilder.AddColumn<string>(
                name: "NumeroINSS",
                table: "empleados",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id_departamento",
                table: "empleados",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id_horario_laboral",
                table: "empleados",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "valor_aplicado",
                table: "deducciones_planilla",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "monto_calculado",
                table: "deducciones_planilla",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2);

            migrationBuilder.AddColumn<int>(
                name: "id_detalle_planilla",
                table: "deducciones_planilla",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "es_entrada_tardia",
                table: "asistencias",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "hora_programada_entrada",
                table: "asistencias",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "hora_programada_salida",
                table: "asistencias",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "minutos_tardanza",
                table: "asistencias",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ConceptosPlanilla",
                columns: table => new
                {
                    IdConceptoPlanilla = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConceptosPlanilla", x => x.IdConceptoPlanilla);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionesNomina",
                columns: table => new
                {
                    IdConfiguracionNomina = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MinutosToleranciaEntrada = table.Column<int>(type: "integer", nullable: false),
                    MultiplicadorHoraExtra = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DiasVacacionesPorMes = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DiasBaseProrrateoVacaciones = table.Column<int>(type: "integer", nullable: false),
                    PoliticaDescuentoTardanza = table.Column<int>(type: "integer", nullable: false),
                    MinutosMaximosVerificacionDepartamento = table.Column<int>(type: "integer", nullable: false),
                    TasaINSS = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesNomina", x => x.IdConfiguracionNomina);
                });

            migrationBuilder.CreateTable(
                name: "Departamentos",
                columns: table => new
                {
                    IdDepartamento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamentos", x => x.IdDepartamento);
                });

            migrationBuilder.CreateTable(
                name: "DetallesPlanilla",
                columns: table => new
                {
                    IdDetallePlanilla = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdPlanilla = table.Column<int>(type: "integer", nullable: false),
                    IdEmpleado = table.Column<int>(type: "integer", nullable: false),
                    CodigoEmpleado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    NombreEmpleado = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    NumeroINSS = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Cargo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Departamento = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    SalarioBase = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DiasLaborados = table.Column<int>(type: "integer", nullable: false),
                    MinutosLaborados = table.Column<int>(type: "integer", nullable: false),
                    CantidadTardanzas = table.Column<int>(type: "integer", nullable: false),
                    MinutosTardanza = table.Column<int>(type: "integer", nullable: false),
                    DescuentoTardanza = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    MinutosExtrasDetectados = table.Column<int>(type: "integer", nullable: false),
                    MinutosExtrasAprobados = table.Column<int>(type: "integer", nullable: false),
                    MontoHorasExtras = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    VacacionesAcumuladasPeriodo = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SaldoVacaciones = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalIngresos = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalDeducciones = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SalarioNeto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IndemnizacionProyectada = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesPlanilla", x => x.IdDetallePlanilla);
                    table.ForeignKey(
                        name: "FK_DetallesPlanilla_empleados_IdEmpleado",
                        column: x => x.IdEmpleado,
                        principalTable: "empleados",
                        principalColumn: "id_empleado",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetallesPlanilla_planillas_IdPlanilla",
                        column: x => x.IdPlanilla,
                        principalTable: "planillas",
                        principalColumn: "id_planilla",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Embargos",
                columns: table => new
                {
                    IdEmbargo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpleado = table.Column<int>(type: "integer", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TipoCalculo = table.Column<int>(type: "integer", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Porcentaje = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    SaldoPendiente = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    Referencia = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Observacion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Embargos", x => x.IdEmbargo);
                    table.ForeignKey(
                        name: "FK_Embargos_empleados_IdEmpleado",
                        column: x => x.IdEmpleado,
                        principalTable: "empleados",
                        principalColumn: "id_empleado",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HorariosLaborales",
                columns: table => new
                {
                    IdHorarioLaboral = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    HoraEntrada = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    HoraSalida = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    DiasLaborales = table.Column<string>(type: "text", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorariosLaborales", x => x.IdHorarioLaboral);
                });

            migrationBuilder.CreateTable(
                name: "HorasExtras",
                columns: table => new
                {
                    IdHoraExtra = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpleado = table.Column<int>(type: "integer", nullable: false),
                    IdAsistencia = table.Column<int>(type: "integer", nullable: true),
                    Fecha = table.Column<DateTime>(type: "date", nullable: false),
                    MinutosDetectados = table.Column<int>(type: "int", nullable: false),
                    MinutosAprobados = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    AprobadoPor = table.Column<int>(type: "integer", nullable: true),
                    FechaAprobacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Observacion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorasExtras", x => x.IdHoraExtra);
                    table.ForeignKey(
                        name: "FK_HorasExtras_asistencias_IdAsistencia",
                        column: x => x.IdAsistencia,
                        principalTable: "asistencias",
                        principalColumn: "id_asistencia",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_HorasExtras_empleados_IdEmpleado",
                        column: x => x.IdEmpleado,
                        principalTable: "empleados",
                        principalColumn: "id_empleado",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DispositivosMarcaje",
                columns: table => new
                {
                    IdDispositivoMarcaje = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    IdDepartamento = table.Column<int>(type: "integer", nullable: true),
                    Identificador = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispositivosMarcaje", x => x.IdDispositivoMarcaje);
                    table.ForeignKey(
                        name: "FK_DispositivosMarcaje_Departamentos_IdDepartamento",
                        column: x => x.IdDepartamento,
                        principalTable: "Departamentos",
                        principalColumn: "IdDepartamento",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EmpleadoDepartamentoHistorials",
                columns: table => new
                {
                    IdEmpleadoDepartamentoHistorial = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpleado = table.Column<int>(type: "integer", nullable: false),
                    IdDepartamento = table.Column<int>(type: "integer", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadoDepartamentoHistorials", x => x.IdEmpleadoDepartamentoHistorial);
                    table.ForeignKey(
                        name: "FK_EmpleadoDepartamentoHistorials_Departamentos_IdDepartamento",
                        column: x => x.IdDepartamento,
                        principalTable: "Departamentos",
                        principalColumn: "IdDepartamento",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpleadoDepartamentoHistorials_empleados_IdEmpleado",
                        column: x => x.IdEmpleado,
                        principalTable: "empleados",
                        principalColumn: "id_empleado",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetallesConceptoPlanilla",
                columns: table => new
                {
                    IdDetalleConceptoPlanilla = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdDetallePlanilla = table.Column<int>(type: "integer", nullable: false),
                    IdConceptoPlanilla = table.Column<int>(type: "integer", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesConceptoPlanilla", x => x.IdDetalleConceptoPlanilla);
                    table.ForeignKey(
                        name: "FK_DetallesConceptoPlanilla_ConceptosPlanilla_IdConceptoPlanil~",
                        column: x => x.IdConceptoPlanilla,
                        principalTable: "ConceptosPlanilla",
                        principalColumn: "IdConceptoPlanilla",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetallesConceptoPlanilla_DetallesPlanilla_IdDetallePlanilla",
                        column: x => x.IdDetallePlanilla,
                        principalTable: "DetallesPlanilla",
                        principalColumn: "IdDetallePlanilla",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VerificacionesPresencia",
                columns: table => new
                {
                    IdVerificacionPresencia = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpleado = table.Column<int>(type: "integer", nullable: false),
                    IdAsistencia = table.Column<int>(type: "integer", nullable: true),
                    IdDepartamento = table.Column<int>(type: "integer", nullable: true),
                    IdDispositivoMarcaje = table.Column<int>(type: "integer", nullable: true),
                    FechaHora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificacionesPresencia", x => x.IdVerificacionPresencia);
                    table.ForeignKey(
                        name: "FK_VerificacionesPresencia_Departamentos_IdDepartamento",
                        column: x => x.IdDepartamento,
                        principalTable: "Departamentos",
                        principalColumn: "IdDepartamento",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VerificacionesPresencia_DispositivosMarcaje_IdDispositivoMa~",
                        column: x => x.IdDispositivoMarcaje,
                        principalTable: "DispositivosMarcaje",
                        principalColumn: "IdDispositivoMarcaje",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VerificacionesPresencia_asistencias_IdAsistencia",
                        column: x => x.IdAsistencia,
                        principalTable: "asistencias",
                        principalColumn: "id_asistencia",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VerificacionesPresencia_empleados_IdEmpleado",
                        column: x => x.IdEmpleado,
                        principalTable: "empleados",
                        principalColumn: "id_empleado",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ConceptosPlanilla",
                columns: new[] { "IdConceptoPlanilla", "Activo", "Codigo", "Nombre", "Tipo" },
                values: new object[,]
                {
                    { 1, true, "SALARIO_BASE", "Salario Base", 1 },
                    { 2, true, "HORA_EXTRA", "Hora Extra", 1 },
                    { 3, true, "INSS", "INSS", 2 },
                    { 4, true, "EMBARGO", "Embargo", 2 },
                    { 5, true, "TARDANZA", "Tardanza", 2 },
                    { 6, true, "BONIFICACION", "Bonificación", 1 },
                    { 7, true, "OTRA_DEDUCCION", "Otra Deducción", 2 }
                });

            migrationBuilder.InsertData(
                table: "ConfiguracionesNomina",
                columns: new[] { "IdConfiguracionNomina", "DiasBaseProrrateoVacaciones", "DiasVacacionesPorMes", "FechaActualizacion", "FechaCreacion", "MinutosMaximosVerificacionDepartamento", "MinutosToleranciaEntrada", "MultiplicadorHoraExtra", "PoliticaDescuentoTardanza", "TasaINSS" },
                values: new object[] { 1, 30, 2.5m, null, new DateTime(2026, 8, 19, 0, 0, 0, 0, DateTimeKind.Utc), 10, 10, 2.0m, 1, null });

            migrationBuilder.CreateIndex(
                name: "IX_planillas_id_departamento_fecha_inicio_periodo_fecha_fin_pe~",
                table: "planillas",
                columns: new[] { "id_departamento", "fecha_inicio_periodo", "fecha_fin_periodo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_planillas_id_empleado",
                table: "planillas",
                column: "id_empleado");

            migrationBuilder.CreateIndex(
                name: "IX_empleados_id_departamento",
                table: "empleados",
                column: "id_departamento");

            migrationBuilder.CreateIndex(
                name: "IX_empleados_id_horario_laboral",
                table: "empleados",
                column: "id_horario_laboral");

            migrationBuilder.CreateIndex(
                name: "IX_empleados_NumeroINSS",
                table: "empleados",
                column: "NumeroINSS",
                unique: true,
                filter: "\"NumeroINSS\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_deducciones_planilla_id_detalle_planilla",
                table: "deducciones_planilla",
                column: "id_detalle_planilla");

            migrationBuilder.CreateIndex(
                name: "IX_ConceptosPlanilla_Codigo",
                table: "ConceptosPlanilla",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetallesConceptoPlanilla_IdConceptoPlanilla",
                table: "DetallesConceptoPlanilla",
                column: "IdConceptoPlanilla");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesConceptoPlanilla_IdDetallePlanilla",
                table: "DetallesConceptoPlanilla",
                column: "IdDetallePlanilla");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPlanilla_IdEmpleado",
                table: "DetallesPlanilla",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPlanilla_IdPlanilla",
                table: "DetallesPlanilla",
                column: "IdPlanilla");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPlanilla_IdPlanilla_IdEmpleado",
                table: "DetallesPlanilla",
                columns: new[] { "IdPlanilla", "IdEmpleado" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DispositivosMarcaje_IdDepartamento",
                table: "DispositivosMarcaje",
                column: "IdDepartamento");

            migrationBuilder.CreateIndex(
                name: "IX_Embargos_IdEmpleado_Activo",
                table: "Embargos",
                columns: new[] { "IdEmpleado", "Activo" });

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoDepartamentoHistorials_IdDepartamento",
                table: "EmpleadoDepartamentoHistorials",
                column: "IdDepartamento");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoDepartamentoHistorials_IdEmpleado",
                table: "EmpleadoDepartamentoHistorials",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_HorasExtras_IdAsistencia",
                table: "HorasExtras",
                column: "IdAsistencia");

            migrationBuilder.CreateIndex(
                name: "IX_HorasExtras_IdEmpleado_Fecha",
                table: "HorasExtras",
                columns: new[] { "IdEmpleado", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_VerificacionesPresencia_IdAsistencia",
                table: "VerificacionesPresencia",
                column: "IdAsistencia");

            migrationBuilder.CreateIndex(
                name: "IX_VerificacionesPresencia_IdDepartamento",
                table: "VerificacionesPresencia",
                column: "IdDepartamento");

            migrationBuilder.CreateIndex(
                name: "IX_VerificacionesPresencia_IdDispositivoMarcaje",
                table: "VerificacionesPresencia",
                column: "IdDispositivoMarcaje");

            migrationBuilder.CreateIndex(
                name: "IX_VerificacionesPresencia_IdEmpleado",
                table: "VerificacionesPresencia",
                column: "IdEmpleado");

            migrationBuilder.AddForeignKey(
                name: "FK_deducciones_planilla_DetallesPlanilla_id_detalle_planilla",
                table: "deducciones_planilla",
                column: "id_detalle_planilla",
                principalTable: "DetallesPlanilla",
                principalColumn: "IdDetallePlanilla",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_empleados_Departamentos_id_departamento",
                table: "empleados",
                column: "id_departamento",
                principalTable: "Departamentos",
                principalColumn: "IdDepartamento",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_empleados_HorariosLaborales_id_horario_laboral",
                table: "empleados",
                column: "id_horario_laboral",
                principalTable: "HorariosLaborales",
                principalColumn: "IdHorarioLaboral",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_planillas_Departamentos_id_departamento",
                table: "planillas",
                column: "id_departamento",
                principalTable: "Departamentos",
                principalColumn: "IdDepartamento",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM planillas
                        WHERE id_empleado IS NULL
                           OR id_administrador IS NULL
                           OR id_departamento IS NOT NULL
                    ) THEN
                        RAISE EXCEPTION 'No es posible revertir la migración porque existen planillas departamentales incompatibles con el modelo anterior.';
                    END IF;
                END
                $$;
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_deducciones_planilla_DetallesPlanilla_id_detalle_planilla",
                table: "deducciones_planilla");

            migrationBuilder.DropForeignKey(
                name: "FK_empleados_Departamentos_id_departamento",
                table: "empleados");

            migrationBuilder.DropForeignKey(
                name: "FK_empleados_HorariosLaborales_id_horario_laboral",
                table: "empleados");

            migrationBuilder.DropForeignKey(
                name: "FK_planillas_Departamentos_id_departamento",
                table: "planillas");

            migrationBuilder.DropTable(
                name: "ConfiguracionesNomina");

            migrationBuilder.DropTable(
                name: "DetallesConceptoPlanilla");

            migrationBuilder.DropTable(
                name: "Embargos");

            migrationBuilder.DropTable(
                name: "EmpleadoDepartamentoHistorials");

            migrationBuilder.DropTable(
                name: "HorariosLaborales");

            migrationBuilder.DropTable(
                name: "HorasExtras");

            migrationBuilder.DropTable(
                name: "VerificacionesPresencia");

            migrationBuilder.DropTable(
                name: "ConceptosPlanilla");

            migrationBuilder.DropTable(
                name: "DetallesPlanilla");

            migrationBuilder.DropTable(
                name: "DispositivosMarcaje");

            migrationBuilder.DropTable(
                name: "Departamentos");

            migrationBuilder.DropIndex(
                name: "IX_planillas_id_departamento_fecha_inicio_periodo_fecha_fin_pe~",
                table: "planillas");

            migrationBuilder.DropIndex(
                name: "IX_planillas_id_empleado",
                table: "planillas");

            migrationBuilder.DropIndex(
                name: "IX_empleados_id_departamento",
                table: "empleados");

            migrationBuilder.DropIndex(
                name: "IX_empleados_id_horario_laboral",
                table: "empleados");

            migrationBuilder.DropIndex(
                name: "IX_empleados_NumeroINSS",
                table: "empleados");

            migrationBuilder.DropIndex(
                name: "IX_deducciones_planilla_id_detalle_planilla",
                table: "deducciones_planilla");

            migrationBuilder.DropColumn(
                name: "CantidadEmpleados",
                table: "planillas");

            migrationBuilder.DropColumn(
                name: "CerradoPor",
                table: "planillas");

            migrationBuilder.DropColumn(
                name: "FechaCierre",
                table: "planillas");

            migrationBuilder.DropColumn(
                name: "id_departamento",
                table: "planillas");

            migrationBuilder.DropColumn(
                name: "total_horas_extras",
                table: "planillas");

            migrationBuilder.DropColumn(
                name: "total_ingresos",
                table: "planillas");

            migrationBuilder.DropColumn(
                name: "total_neto",
                table: "planillas");

            migrationBuilder.DropColumn(
                name: "total_salario_base",
                table: "planillas");

            migrationBuilder.DropColumn(
                name: "NumeroINSS",
                table: "empleados");

            migrationBuilder.DropColumn(
                name: "id_departamento",
                table: "empleados");

            migrationBuilder.DropColumn(
                name: "id_horario_laboral",
                table: "empleados");

            migrationBuilder.DropColumn(
                name: "id_detalle_planilla",
                table: "deducciones_planilla");

            migrationBuilder.DropColumn(
                name: "es_entrada_tardia",
                table: "asistencias");

            migrationBuilder.DropColumn(
                name: "hora_programada_entrada",
                table: "asistencias");

            migrationBuilder.DropColumn(
                name: "hora_programada_salida",
                table: "asistencias");

            migrationBuilder.DropColumn(
                name: "minutos_tardanza",
                table: "asistencias");

            migrationBuilder.AlterColumn<decimal>(
                name: "total_deducciones",
                table: "planillas",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "salario_neto",
                table: "planillas",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "salario_base_periodo",
                table: "planillas",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "ingresos_adicionales",
                table: "planillas",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "id_empleado",
                table: "planillas",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id_administrador",
                table: "planillas",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "salario_base",
                table: "empleados",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "valor_aplicado",
                table: "deducciones_planilla",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "monto_calculado",
                table: "deducciones_planilla",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.CreateIndex(
                name: "IX_planillas_id_empleado_fecha_inicio_periodo_fecha_fin_periodo",
                table: "planillas",
                columns: new[] { "id_empleado", "fecha_inicio_periodo", "fecha_fin_periodo" },
                unique: true);
        }
    }
}
