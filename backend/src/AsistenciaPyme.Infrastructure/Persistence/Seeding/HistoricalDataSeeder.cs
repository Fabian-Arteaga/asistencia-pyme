using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Domain.Entities;
using AsistenciaPyme.Domain.Enums;
using AsistenciaPyme.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsistenciaPyme.Infrastructure.Persistence.Seeding;

public class HistoricalDataSeeder : IHistoricalDataSeeder
{
    private readonly AsistenciaPymeDbContext _context;
    private readonly IPinHasher _pinHasher;
    private readonly IContrasenaHasher _contrasenaHasher;
    private readonly ICalculadorHorasExtras _calculadorHorasExtras;

    // Semilla determinista fija para reproducibilidad exacta
    private const int RandomSeed = 2014;
    private static readonly DateOnly FechaInicioHistorico = new(2014, 1, 1);
    private static readonly DateOnly FechaCorteHistorico = new(2026, 8, 19);

    // Parámetro centralizado de volumen para demostración histórica (~2,000 asistencias)
    public const int TargetHistoricalAttendanceCount = 2000;

    public HistoricalDataSeeder(
        AsistenciaPymeDbContext context,
        IPinHasher pinHasher,
        IContrasenaHasher contrasenaHasher,
        ICalculadorHorasExtras calculadorHorasExtras)
    {
        _context = context;
        _pinHasher = pinHasher;
        _contrasenaHasher = contrasenaHasher;
        _calculadorHorasExtras = calculadorHorasExtras;
    }

    public async Task SeedHistoricalDataAsync(CancellationToken cancellationToken = default)
    {
        var rng = new Random(RandomSeed);

        // 1. Verificación de Idempotencia
        bool yaExisteHistorico = await _context.PeriodosEvaluacion.AnyAsync(p => p.Nombre == "2014-I", cancellationToken)
            && await _context.Empleados.AnyAsync(e => e.CodigoEmpleado == "EMP-2014-01", cancellationToken);

        if (yaExisteHistorico)
        {
            Console.WriteLine("[SEEDER] El dataset histórico 2014-2026 ya se encuentra generado en la base de datos. Operación omitida por idempotencia.");
            return;
        }

        Console.WriteLine("================================================================================");
        Console.WriteLine("[SEEDER] Iniciando generación de dataset histórico AsistenciaPyme (2014 - 2026)");
        Console.WriteLine($"[SEEDER] Volumen objetivo de asistencias históricas: ~{TargetHistoricalAttendanceCount}");
        Console.WriteLine("================================================================================");

        // Fase 1: Catálogos Base (Administrador, Departamentos, Cargos, Horarios, etc.)
        var admin = await GarantizarAdministradorAsync(cancellationToken);
        var departamentos = await GarantizarDepartamentosAsync(cancellationToken);
        var cargos = await GarantizarCargosAsync(cancellationToken);
        var horarios = await GarantizarHorariosAsync(cancellationToken);
        var tiposDeduccion = await GarantizarTiposDeduccionAsync(cancellationToken);
        var configNomina = await GarantizarConfiguracionNominaAsync(cancellationToken);
        var categorias = await _context.CategoriasEvaluacion.Include(c => c.Criterios).OrderBy(c => c.Orden).ToListAsync(cancellationToken);

        Console.WriteLine("[SEEDER] Fase 1 completada: Catálogos base verificados.");

        // Fase 2: Empleados Históricos y Estructura Organizacional (29 colaboradores)
        var empleadosDef = DefinirEmpleados(departamentos, cargos, horarios);
        var empleadosCreados = await SembrarEmpleadosAsync(empleadosDef, cancellationToken);
        Console.WriteLine($"[SEEDER] Fase 2 completada: {empleadosCreados.Count} empleados históricos registrados con jerarquía organizacional.");

        // Fase 3: Vacaciones Históricas
        var vacacionesCreadas = await SembrarVacacionesAsync(empleadosDef, admin.IdAdministrador, rng, cancellationToken);
        Console.WriteLine($"[SEEDER] Fase 3 completada: {vacacionesCreadas.Count} solicitudes de vacaciones históricas registradas.");

        // Fase 4: Asistencias y Horas Extras Muestreadas (2014 - 2026, ~2,000 registros)
        var (totalAsistencias, totalHorasExtras) = await SembrarAsistenciasYHorasExtrasAsync(empleadosDef, vacacionesCreadas, rng, cancellationToken);
        Console.WriteLine($"[SEEDER] Fase 4 completada: {totalAsistencias} asistencias y {totalHorasExtras} horas extras generadas.");

        // Fase 5: Planillas Departamentales Históricas
        int totalPlanillas = await SembrarPlanillasDepartamentalesAsync(departamentos, empleadosDef, admin.IdAdministrador, configNomina.MultiplicadorHoraExtra, cancellationToken);
        Console.WriteLine($"[SEEDER] Fase 5 completada: {totalPlanillas} planillas departamentales generadas y calculadas.");

        // Fase 6: Períodos Semestrales y Evaluaciones 360° con Snapshot Histórico
        var (totalPeriodos, totalEvaluaciones, totalDetalles) = await SembrarEvaluaciones360Async(empleadosDef, categorias, rng, cancellationToken);
        Console.WriteLine($"[SEEDER] Fase 6 completada: {totalPeriodos} períodos semestrales, {totalEvaluaciones} evaluaciones 360° y {totalDetalles} respuestas con snapshot histórico.");

        Console.WriteLine("================================================================================");
        Console.WriteLine("[SEEDER] ¡Generación histórica completada con éxito y total integridad!");
        Console.WriteLine("================================================================================");
    }

    #region Fase 1: Catálogos Base

    private async Task<Administrador> GarantizarAdministradorAsync(CancellationToken ct)
    {
        var admin = await _context.Administradores.FirstOrDefaultAsync(a => a.Correo == "admin@asistenciapyme.com", ct);
        if (admin == null)
        {
            admin = new Administrador
            {
                Nombres = "Administrador",
                Apellidos = "Principal",
                Correo = "admin@asistenciapyme.com",
                ContrasenaHash = _contrasenaHasher.CrearHash("Admin1234!"),
                Activo = true,
                FechaCreacion = new DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };
            await _context.Administradores.AddAsync(admin, ct);
            await _context.SaveChangesAsync(ct);
        }
        return admin;
    }

    private async Task<Dictionary<string, Departamento>> GarantizarDepartamentosAsync(CancellationToken ct)
    {
        var nombresDeptos = new[]
        {
            ("Dirección Ejecutiva", "Dirección general y gestión estratégica empresarial."),
            ("Tecnología de la Información", "Desarrollo de software, infraestructura y soporte técnico."),
            ("Recursos Humanos", "Gestión del talento humano, nómina y bienestar organizacional."),
            ("Finanzas y Contabilidad", "Gestión contable, tesorería, auditoría y control financiero."),
            ("Operaciones y Ventas", "Gestión comercial, atención al cliente y operaciones de campo.")
        };

        var mapa = new Dictionary<string, Departamento>();
        foreach (var (nombre, desc) in nombresDeptos)
        {
            var depto = await _context.Departamentos.FirstOrDefaultAsync(d => d.Nombre == nombre, ct);
            if (depto == null)
            {
                depto = new Departamento
                {
                    Nombre = nombre,
                    Descripcion = desc,
                    Activo = true,
                    FechaCreacion = new DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                };
                await _context.Departamentos.AddAsync(depto, ct);
                await _context.SaveChangesAsync(ct);
            }
            mapa[nombre] = depto;
        }
        return mapa;
    }

