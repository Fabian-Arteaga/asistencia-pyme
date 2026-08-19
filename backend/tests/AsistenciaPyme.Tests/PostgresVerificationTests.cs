using AsistenciaPyme.Domain.Enums;
using AsistenciaPyme.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AsistenciaPyme.Tests;

public class PostgresVerificationTests
{
    private readonly ITestOutputHelper _output;

    public PostgresVerificationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private AsistenciaPymeDbContext ConectarPostgres()
    {
        var connectionString = "Host=localhost;Port=5432;Database=asistencia_pyme;Username=postgres;Password=fabian2210";
        var options = new DbContextOptionsBuilder<AsistenciaPymeDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AsistenciaPymeDbContext(options);
    }

    [Fact]
    public async Task VerificarPostgreSQL_EstadoRealYConteoExhaustivo()
    {
        using var context = ConectarPostgres();

        // 1. Conteos
        int deptos = await context.Departamentos.CountAsync();
        int cargos = await context.Cargos.CountAsync();
        int empleados = await context.Empleados.CountAsync();
        int activos = await context.Empleados.CountAsync(e => e.Estado == EstadoEmpleado.Activo);
        int inactivos = await context.Empleados.CountAsync(e => e.Estado == EstadoEmpleado.Inactivo);
        int historialesDepto = await context.EmpleadoDepartamentoHistorials.CountAsync();
        int asistencias = await context.Asistencias.CountAsync();
        int vacaciones = await context.Vacaciones.CountAsync();
        int horasExtras = await context.HorasExtras.CountAsync();
        int planillas = await context.Planillas.CountAsync();
        int detallesPlanilla = await context.DetallesPlanilla.CountAsync();
        int periodos = await context.PeriodosEvaluacion.CountAsync();
        int evaluaciones = await context.EvaluacionesDesempeno.CountAsync();
        int detallesEval = await context.DetallesEvaluacionDesempeno.CountAsync();

        _output.WriteLine($"[POSTGRES] Departamentos: {deptos}");
        _output.WriteLine($"[POSTGRES] Cargos: {cargos}");
        _output.WriteLine($"[POSTGRES] Empleados: {empleados} (Activos: {activos}, Inactivos: {inactivos})");
        _output.WriteLine($"[POSTGRES] Historiales Depto: {historialesDepto}");
        _output.WriteLine($"[POSTGRES] Asistencias: {asistencias}");
        _output.WriteLine($"[POSTGRES] Vacaciones: {vacaciones}");
        _output.WriteLine($"[POSTGRES] Horas Extras: {horasExtras}");
        _output.WriteLine($"[POSTGRES] Planillas: {planillas}");
        _output.WriteLine($"[POSTGRES] Detalles Planilla: {detallesPlanilla}");
        _output.WriteLine($"[POSTGRES] Periodos Evaluación: {periodos}");
        _output.WriteLine($"[POSTGRES] Evaluaciones: {evaluaciones}");
        _output.WriteLine($"[POSTGRES] Detalles Evaluación: {detallesEval}");

        // 2. Rangos Temporales
        var primeraAsistencia = await context.Asistencias.MinAsync(a => a.HoraEntrada);
        var ultimaAsistencia = await context.Asistencias.MaxAsync(a => a.HoraEntrada);
        var primeraPlanilla = await context.Planillas.MinAsync(p => p.FechaInicioPeriodo);
        var ultimaPlanilla = await context.Planillas.MaxAsync(p => p.FechaInicioPeriodo);
        var primerPeriodo = await context.PeriodosEvaluacion.OrderBy(p => p.FechaInicio).FirstAsync();
        var ultimoPeriodo = await context.PeriodosEvaluacion.OrderByDescending(p => p.FechaInicio).FirstAsync();

        _output.WriteLine($"[POSTGRES] Primera Asistencia: {primeraAsistencia:yyyy-MM-dd HH:mm:ss}");
        _output.WriteLine($"[POSTGRES] Última Asistencia: {ultimaAsistencia:yyyy-MM-dd HH:mm:ss}");
        _output.WriteLine($"[POSTGRES] Primera Planilla: {primeraPlanilla:yyyy-MM-dd}");
        _output.WriteLine($"[POSTGRES] Última Planilla: {ultimaPlanilla:yyyy-MM-dd}");
        _output.WriteLine($"[POSTGRES] Primer Periodo: {primerPeriodo.Nombre} ({primerPeriodo.FechaInicio:yyyy-MM-dd})");
        _output.WriteLine($"[POSTGRES] Último Periodo: {ultimoPeriodo.Nombre} ({ultimoPeriodo.FechaInicio:yyyy-MM-dd})");

        // 3. Invariantes del dataset histórico sembrado
        var codigosHistoricos = Enumerable.Range(1, 29).Select(i => $"EMP-2014-0{i}").Concat(
            new[] { "EMP-2014-01", "EMP-2014-02", "EMP-2014-03", "EMP-2014-04", "EMP-2014-05", "EMP-2014-06", "EMP-2014-07", "EMP-2014-08",
                    "EMP-2015-01", "EMP-2015-02", "EMP-2016-01", "EMP-2016-02", "EMP-2017-01", "EMP-2017-02", "EMP-2018-01", "EMP-2018-02",
                    "EMP-2019-01", "EMP-2019-02", "EMP-2020-01", "EMP-2020-02", "EMP-2021-01", "EMP-2021-02", "EMP-2022-01",
                    "EMP-2023-01", "EMP-2023-02", "EMP-2024-01", "EMP-2024-02", "EMP-2025-01", "EMP-2026-01" }).Distinct().ToList();

        var empsHistoricos = await context.Empleados.Where(e => codigosHistoricos.Contains(e.CodigoEmpleado)).ToListAsync();
        var idsHistoricos = empsHistoricos.Select(e => e.IdEmpleado).ToList();

        var asistenciasHistoricas = await context.Asistencias.Where(a => idsHistoricos.Contains(a.IdEmpleado)).ToListAsync();
        var fechaLimiteUtc = new DateTime(2026, 8, 19, 23, 59, 59, DateTimeKind.Utc);
        int asistenciasFuturasHistoricas = asistenciasHistoricas.Count(a => a.HoraEntrada > fechaLimiteUtc);
        Assert.Equal(0, asistenciasFuturasHistoricas);

        var empDict = empsHistoricos.ToDictionary(e => e.IdEmpleado);
        int antesContratacion = asistenciasHistoricas.Count(a => a.HoraEntrada.Date < empDict[a.IdEmpleado].FechaContratacion.ToDateTime(TimeOnly.MinValue));
        Assert.Equal(0, antesContratacion);

        // Planillas del dataset histórico
        var planillasHistoricas = await context.Planillas.Include(p => p.Detalles).Where(p => p.FechaInicioPeriodo < new DateOnly(2026, 8, 1)).ToListAsync();
        int planillasSinDetalles = planillasHistoricas.Count(p => !p.Detalles.Any());
        Assert.Equal(0, planillasSinDetalles);

        // 2026-II
        var p2026_2 = await context.PeriodosEvaluacion.FirstAsync(p => p.Nombre == "2026-II");
        Assert.Equal(EstadoPeriodoEvaluacion.Activo, p2026_2.Estado);

        var evals2026_2 = await context.EvaluacionesDesempeno.Where(e => e.IdPeriodoEvaluacion == p2026_2.IdPeriodoEvaluacion).ToListAsync();
        int comp2026_2 = evals2026_2.Count(e => e.Estado == EstadoEvaluacion.Completada);
        int pend2026_2 = evals2026_2.Count(e => e.Estado == EstadoEvaluacion.Pendiente);
        int proc2026_2 = evals2026_2.Count(e => e.Estado == EstadoEvaluacion.EnProceso);

        _output.WriteLine($"[POSTGRES] Dataset Histórico - Asistencias: {asistenciasHistoricas.Count} (Max: {asistenciasHistoricas.Max(a => a.HoraEntrada):yyyy-MM-dd HH:mm:ss})");
        _output.WriteLine($"[POSTGRES] 2026-II: Estado={p2026_2.Estado}, Completadas={comp2026_2}, Pendientes={pend2026_2}, EnProceso={proc2026_2}");
        Assert.Equal(0, comp2026_2);
        Assert.True(pend2026_2 > 0);
        Assert.True(proc2026_2 > 0);
    }
}