    private async Task<Dictionary<string, Cargo>> GarantizarCargosAsync(CancellationToken ct)
    {
        var listaCargos = new[]
        {
            ("Director General", "Liderazgo y dirección de la empresa", "Definir visión estratégica", "Representación legal"),
            ("Jefe de TI", "Liderazgo de tecnología y sistemas", "Planificar proyectos de software", "Gestión del equipo técnico"),
            ("Jefe de Recursos Humanos", "Liderazgo de gestión humana", "Supervisar nómina y selección", "Cumplimiento laboral"),
            ("Jefe de Finanzas", "Liderazgo financiero y contable", "Planificar presupuestos", "Reportes financieros"),
            ("Jefe de Operaciones", "Liderazgo de ventas y operaciones", "Gestionar cartera de clientes", "Metas comerciales"),
            ("Desarrollador Senior", "Desarrollo de software avanzado", "Arquitectura e implementación", "Calidad de código"),
            ("Desarrollador Full Stack", "Desarrollo frontend y backend", "Construcción de módulos", "Mantenimiento"),
            ("Desarrollador Frontend", "Construcción de interfaces web", "Diseño de vistas y usabilidad", "Interacción UI"),
            ("Desarrollador Backend", "Lógica de negocio y APIs", "Servicios web y base de datos", "Seguridad"),
            ("Diseñador UI/UX", "Diseño de experiencia de usuario", "Prototipado e interfaces", "Guías visuales"),
            ("QA Tester", "Aseguramiento de calidad", "Pruebas funcionales y automatizadas", "Reporte de bugs"),
            ("Contador General", "Contabilidad empresarial", "Estados financieros y balances", "Cumplimiento tributario"),
            ("Asistente Contable", "Apoyo en registros contables", "Conciliaciones y facturación", "Archivo contable"),
            ("Analista Financiero", "Análisis y proyecciones", "Evaluación de costos y flujo", "Métricas financieras"),
            ("Analista de Nómina", "Cálculo y gestión de planillas", "Procesamiento de pagos e INSS", "Atención a empleados"),
            ("Especialista de Reclutamiento", "Atracción de talento", "Entrevistas y selección", "Inducción laboral"),
            ("Especialista en Bienestar", "Clima organizacional", "Actividades y salud ocupacional", "Bienestar"),
            ("Supervisor de Ventas", "Supervisión del equipo comercial", "Seguimiento de metas", "Atención a cuentas"),
            ("Ejecutivo de Cuentas Clave", "Gestión de grandes clientes", "Fidelización y ventas", "Contratos comerciales"),
            ("Ejecutivo de Ventas", "Venta directa de servicios", "Prospección y cierre", "Atención al cliente"),
            ("Técnico de Operaciones", "Soporte operativo y logística", "Gestión de incidencias", "Operación diaria"),
            ("Pasante de TI", "Aprendizaje y apoyo técnico", "Tareas básicas de desarrollo", "Soporte inicial")
        };

        var mapa = new Dictionary<string, Cargo>();
        foreach (var (nombre, desc, func, resp) in listaCargos)
        {
            var cargo = await _context.Cargos.FirstOrDefaultAsync(c => c.Nombre == nombre, ct);
            if (cargo == null)
            {
                cargo = new Cargo
                {
                    Nombre = nombre,
                    Descripcion = desc,
                    Funciones = func,
                    Responsabilidades = resp,
                    Activo = true,
                    FechaCreacion = new DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                };
                await _context.Cargos.AddAsync(cargo, ct);
                await _context.SaveChangesAsync(ct);
            }
            mapa[nombre] = cargo;
        }
        return mapa;
    }

    private async Task<Dictionary<string, HorarioLaboral>> GarantizarHorariosAsync(CancellationToken ct)
    {
        var horarios = new[]
        {
            ("Horario Administrativo", new TimeOnly(8, 0), new TimeOnly(17, 0), "Lunes,Martes,Miercoles,Jueves,Viernes"),
            ("Horario Operativo", new TimeOnly(7, 30), new TimeOnly(16, 30), "Lunes,Martes,Miercoles,Jueves,Viernes,Sabado")
        };

        var mapa = new Dictionary<string, HorarioLaboral>();
        foreach (var (nombre, entrada, salida, dias) in horarios)
        {
            var horario = await _context.HorariosLaborales.FirstOrDefaultAsync(h => h.Nombre == nombre, ct);
            if (horario == null)
            {
                horario = new HorarioLaboral
                {
                    Nombre = nombre,
                    HoraEntrada = entrada,
                    HoraSalida = salida,
                    DiasLaborales = dias,
                    Activo = true
                };
                await _context.HorariosLaborales.AddAsync(horario, ct);
                await _context.SaveChangesAsync(ct);
            }
            mapa[nombre] = horario;
        }
        return mapa;
    }

    private async Task<List<TipoDeduccion>> GarantizarTiposDeduccionAsync(CancellationToken ct)
    {
        var tipos = new[]
        {
            ("INSS Laboral", "Deducción legal del seguro social", TipoCalculoDeduccion.Porcentaje, 7.00m),
            ("Anticipo de Salario", "Deducción por adelanto de nómina", TipoCalculoDeduccion.Fijo, 0.00m),
            ("Préstamo Empresarial", "Cuota de amortización de préstamo", TipoCalculoDeduccion.Fijo, 0.00m),
            ("Seguro Médico Colectivo", "Aporte complementario de salud", TipoCalculoDeduccion.Fijo, 350.00m)
        };

        var lista = new List<TipoDeduccion>();
        foreach (var (nombre, desc, tipoCalc, val) in tipos)
        {
            var tipo = await _context.TiposDeduccion.FirstOrDefaultAsync(t => t.Nombre == nombre, ct);
            if (tipo == null)
            {
                tipo = new TipoDeduccion
                {
                    Nombre = nombre,
                    Descripcion = desc,
                    TipoCalculo = tipoCalc,
                    ValorPredeterminado = val,
                    Activo = true,
                    FechaCreacion = new DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                };
                await _context.TiposDeduccion.AddAsync(tipo, ct);
                await _context.SaveChangesAsync(ct);
            }
            lista.Add(tipo);
        }
        return lista;
    }

    private async Task<ConfiguracionNomina> GarantizarConfiguracionNominaAsync(CancellationToken ct)
    {
        var config = await _context.ConfiguracionesNomina.FirstOrDefaultAsync(ct);
        if (config == null)
        {
            config = new ConfiguracionNomina
            {
                MinutosToleranciaEntrada = 10,
                MultiplicadorHoraExtra = 2.0m,
                DiasVacacionesPorMes = 2.5m,
                DiasBaseProrrateoVacaciones = 30,
                PoliticaDescuentoTardanza = PoliticaDescuentoTardanza.SinDeduccion,
                MinutosMaximosVerificacionDepartamento = 10,
                TasaINSS = 7.0m,
                FechaCreacion = new DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };
            await _context.ConfiguracionesNomina.AddAsync(config, ct);
            await _context.SaveChangesAsync(ct);
        }
        return config;
    }

    #endregion

    #region Fase 2: Definición y Creación de Empleados

    private class EmpleadoDefinicion
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Identificacion { get; set; } = string.Empty;
        public string NumeroINSS { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public DateOnly FechaContratacion { get; set; }
        public DateOnly? FechaBaja { get; set; }
        public decimal SalarioBase { get; set; }
        public string NombreDepartamento { get; set; } = string.Empty;
        public string NombreCargo { get; set; } = string.Empty;
        public string NombreHorario { get; set; } = string.Empty;
        public string? CodigoJefe { get; set; }
        public EstadoEmpleado Estado { get; set; } = EstadoEmpleado.Activo;

        // Historial de cambios organizacionales
        public List<(string Depto, string Cargo, DateOnly Inicio, DateOnly? Fin)> HistorialDepartamentos { get; set; } = new();

        // Referencia a la entidad en BD
        public Empleado? Entidad { get; set; }
    }

    private List<EmpleadoDefinicion> DefinirEmpleados(
        Dictionary<string, Departamento> deptos,
        Dictionary<string, Cargo> cargos,
        Dictionary<string, HorarioLaboral> horarios)
    {
        return new List<EmpleadoDefinicion>
        {
            // === 2014: Fundadores y Jefaturas ===
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2014-01", Nombres = "Roberto Carlos", Apellidos = "Mendoza Vivas",
                Identificacion = "001-150375-0012A", NumeroINSS = "1045231-1", Correo = "roberto.mendoza@asistenciapyme.com",
                Telefono = "+505 8899-1001", Direccion = "Colonia Los Robles, Managua",
                FechaContratacion = new DateOnly(2014, 1, 2), SalarioBase = 65000.00m,
                NombreDepartamento = "Dirección Ejecutiva", NombreCargo = "Director General", NombreHorario = "Horario Administrativo",
                CodigoJefe = null, Estado = EstadoEmpleado.Activo
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2014-02", Nombres = "Carlos Alberto", Apellidos = "Morales Guido",
                Identificacion = "001-220480-0023B", NumeroINSS = "1045232-2", Correo = "carlos.morales@asistenciapyme.com",
                Telefono = "+505 8899-1002", Direccion = "Villa Fontana, Managua",
                FechaContratacion = new DateOnly(2014, 1, 15), SalarioBase = 48000.00m,
                NombreDepartamento = "Tecnología de la Información", NombreCargo = "Jefe de TI", NombreHorario = "Horario Administrativo",
                CodigoJefe = "EMP-2014-01", Estado = EstadoEmpleado.Activo
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2014-03", Nombres = "Elena María", Apellidos = "Rostrán Brenes",
                Identificacion = "001-100882-0034C", NumeroINSS = "1045233-3", Correo = "elena.rostran@asistenciapyme.com",
                Telefono = "+505 8899-1003", Direccion = "Bello Horizonte, Managua",
                FechaContratacion = new DateOnly(2014, 1, 15), SalarioBase = 42000.00m,
                NombreDepartamento = "Recursos Humanos", NombreCargo = "Jefe de Recursos Humanos", NombreHorario = "Horario Administrativo",
                CodigoJefe = "EMP-2014-01", Estado = EstadoEmpleado.Activo
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2014-04", Nombres = "Sandra Patricia", Apellidos = "Silva Gutiérrez",
                Identificacion = "001-050681-0045D", NumeroINSS = "1045234-4", Correo = "sandra.silva@asistenciapyme.com",
                Telefono = "+505 8899-1004", Direccion = "Las Colinas, Managua",
                FechaContratacion = new DateOnly(2014, 2, 1), SalarioBase = 45000.00m,
                NombreDepartamento = "Finanzas y Contabilidad", NombreCargo = "Jefe de Finanzas", NombreHorario = "Horario Administrativo",
                CodigoJefe = "EMP-2014-01", Estado = EstadoEmpleado.Activo
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2014-05", Nombres = "Mauricio José", Apellidos = "López Calero",
                Identificacion = "001-180979-0056E", NumeroINSS = "1045235-5", Correo = "mauricio.lopez@asistenciapyme.com",
                Telefono = "+505 8899-1005", Direccion = "Reparto San Juan, Managua",
                FechaContratacion = new DateOnly(2014, 2, 1), SalarioBase = 44000.00m,
                NombreDepartamento = "Operaciones y Ventas", NombreCargo = "Jefe de Operaciones", NombreHorario = "Horario Operativo",
                CodigoJefe = "EMP-2014-01", Estado = EstadoEmpleado.Activo
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2014-06", Nombres = "Francisco Javier", Apellidos = "Arteaga Ruiz",
                Identificacion = "001-121184-0067F", NumeroINSS = "1045236-6", Correo = "francisco.arteaga@asistenciapyme.com",
                Telefono = "+505 8899-1006", Direccion = "Santa Ana, Managua",
                FechaContratacion = new DateOnly(2014, 3, 1), SalarioBase = 35000.00m,
                NombreDepartamento = "Tecnología de la Información", NombreCargo = "Desarrollador Senior", NombreHorario = "Horario Administrativo",
                CodigoJefe = "EMP-2014-02", Estado = EstadoEmpleado.Activo
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2014-07", Nombres = "Gloria Amparo", Apellidos = "Méndez Solís",
                Identificacion = "001-250283-0078G", NumeroINSS = "1045237-7", Correo = "gloria.mendez@asistenciapyme.com",
                Telefono = "+505 8899-1007", Direccion = "Ciudad Jardín, Managua",
                FechaContratacion = new DateOnly(2014, 3, 15), SalarioBase = 32000.00m,
                NombreDepartamento = "Finanzas y Contabilidad", NombreCargo = "Contador General", NombreHorario = "Horario Administrativo",
                CodigoJefe = "EMP-2014-04", Estado = EstadoEmpleado.Activo
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2014-08", Nombres = "Denis Noel", Apellidos = "Castillo Mayorga",
                Identificacion = "001-090485-0089H", NumeroINSS = "1045238-8", Correo = "denis.castillo@asistenciapyme.com",
                Telefono = "+505 8899-1008", Direccion = "Linda Vista, Managua",
                FechaContratacion = new DateOnly(2014, 4, 1), FechaBaja = new DateOnly(2021, 8, 31), SalarioBase = 28000.00m,
                NombreDepartamento = "Operaciones y Ventas", NombreCargo = "Ejecutivo de Ventas", NombreHorario = "Horario Operativo",
                CodigoJefe = "EMP-2014-05", Estado = EstadoEmpleado.Inactivo // Inactivo desde 2021
            },

            // === 2015 - 2018: Fase de Crecimiento ===
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2015-01", Nombres = "Karla Vanessa", Apellidos = "Duarte Rivas",
                Identificacion = "001-140788-0090J", NumeroINSS = "1045239-9", Correo = "karla.duarte@asistenciapyme.com",
                Telefono = "+505 8899-1009", Direccion = "Monseñor Lezcano, Managua",
                FechaContratacion = new DateOnly(2015, 5, 10), SalarioBase = 26000.00m,
                NombreDepartamento = "Recursos Humanos", NombreCargo = "Especialista de Reclutamiento", NombreHorario = "Horario Administrativo",
                CodigoJefe = "EMP-2014-03", Estado = EstadoEmpleado.Activo
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2015-02", Nombres = "Ricardo Antonio", Apellidos = "Gómez Centeno",
                Identificacion = "001-300186-0101K", NumeroINSS = "1045240-0", Correo = "ricardo.gomez@asistenciapyme.com",
                Telefono = "+505 8899-1010", Direccion = "Altagracia, Managua",
                FechaContratacion = new DateOnly(2015, 8, 1), SalarioBase = 32000.00m,
                NombreDepartamento = "Tecnología de la Información", NombreCargo = "Desarrollador Full Stack", NombreHorario = "Horario Administrativo",
                CodigoJefe = "EMP-2014-02", Estado = EstadoEmpleado.Activo
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2016-01", Nombres = "Martha Lorena", Apellidos = "Espinoza Pérez",
                Identificacion = "001-190987-0112L", NumeroINSS = "1045241-1", Correo = "martha.espinoza@asistenciapyme.com",
                Telefono = "+505 8899-1011", Direccion = "Colonia Centroamérica, Managua",
                FechaContratacion = new DateOnly(2016, 2, 15), SalarioBase = 22000.00m,
                NombreDepartamento = "Finanzas y Contabilidad", NombreCargo = "Asistente Contable", NombreHorario = "Horario Administrativo",
                CodigoJefe = "EMP-2014-07", Estado = EstadoEmpleado.Activo
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2016-02", Nombres = "Manuel Salvador", Apellidos = "Rivera Borge",
                Identificacion = "001-081289-0123M", NumeroINSS = "1045242-2", Correo = "manuel.rivera@asistenciapyme.com",
                Telefono = "+505 8899-1012", Direccion = "Villa Miguel Gutiérrez, Managua",
                FechaContratacion = new DateOnly(2016, 9, 1), SalarioBase = 30000.00m,
                NombreDepartamento = "Tecnología de la Información", NombreCargo = "Desarrollador Full Stack", NombreHorario = "Horario Administrativo",
                CodigoJefe = "EMP-2014-02", Estado = EstadoEmpleado.Activo
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2017-01", Nombres = "Jessica María", Apellidos = "Jarquín Pavón",
                Identificacion = "001-040391-0134N", NumeroINSS = "1045243-3", Correo = "jessica.jarquin@asistenciapyme.com",
                Telefono = "+505 8899-1013", Direccion = "Don Bosco, Managua",
                FechaContratacion = new DateOnly(2017, 3, 1), SalarioBase = 33000.00m,
                NombreDepartamento = "Operaciones y Ventas", NombreCargo = "Supervisor de Ventas", NombreHorario = "Horario Operativo",
                CodigoJefe = "EMP-2014-05", Estado = EstadoEmpleado.Activo,
                HistorialDepartamentos = new()
                {
                    ("Operaciones y Ventas", "Ejecutivo de Ventas", new DateOnly(2017, 3, 1), new DateOnly(2021, 12, 31)),
                    ("Operaciones y Ventas", "Supervisor de Ventas", new DateOnly(2022, 1, 1), null)
                }
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2017-02", Nombres = "Christian Daniel", Apellidos = "Obando Valle",
                Identificacion = "001-160690-0145P", NumeroINSS = "1045244-4", Correo = "christian.obando@asistenciapyme.com",
                Telefono = "+505 8899-1014", Direccion = "Rubenia, Managua",
                FechaContratacion = new DateOnly(2017, 7, 15), FechaBaja = new DateOnly(2023, 4, 30), SalarioBase = 24000.00m,
                NombreDepartamento = "Tecnología de la Información", NombreCargo = "Desarrollador Frontend", NombreHorario = "Horario Administrativo",
                CodigoJefe = "EMP-2014-02", Estado = EstadoEmpleado.Inactivo // Inactivo desde 2023
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2018-01", Nombres = "Mayra Auxiliadora", Apellidos = "Blandón Vega",
                Identificacion = "001-210587-0156Q", NumeroINSS = "1045245-5", Correo = "mayra.blandon@asistenciapyme.com",
                Telefono = "+505 8899-1015", Direccion = "San Judas, Managua",
                FechaContratacion = new DateOnly(2018, 1, 10), SalarioBase = 25000.00m,
                NombreDepartamento = "Recursos Humanos", NombreCargo = "Analista de Nómina", NombreHorario = "Horario Administrativo",
                CodigoJefe = "EMP-2014-03", Estado = EstadoEmpleado.Activo
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2018-02", Nombres = "Julio César", Apellidos = "Téllez Luna",
                Identificacion = "001-110886-0167R", NumeroINSS = "1045246-6", Correo = "julio.tellez@asistenciapyme.com",
                Telefono = "+505 8899-1016", Direccion = "Waspán Sur, Managua",
                FechaContratacion = new DateOnly(2018, 6, 1), SalarioBase = 20000.00m,
                NombreDepartamento = "Operaciones y Ventas", NombreCargo = "Técnico de Operaciones", NombreHorario = "Horario Operativo",
                CodigoJefe = "EMP-2014-05", Estado = EstadoEmpleado.Activo
            },

            // === 2019 - 2022: Fase de Expansión ===
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2019-01", Nombres = "Sofía Alejandra", Apellidos = "Bravo Reyes",
                Identificacion = "001-030992-0178S", NumeroINSS = "1045247-7", Correo = "sofia.bravo@asistenciapyme.com",
                Telefono = "+505 8899-1017", Direccion = "Villa Reconciliación, Managua",
                FechaContratacion = new DateOnly(2019, 2, 1), SalarioBase = 27000.00m,
                NombreDepartamento = "Tecnología de la Información", NombreCargo = "Desarrollador Frontend", NombreHorario = "Horario Administrativo",
                CodigoJefe = "EMP-2014-02", Estado = EstadoEmpleado.Activo
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2019-02", Nombres = "Guillermo José", Apellidos = "Lacayo Pineda",
                Identificacion = "001-171085-0189T", NumeroINSS = "1045248-8", Correo = "guillermo.lacayo@asistenciapyme.com",
                Telefono = "+505 8899-1018", Direccion = "Las Brisas, Managua",
                FechaContratacion = new DateOnly(2019, 8, 15), SalarioBase = 31000.00m,
                NombreDepartamento = "Finanzas y Contabilidad", NombreCargo = "Contador General", NombreHorario = "Horario Administrativo",
                CodigoJefe = "EMP-2014-04", Estado = EstadoEmpleado.Activo
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2020-01", Nombres = "Andrea Marcela", Apellidos = "Fonseca Campos",
                Identificacion = "001-280493-0190U", NumeroINSS = "1045249-9", Correo = "andrea.fonseca@asistenciapyme.com",
                Telefono = "+505 8899-1019", Direccion = "Las Mercedes, Managua",
                FechaContratacion = new DateOnly(2020, 1, 15), SalarioBase = 26000.00m,
                NombreDepartamento = "Operaciones y Ventas", NombreCargo = "Ejecutivo de Cuentas Clave", NombreHorario = "Horario Operativo",
                CodigoJefe = "EMP-2017-01", Estado = EstadoEmpleado.Activo
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2020-02", Nombres = "Norman Enrique", Apellidos = "Aguirre Somarriba",
                Identificacion = "001-070689-0201V", NumeroINSS = "1045250-0", Correo = "norman.aguirre@asistenciapyme.com",
                Telefono = "+505 8899-1020", Direccion = "Primero de Mayo, Managua",
                FechaContratacion = new DateOnly(2020, 7, 1), FechaBaja = new DateOnly(2024, 2, 28), SalarioBase = 23000.00m,
                NombreDepartamento = "Recursos Humanos", NombreCargo = "Especialista de Reclutamiento", NombreHorario = "Horario Administrativo",
                CodigoJefe = "EMP-2014-03", Estado = EstadoEmpleado.Inactivo // Inactivo desde 2024
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2021-01", Nombres = "Gabriel Ignacio", Apellidos = "Toruño Zelaya",
                Identificacion = "001-140294-0212W", NumeroINSS = "1045251-1", Correo = "gabriel.toruno@asistenciapyme.com",
                Telefono = "+505 8899-1021", Direccion = "Colonia Managua, Managua",
                FechaContratacion = new DateOnly(2021, 3, 15), SalarioBase = 28000.00m,
                NombreDepartamento = "Tecnología de la Información", NombreCargo = "Desarrollador Backend", NombreHorario = "Horario Administrativo",
                CodigoJefe = "EMP-2014-02", Estado = EstadoEmpleado.Activo
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2021-02", Nombres = "Lucía Del Carmen", Apellidos = "Urbina Mairena",
                Identificacion = "001-201195-0223X", NumeroINSS = "1045252-2", Correo = "lucia.urbina@asistenciapyme.com",
                Telefono = "+505 8899-1022", Direccion = "Barrio Cuba, Managua",
                FechaContratacion = new DateOnly(2021, 10, 1), SalarioBase = 24000.00m,
                NombreDepartamento = "Recursos Humanos", NombreCargo = "Especialista en Bienestar", NombreHorario = "Horario Administrativo",
                CodigoJefe = "EMP-2014-03", Estado = EstadoEmpleado.Activo,
                HistorialDepartamentos = new()
                {
                    ("Dirección Ejecutiva", "Asistente Contable", new DateOnly(2021, 10, 1), new DateOnly(2023, 5, 31)),
                    ("Recursos Humanos", "Especialista en Bienestar", new DateOnly(2023, 6, 1), null)
                }
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2022-01", Nombres = "Kevin Alexander", Apellidos = "Talavera Mora",
                Identificacion = "001-050196-0234Y", NumeroINSS = "1045253-3", Correo = "kevin.talavera@asistenciapyme.com",
                Telefono = "+505 8899-1023", Direccion = "Villa Libertad, Managua",
                FechaContratacion = new DateOnly(2022, 4, 1), SalarioBase = 21000.00m,
                NombreDepartamento = "Operaciones y Ventas", NombreCargo = "Ejecutivo de Ventas", NombreHorario = "Horario Operativo",
                CodigoJefe = "EMP-2017-01", Estado = EstadoEmpleado.Activo
            },

            // === 2023 - 2026: Fase Reciente ===
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2023-01", Nombres = "Valeria Estefanía", Apellidos = "Moncada Cano",
                Identificacion = "001-180897-0245Z", NumeroINSS = "1045254-4", Correo = "valeria.moncada@asistenciapyme.com",
                Telefono = "+505 8899-1024", Direccion = "Carretera Sur, Managua",
                FechaContratacion = new DateOnly(2023, 2, 15), SalarioBase = 26000.00m,
                NombreDepartamento = "Tecnología de la Información", NombreCargo = "Diseñador UI/UX", NombreHorario = "Horario Administrativo",
                CodigoJefe = "EMP-2014-02", Estado = EstadoEmpleado.Activo
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2023-02", Nombres = "Bryan Josué", Apellidos = "Mercado Rugama",
                Identificacion = "001-091296-0256A", NumeroINSS = "1045255-5", Correo = "bryan.mercado@asistenciapyme.com",
                Telefono = "+505 8899-1025", Direccion = "Carretera a Masaya, Managua",
                FechaContratacion = new DateOnly(2023, 9, 1), SalarioBase = 27000.00m,
                NombreDepartamento = "Finanzas y Contabilidad", NombreCargo = "Analista Financiero", NombreHorario = "Horario Administrativo",
                CodigoJefe = "EMP-2014-04", Estado = EstadoEmpleado.Activo
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2024-01", Nombres = "Patricia Nohemy", Apellidos = "Cisneros Palma",
                Identificacion = "001-230398-0267B", NumeroINSS = "1045256-6", Correo = "patricia.cisneros@asistenciapyme.com",
                Telefono = "+505 8899-1026", Direccion = "Tipitapa, Managua",
                FechaContratacion = new DateOnly(2024, 1, 15), SalarioBase = 23000.00m,
                NombreDepartamento = "Recursos Humanos", NombreCargo = "Especialista en Bienestar", NombreHorario = "Horario Administrativo",
                CodigoJefe = "EMP-2014-03", Estado = EstadoEmpleado.Activo
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2024-02", Nombres = "Álvaro José", Apellidos = "Granados Rivas",
                Identificacion = "001-110597-0278C", NumeroINSS = "1045257-7", Correo = "alvaro.granados@asistenciapyme.com",
                Telefono = "+505 8899-1027", Direccion = "San Rafael del Sur, Managua",
                FechaContratacion = new DateOnly(2024, 8, 1), SalarioBase = 25000.00m,
                NombreDepartamento = "Tecnología de la Información", NombreCargo = "QA Tester", NombreHorario = "Horario Administrativo",
                CodigoJefe = "EMP-2014-02", Estado = EstadoEmpleado.Activo
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2025-01", Nombres = "Camila Valentina", Apellidos = "Cordero Lezama",
                Identificacion = "001-300699-0289D", NumeroINSS = "1045258-8", Correo = "camila.cordero@asistenciapyme.com",
                Telefono = "+505 8899-1028", Direccion = "Ciudad Sandino, Managua",
                FechaContratacion = new DateOnly(2025, 3, 1), SalarioBase = 20000.00m,
                NombreDepartamento = "Operaciones y Ventas", NombreCargo = "Ejecutivo de Ventas", NombreHorario = "Horario Operativo",
                CodigoJefe = "EMP-2017-01", Estado = EstadoEmpleado.Activo
            },
            new EmpleadoDefinicion
            {
                Codigo = "EMP-2026-01", Nombres = "David Eduardo", Apellidos = "Arana Zelaya",
                Identificacion = "001-170402-0290E", NumeroINSS = "1045259-9", Correo = "david.arana@asistenciapyme.com",
                Telefono = "+505 8899-1029", Direccion = "Mateare, Managua",
                FechaContratacion = new DateOnly(2026, 1, 15), SalarioBase = 16000.00m,
                NombreDepartamento = "Tecnología de la Información", NombreCargo = "Pasante de TI", NombreHorario = "Horario Administrativo",
                CodigoJefe = "EMP-2014-02", Estado = EstadoEmpleado.Activo
            }
        };
    }

    private async Task<List<Empleado>> SembrarEmpleadosAsync(List<EmpleadoDefinicion> defs, CancellationToken ct)
    {
        var pinHashDefault = _pinHasher.CrearHash("1234");
        var entidades = new Dictionary<string, Empleado>();

        // Paso 1: Crear entidades base sin jefes directos para permitir resolución de FK
        foreach (var def in defs)
        {
            var depto = await _context.Departamentos.FirstAsync(d => d.Nombre == def.NombreDepartamento, ct);
            var cargo = await _context.Cargos.FirstAsync(c => c.Nombre == def.NombreCargo, ct);
            var horario = await _context.HorariosLaborales.FirstAsync(h => h.Nombre == def.NombreHorario, ct);

            var emp = new Empleado
            {
                CodigoEmpleado = def.Codigo,
                Nombres = def.Nombres,
                Apellidos = def.Apellidos,
                Identificacion = def.Identificacion,
                NumeroINSS = def.NumeroINSS,
                Correo = def.Correo,
                Telefono = def.Telefono,
                Direccion = def.Direccion,
                PinHash = pinHashDefault,
                FechaContratacion = def.FechaContratacion,
                SalarioBase = def.SalarioBase,
                Estado = def.Estado,
                IdDepartamento = depto.IdDepartamento,
                IdCargo = cargo.IdCargo,
                IdHorarioLaboral = horario.IdHorarioLaboral,
                FechaCreacion = def.FechaContratacion.ToDateTime(new TimeOnly(8, 0), DateTimeKind.Utc)
            };

            // Agregar historial inicial de departamento
            if (def.HistorialDepartamentos.Any())
            {
                foreach (var h in def.HistorialDepartamentos)
                {
                    var dHist = await _context.Departamentos.FirstAsync(d => d.Nombre == h.Depto, ct);
                    emp.DepartamentoHistorial.Add(new EmpleadoDepartamentoHistorial
                    {
                        IdDepartamento = dHist.IdDepartamento,
                        FechaInicio = h.Inicio.ToDateTime(new TimeOnly(0, 0), DateTimeKind.Utc),
                        FechaFin = h.Fin?.ToDateTime(new TimeOnly(23, 59), DateTimeKind.Utc)
                    });
                }
            }
            else
            {
                emp.DepartamentoHistorial.Add(new EmpleadoDepartamentoHistorial
                {
                    IdDepartamento = depto.IdDepartamento,
                    FechaInicio = def.FechaContratacion.ToDateTime(new TimeOnly(0, 0), DateTimeKind.Utc),
                    FechaFin = def.FechaBaja?.ToDateTime(new TimeOnly(23, 59), DateTimeKind.Utc)
                });
            }

            await _context.Empleados.AddAsync(emp, ct);
            entidades[def.Codigo] = emp;
            def.Entidad = emp;
        }

        await _context.SaveChangesAsync(ct);

        // Paso 2: Vincular Jerarquías de Jefes Directos
        foreach (var def in defs)
        {
            if (!string.IsNullOrEmpty(def.CodigoJefe) && entidades.TryGetValue(def.CodigoJefe, out var jefe))
            {
                def.Entidad!.IdJefeDirecto = jefe.IdEmpleado;
            }
        }

        await _context.SaveChangesAsync(ct);
        return entidades.Values.ToList();
    }

    #endregion

    #region Fase 3: Vacaciones Históricas

    private async Task<List<Vacacion>> SembrarVacacionesAsync(
        List<EmpleadoDefinicion> defs,
        int idAdmin,
        Random rng,
        CancellationToken ct)
    {
        var vacaciones = new List<Vacacion>();

        foreach (var def in defs)
        {
            var emp = def.Entidad!;
            int anioInicio = def.FechaContratacion.Year + 1; // Vacaciones a partir del 1er año
            int anioFin = def.FechaBaja?.Year ?? 2026;

            for (int anio = anioInicio; anio <= anioFin && anio <= 2026; anio++)
            {
                // Un período de vacaciones por año entre mayo y noviembre
                int mes = rng.Next(5, 11);
                int dia = rng.Next(1, 15);
                var fInicio = new DateOnly(anio, mes, dia);
                int diasVacaciones = rng.Next(5, 12);
                var fFin = fInicio.AddDays(diasVacaciones);

                if (fFin > FechaCorteHistorico || (def.FechaBaja.HasValue && fFin >= def.FechaBaja.Value))
                {
                    continue;
                }

                vacaciones.Add(new Vacacion
                {
                    IdEmpleado = emp.IdEmpleado,
                    IdAdministrador = idAdmin,
                    FechaInicio = fInicio,
                    FechaFin = fFin,
                    Motivo = "Vacaciones anuales reglamentarias",
                    Observacion = "Aprobadas por Recursos Humanos en tiempo y forma",
                    Cancelada = false,
                    FechaCreacion = fInicio.AddDays(-15).ToDateTime(new TimeOnly(9, 0), DateTimeKind.Utc)
                });
            }
        }

        await _context.Vacaciones.AddRangeAsync(vacaciones, ct);
        await _context.SaveChangesAsync(ct);
        return vacaciones;
    }

    #endregion

    #region Fase 4: Asistencias y Horas Extras Muestreadas (~2,000 registros)

    private async Task<(int TotalAsistencias, int TotalHorasExtras)> SembrarAsistenciasYHorasExtrasAsync(
        List<EmpleadoDefinicion> defs,
        List<Vacacion> vacaciones,
        Random rng,
        CancellationToken ct)
    {
        int totalAsistencias = 0;
        int totalHorasExtras = 0;

        // Diccionario de vacaciones por empleado para consulta rápida O(1)
        var vacPorEmp = vacaciones.GroupBy(v => v.IdEmpleado)
            .ToDictionary(g => g.Key, g => g.Select(v => (v.FechaInicio, v.FechaFin)).ToList());

        var loteAsistencias = new List<Asistencia>(1000);
        var loteHorasExtras = new List<HoraExtra>(200);

        // Muestreo estratificado por empleado y por mes entre 2014 y 2026
        foreach (var def in defs)
        {
            var emp = def.Entidad!;
            var inicio = def.FechaContratacion;
            var fin = def.FechaBaja ?? FechaCorteHistorico;
            if (fin > FechaCorteHistorico) fin = FechaCorteHistorico;

            var listaVac = vacPorEmp.TryGetValue(emp.IdEmpleado, out var lVac) ? lVac : new();

            for (int anio = inicio.Year; anio <= fin.Year; anio++)
            {
                int mesMin = (anio == inicio.Year) ? inicio.Month : 1;
                int mesMax = (anio == fin.Year) ? fin.Month : 12;

                // Tasa de muestreo ponderada por año para lograr ~2,000 registros distribuidos armónicamente
                double factorMuestreo = anio switch
                {
                    2014 => 1.15, // Mayor muestreo por menor cantidad de empleados fundadores
                    2015 => 0.95,
                    2016 => 0.90,
                    2017 => 0.88,
                    2018 => 0.85,
                    2019 => 0.80,
                    2020 => 0.65,
                    2021 => 0.68,
                    2022 => 0.68,
                    2023 => 0.65,
                    2024 => 0.63,
                    2025 => 0.62,
                    _ => 0.62 // 2026 hasta fecha actual
                };

                for (int mes = mesMin; mes <= mesMax; mes++)
                {
                    int diasEnMes = DateTime.DaysInMonth(anio, mes);
                    var diasValidos = new List<DateOnly>();

                    for (int dia = 1; dia <= diasEnMes; dia++)
                    {
                        var fechaDia = new DateOnly(anio, mes, dia);
                        if (fechaDia < inicio || fechaDia > fin) continue;

                        var dow = fechaDia.DayOfWeek;
                        bool esDiaLaboral = def.NombreHorario == "Horario Operativo"
                            ? (dow >= DayOfWeek.Monday && dow <= DayOfWeek.Saturday)
                            : (dow >= DayOfWeek.Monday && dow <= DayOfWeek.Friday);

                        if (!esDiaLaboral) continue;

                        bool enVacacion = listaVac.Any(v => fechaDia >= v.FechaInicio && fechaDia <= v.FechaFin);
                        if (enVacacion) continue;

                        diasValidos.Add(fechaDia);
                    }

                    if (!diasValidos.Any()) continue;

                    // Determinar cuántas jornadas muestrear en este mes para el empleado (0, 1 o 2)
                    int cantidadMuestras = (int)factorMuestreo;
                    double probFraccional = factorMuestreo - cantidadMuestras;
                    if (rng.NextDouble() < probFraccional)
                    {
                        cantidadMuestras++;
                    }

                    if (cantidadMuestras > diasValidos.Count) cantidadMuestras = diasValidos.Count;
                    if (cantidadMuestras <= 0) continue;

                    // Seleccionar días representativos distintos
                    var diasSeleccionados = diasValidos.OrderBy(_ => rng.Next()).Take(cantidadMuestras).OrderBy(d => d).ToList();

                    foreach (var fechaMuestra in diasSeleccionados)
                    {
                        int horaProg = def.NombreHorario == "Horario Operativo" ? 7 : 8;
                        int minProg = def.NombreHorario == "Horario Operativo" ? 30 : 0;
                        int horaSalProg = def.NombreHorario == "Horario Operativo" ? 16 : 17;
                        int minSalProg = minProg;

                        DateTime horaEntradaUtc;
                        DateTime horaSalidaUtc;
                        bool esTardanza = false;
                        int minutosTardanza = 0;

                        int roll = rng.Next(100);
                        if (roll < 7)
                        {
                            // 7% Tardanza menor (5 a 25 min tarde)
                            minutosTardanza = rng.Next(5, 26);
                            esTardanza = true;
                            horaEntradaUtc = fechaMuestra.ToDateTime(new TimeOnly(horaProg, minProg).Add(TimeSpan.FromMinutes(minutosTardanza)), DateTimeKind.Utc);
                            horaSalidaUtc = fechaMuestra.ToDateTime(new TimeOnly(horaSalProg, minSalProg).Add(TimeSpan.FromMinutes(rng.Next(0, 10))), DateTimeKind.Utc);
                        }
                        else if (roll < 95)
                        {
                            // 88% Puntualidad normal
                            int adelantoMin = rng.Next(0, 11);
                            horaEntradaUtc = fechaMuestra.ToDateTime(new TimeOnly(horaProg, minProg).Add(TimeSpan.FromMinutes(-adelantoMin)), DateTimeKind.Utc);
                            horaSalidaUtc = fechaMuestra.ToDateTime(new TimeOnly(horaSalProg, minSalProg).Add(TimeSpan.FromMinutes(rng.Next(0, 8))), DateTimeKind.Utc);
                        }
                        else
                        {
                            // 5% Jornada con Horas Extras aprobadas
                            int adelantoMin = rng.Next(0, 8);
                            int minExtras = rng.Next(1, 3) * 60; // 60 o 120 min
                            horaEntradaUtc = fechaMuestra.ToDateTime(new TimeOnly(horaProg, minProg).Add(TimeSpan.FromMinutes(-adelantoMin)), DateTimeKind.Utc);
                            horaSalidaUtc = fechaMuestra.ToDateTime(new TimeOnly(horaSalProg, minSalProg).Add(TimeSpan.FromMinutes(minExtras + rng.Next(0, 5))), DateTimeKind.Utc);

                            var horaExtra = new HoraExtra
                            {
                                IdEmpleado = emp.IdEmpleado,
                                Fecha = fechaMuestra.ToDateTime(new TimeOnly(horaSalProg, minSalProg), DateTimeKind.Utc),
                                MinutosDetectados = minExtras,
                                MinutosAprobados = minExtras,
                                Estado = EstadoHoraExtra.Aprobada,
                                AprobadoPor = emp.IdJefeDirecto,
                                FechaAprobacion = fechaMuestra.AddDays(1).ToDateTime(new TimeOnly(9, 0), DateTimeKind.Utc),
                                Observacion = "Horas extras para cumplimiento de entregables y cierre de proyectos"
                            };
                            loteHorasExtras.Add(horaExtra);
                        }

                        var asistencia = new Asistencia
                        {
                            IdEmpleado = emp.IdEmpleado,
                            HoraEntrada = horaEntradaUtc,
                            HoraSalida = horaSalidaUtc,
                            EsEntradaTardia = esTardanza,
                            MinutosTardanza = minutosTardanza,
                            HoraProgramadaEntrada = fechaMuestra.ToDateTime(new TimeOnly(horaProg, minProg), DateTimeKind.Utc),
                            HoraProgramadaSalida = fechaMuestra.ToDateTime(new TimeOnly(horaSalProg, minSalProg), DateTimeKind.Utc),
                            Corregida = false,
                            FechaCreacion = horaEntradaUtc
                        };
                        loteAsistencias.Add(asistencia);

                        if (loteAsistencias.Count >= 1000)
                        {
                            await _context.Asistencias.AddRangeAsync(loteAsistencias, ct);
                            await _context.SaveChangesAsync(ct);
                            totalAsistencias += loteAsistencias.Count;
                            loteAsistencias.Clear();
                        }

                        if (loteHorasExtras.Count >= 200)
                        {
                            await _context.HorasExtras.AddRangeAsync(loteHorasExtras, ct);
                            await _context.SaveChangesAsync(ct);
                            totalHorasExtras += loteHorasExtras.Count;
                            loteHorasExtras.Clear();
                        }
                    }
                }
            }
        }

        if (loteAsistencias.Any())
        {
            await _context.Asistencias.AddRangeAsync(loteAsistencias, ct);
            await _context.SaveChangesAsync(ct);
            totalAsistencias += loteAsistencias.Count;
            loteAsistencias.Clear();
        }

        if (loteHorasExtras.Any())
        {
            await _context.HorasExtras.AddRangeAsync(loteHorasExtras, ct);
            await _context.SaveChangesAsync(ct);
            totalHorasExtras += loteHorasExtras.Count;
            loteHorasExtras.Clear();
        }

        return (totalAsistencias, totalHorasExtras);
    }

    #endregion

    #region Fase 5: Planillas Departamentales Históricas

    private async Task<int> SembrarPlanillasDepartamentalesAsync(
        Dictionary<string, Departamento> deptos,
        List<EmpleadoDefinicion> defs,
        int idAdmin,
        decimal multiplicadorHoraExtra,
        CancellationToken ct)
    {
        int totalPlanillas = 0;

        var todasHorasExtras = await _context.HorasExtras
            .AsNoTracking()
            .Where(h => h.Estado == EstadoHoraExtra.Aprobada)
            .ToListAsync(ct);

        var todasAsistencias = await _context.Asistencias
            .AsNoTracking()
            .Where(a => a.EsEntradaTardia)
            .ToListAsync(ct);

        for (int anio = 2014; anio <= 2026; anio++)
        {
            int mesMax = (anio == 2026) ? 7 : 12; // Hasta julio 2026 inclusive
            for (int mes = 1; mes <= mesMax; mes++)
            {
                var inicioMes = new DateOnly(anio, mes, 1);
                var finMes = new DateOnly(anio, mes, DateTime.DaysInMonth(anio, mes));

                var inicioMesUtc = inicioMes.ToDateTime(new TimeOnly(0, 0), DateTimeKind.Utc);
                var finMesUtc = finMes.ToDateTime(new TimeOnly(23, 59), DateTimeKind.Utc);

                foreach (var depto in deptos.Values)
                {
                    var empleadosEnDepto = defs.Where(d =>
                    {
                        if (d.FechaContratacion > finMes) return false;
                        if (d.FechaBaja.HasValue && d.FechaBaja.Value < inicioMes) return false;

                        if (d.HistorialDepartamentos.Any())
                        {
                            var hActivo = d.HistorialDepartamentos.FirstOrDefault(h =>
                                h.Inicio <= finMes && (!h.Fin.HasValue || h.Fin.Value >= inicioMes));
                            return hActivo.Depto == depto.Nombre;
                        }

                        return d.NombreDepartamento == depto.Nombre;
                    }).ToList();

                    if (!empleadosEnDepto.Any()) continue;

                    var planilla = new Planilla
                    {
                        IdDepartamento = depto.IdDepartamento,
                        IdAdministrador = idAdmin,
                        FechaInicioPeriodo = inicioMes,
                        FechaFinPeriodo = finMes,
                        Estado = EstadoPlanilla.Pagada,
                        FechaGeneracion = finMesUtc,
                        FechaCierre = finMesUtc.AddDays(1),
                        CerradoPor = idAdmin,
                        FechaCreacion = finMesUtc
                    };

                    await _context.Planillas.AddAsync(planilla, ct);

                    var detalles = new List<DetallePlanilla>();
                    foreach (var empDef in empleadosEnDepto)
                    {
                        var emp = empDef.Entidad!;

                        var heMes = todasHorasExtras.Where(h => h.IdEmpleado == emp.IdEmpleado && h.Fecha >= inicioMesUtc && h.Fecha <= finMesUtc).ToList();
                        int minExtrasAprobados = heMes.Sum(h => h.MinutosAprobados);
                        int minExtrasDetectados = heMes.Sum(h => h.MinutosDetectados);
                        decimal montoHorasExtras = _calculadorHorasExtras.CalcularMontoHorasExtras(emp.SalarioBase, minExtrasAprobados, multiplicadorHoraExtra);

                        var tardanzasMes = todasAsistencias.Where(a => a.IdEmpleado == emp.IdEmpleado && a.HoraEntrada >= inicioMesUtc && a.HoraEntrada <= finMesUtc).ToList();
                        int cantTardanzas = tardanzasMes.Count;
                        int minTardanza = tardanzasMes.Sum(a => a.MinutosTardanza);

                        decimal totalIngresos = montoHorasExtras;
                        decimal totalDeducciones = 0m;
                        decimal salarioNeto = Math.Round(emp.SalarioBase + totalIngresos - totalDeducciones, 2, MidpointRounding.AwayFromZero);

                        detalles.Add(new DetallePlanilla
                        {
                            IdEmpleado = emp.IdEmpleado,
                            CodigoEmpleado = emp.CodigoEmpleado,
                            NombreEmpleado = $"{emp.Nombres} {emp.Apellidos}",
                            NumeroINSS = emp.NumeroINSS,
                            Cargo = empDef.NombreCargo,
                            Departamento = depto.Nombre,
                            SalarioBase = emp.SalarioBase,
                            DiasLaborados = DateTime.DaysInMonth(anio, mes) >= 30 ? 30 : DateTime.DaysInMonth(anio, mes),
                            MinutosLaborados = 0,
                            CantidadTardanzas = cantTardanzas,
                            MinutosTardanza = minTardanza,
                            DescuentoTardanza = 0m,
                            MinutosExtrasDetectados = minExtrasDetectados,
                            MinutosExtrasAprobados = minExtrasAprobados,
                            MontoHorasExtras = montoHorasExtras,
                            TotalIngresos = totalIngresos,
                            TotalDeducciones = totalDeducciones,
                            SalarioNeto = salarioNeto,
                            Planilla = planilla
                        });
                    }

                    await _context.DetallesPlanilla.AddRangeAsync(detalles, ct);

                    planilla.CantidadEmpleados = detalles.Count;
                    planilla.TotalSalarioBase = detalles.Sum(d => d.SalarioBase);
                    planilla.TotalHorasExtras = detalles.Sum(d => d.MontoHorasExtras);
                    planilla.TotalIngresos = detalles.Sum(d => d.TotalIngresos);
                    planilla.TotalDeducciones = detalles.Sum(d => d.TotalDeducciones);
                    planilla.TotalNeto = detalles.Sum(d => d.SalarioNeto);

                    totalPlanillas++;
                }
            }

            await _context.SaveChangesAsync(ct);
        }

        return totalPlanillas;
    }

    #endregion

    #region Fase 6: Evaluaciones de Desempeño 360° con Snapshot Histórico

    private async Task<(int TotalPeriodos, int TotalEvaluaciones, int TotalDetalles)> SembrarEvaluaciones360Async(
        List<EmpleadoDefinicion> defs,
        List<CategoriaEvaluacion> categorias,
        Random rng,
        CancellationToken ct)
    {
        int totalPeriodos = 0;
        int totalEvaluaciones = 0;
        int totalDetalles = 0;

        var todosCriterios = categorias.SelectMany(c => c.Criterios).ToList();

        // 26 Períodos semestrales: 2014-I hasta 2026-II
        for (int anio = 2014; anio <= 2026; anio++)
        {
            for (int sem = 1; sem <= 2; sem++)
            {
                string nombrePeriodo = $"{anio}-{(sem == 1 ? "I" : "II")}";
                var fInicio = new DateOnly(anio, sem == 1 ? 1 : 7, 1);
                var fFin = new DateOnly(anio, sem == 1 ? 6 : 12, sem == 1 ? 30 : 31);

                // 2026-II es el período activo actual en curso
                bool esPeriodoVigente = (anio == 2026 && sem == 2);
                var estadoPeriodo = esPeriodoVigente ? EstadoPeriodoEvaluacion.Activo : EstadoPeriodoEvaluacion.Finalizado;

                var periodo = new PeriodoEvaluacion
                {
                    Nombre = nombrePeriodo,
                    FechaInicio = fInicio,
                    FechaFin = fFin,
                    Estado = estadoPeriodo,
                    FechaCreacion = fInicio.ToDateTime(new TimeOnly(0, 0), DateTimeKind.Utc)
                };

                await _context.PeriodosEvaluacion.AddAsync(periodo, ct);
                await _context.SaveChangesAsync(ct);
                totalPeriodos++;

                // Empleados que laboraban durante este semestre
                var empleadosEnPeriodo = defs.Where(d =>
                    d.FechaContratacion <= fFin &&
                    (!d.FechaBaja.HasValue || d.FechaBaja.Value >= fInicio)
                ).ToList();

                var evaluacionesPeriodo = new List<EvaluacionDesempeno>();
                var detallesPeriodo = new List<DetalleEvaluacionDesempeno>();

                if (esPeriodoVigente)
                {
                    // En 2026-II (período activo): Generar evaluaciones Pendientes y En Proceso (0 completadas)
                    int contador = 0;
                    foreach (var empDef in empleadosEnPeriodo)
                    {
                        var emp = empDef.Entidad!;
                        contador++;

                        // Alternar estados entre En Proceso y Pendiente para demostración
                        var estadoEvalAuto = (contador % 2 == 0) ? EstadoEvaluacion.EnProceso : EstadoEvaluacion.Pendiente;

                        var evalAuto = new EvaluacionDesempeno
                        {
                            IdPeriodoEvaluacion = periodo.IdPeriodoEvaluacion,
                            IdEmpleadoEvaluado = emp.IdEmpleado,
                            IdEvaluador = emp.IdEmpleado,
                            TipoEvaluador = TipoEvaluador.Autoevaluacion,
                            Estado = estadoEvalAuto,
                            PuntajeFinal = null, // Período aún en curso
                            ObservacionesGenerales = estadoEvalAuto == EstadoEvaluacion.EnProceso ? "Autoevaluación en progreso de llenado" : null,
                            FechaAsignacion = fInicio.ToDateTime(new TimeOnly(8, 0), DateTimeKind.Utc),
                            FechaCompletada = null
                        };
                        evaluacionesPeriodo.Add(evalAuto);

                        // Si el empleado tiene jefe directo
                        if (emp.IdJefeDirecto.HasValue)
                        {
                            var estadoEvalJefe = (contador % 3 == 0) ? EstadoEvaluacion.EnProceso : EstadoEvaluacion.Pendiente;
                            var evalJefe = new EvaluacionDesempeno
                            {
                                IdPeriodoEvaluacion = periodo.IdPeriodoEvaluacion,
                                IdEmpleadoEvaluado = emp.IdEmpleado,
                                IdEvaluador = emp.IdJefeDirecto.Value,
                                TipoEvaluador = TipoEvaluador.JefeDirecto,
                                Estado = estadoEvalJefe,
                                PuntajeFinal = null,
                                ObservacionesGenerales = estadoEvalJefe == EstadoEvaluacion.EnProceso ? "Evaluación de jefatura en proceso de revisión" : null,
                                FechaAsignacion = fInicio.ToDateTime(new TimeOnly(8, 0), DateTimeKind.Utc),
                                FechaCompletada = null
                            };
                            evaluacionesPeriodo.Add(evalJefe);
                        }

                        // Subordinados
                        var subordinadosActivos = empleadosEnPeriodo.Where(s => s.Entidad!.IdJefeDirecto == emp.IdEmpleado).ToList();
                        foreach (var sub in subordinadosActivos)
                        {
                            var evalSub = new EvaluacionDesempeno
                            {
                                IdPeriodoEvaluacion = periodo.IdPeriodoEvaluacion,
                                IdEmpleadoEvaluado = emp.IdEmpleado,
                                IdEvaluador = sub.Entidad!.IdEmpleado,
                                TipoEvaluador = TipoEvaluador.Subordinado,
                                Estado = EstadoEvaluacion.Pendiente,
                                PuntajeFinal = null,
                                FechaAsignacion = fInicio.ToDateTime(new TimeOnly(8, 0), DateTimeKind.Utc),
                                FechaCompletada = null
                            };
                            evaluacionesPeriodo.Add(evalSub);
                        }
                    }

                    await _context.EvaluacionesDesempeno.AddRangeAsync(evaluacionesPeriodo, ct);
                    await _context.SaveChangesAsync(ct);
                    totalEvaluaciones += evaluacionesPeriodo.Count;
                    continue;
                }

                // Períodos cerrados (2014-I hasta 2026-I): Evaluaciones completadas con puntuación y snapshot
                foreach (var empDef in empleadosEnPeriodo)
                {
                    var emp = empDef.Entidad!;
                    double sesgoEmpleado = 3.6 + (rng.NextDouble() * 1.0);

                    // 1. Autoevaluación
                    var (evalAuto, detAuto) = CrearEvaluacionCompleta(
                        periodo.IdPeriodoEvaluacion,
                        emp.IdEmpleado,
                        emp.IdEmpleado,
                        TipoEvaluador.Autoevaluacion,
                        fFin,
                        categorias,
                        todosCriterios,
                        sesgoEmpleado + 0.15,
                        rng);

                    evaluacionesPeriodo.Add(evalAuto);
                    detallesPeriodo.AddRange(detAuto);

                    // 2. Jefe Directo
                    if (emp.IdJefeDirecto.HasValue)
                    {
                        var (evalJefe, detJefe) = CrearEvaluacionCompleta(
                            periodo.IdPeriodoEvaluacion,
                            emp.IdEmpleado,
                            emp.IdJefeDirecto.Value,
                            TipoEvaluador.JefeDirecto,
                            fFin,
                            categorias,
                            todosCriterios,
                            sesgoEmpleado - 0.05,
                            rng);

                        evaluacionesPeriodo.Add(evalJefe);
                        detallesPeriodo.AddRange(detJefe);
                    }

                    // 3. Subordinados
                    var subordinadosActivos = empleadosEnPeriodo.Where(s => s.Entidad!.IdJefeDirecto == emp.IdEmpleado).ToList();
                    foreach (var sub in subordinadosActivos)
                    {
                        var (evalSub, detSub) = CrearEvaluacionCompleta(
                            periodo.IdPeriodoEvaluacion,
                            emp.IdEmpleado,
                            sub.Entidad!.IdEmpleado,
                            TipoEvaluador.Subordinado,
                            fFin,
                            categorias,
                            todosCriterios,
                            sesgoEmpleado + (rng.NextDouble() * 0.3 - 0.15),
                            rng);

                        evaluacionesPeriodo.Add(evalSub);
                        detallesPeriodo.AddRange(detSub);
                    }
                }

                await _context.EvaluacionesDesempeno.AddRangeAsync(evaluacionesPeriodo, ct);
                await _context.SaveChangesAsync(ct);

                foreach (var eval in evaluacionesPeriodo)
                {
                    var detsDeEval = detallesPeriodo.Where(d => d.Evaluacion == eval).ToList();
                    foreach (var d in detsDeEval)
                    {
                        d.IdEvaluacionDesempeno = eval.IdEvaluacionDesempeno;
                    }
                }

                await _context.DetallesEvaluacionDesempeno.AddRangeAsync(detallesPeriodo, ct);
                await _context.SaveChangesAsync(ct);

                totalEvaluaciones += evaluacionesPeriodo.Count;
                totalDetalles += detallesPeriodo.Count;
            }
        }

        return (totalPeriodos, totalEvaluaciones, totalDetalles);
    }

    private (EvaluacionDesempeno Eval, List<DetalleEvaluacionDesempeno> Detalles) CrearEvaluacionCompleta(
        int idPeriodo,
        int idEvaluado,
        int idEvaluador,
        TipoEvaluador tipo,
        DateOnly fechaCierreSemestre,
        List<CategoriaEvaluacion> categorias,
        List<CriterioEvaluacion> criterios,
        double sesgo,
        Random rng)
    {
        var eval = new EvaluacionDesempeno
        {
            IdPeriodoEvaluacion = idPeriodo,
            IdEmpleadoEvaluado = idEvaluado,
            IdEvaluador = idEvaluador,
            TipoEvaluador = tipo,
            Estado = EstadoEvaluacion.Completada,
            ObservacionesGenerales = tipo switch
            {
                TipoEvaluador.Autoevaluacion => "He cumplido con mis responsabilidades y metas con compromiso y proactividad.",
                TipoEvaluador.JefeDirecto => "Demuestra excelente desempeño, cumplimiento y alineación con los objetivos.",
                _ => "Buen liderazgo, comunicación abierta y apoyo continuo en el desarrollo del equipo."
            },
            FechaAsignacion = fechaCierreSemestre.AddDays(-15).ToDateTime(new TimeOnly(8, 0), DateTimeKind.Utc),
            FechaCompletada = fechaCierreSemestre.ToDateTime(new TimeOnly(17, 0), DateTimeKind.Utc)
        };

        var detalles = new List<DetalleEvaluacionDesempeno>();
        decimal puntajeFinal = 0m;

        foreach (var cat in categorias)
        {
            var critsDeCat = criterios.Where(c => c.IdCategoriaEvaluacion == cat.IdCategoriaEvaluacion).ToList();
            if (!critsDeCat.Any()) continue;

            decimal sumaNotas = 0m;
            foreach (var crit in critsDeCat)
            {
                // Generar nota entre 1 y 5 centrada en el sesgo
                int nota = (int)Math.Round(sesgo + (rng.NextDouble() * 0.8 - 0.4));
                if (nota < 2) nota = 2;
                if (nota > 5) nota = 5;

                sumaNotas += nota;

                detalles.Add(new DetalleEvaluacionDesempeno
                {
                    Evaluacion = eval,
                    IdCriterioEvaluacion = crit.IdCriterioEvaluacion,
                    Puntuacion = nota,
                    Comentario = nota >= 4 ? "Cumplimiento satisfactorio y destacado" : "Área de oportunidad con potencial de mejora",
                    // Congelar snapshot histórico inmutable
                    PonderacionCategoriaHistorica = cat.Ponderacion,
                    NombreCategoriaHistorica = cat.Nombre,
                    TextoCriterioHistorico = crit.Texto
                });
            }

            decimal promCat = sumaNotas / critsDeCat.Count;
            decimal ponderadoCat = (promCat / 5.0m) * cat.Ponderacion;
            puntajeFinal += ponderadoCat;
        }

        eval.PuntajeFinal = Math.Round(puntajeFinal, 2);
        return (eval, detalles);
    }

    #endregion
}
