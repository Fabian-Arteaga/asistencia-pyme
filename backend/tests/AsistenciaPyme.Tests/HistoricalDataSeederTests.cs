using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Domain.Entities;
using AsistenciaPyme.Domain.Enums;
using AsistenciaPyme.Infrastructure.Persistence;
using AsistenciaPyme.Infrastructure.Persistence.Seeding;
using AsistenciaPyme.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AsistenciaPyme.Tests;

public class HistoricalDataSeederTests
{
    private AsistenciaPymeDbContext CrearContexto(string dbName)
    {
        var options = new DbContextOptionsBuilder<AsistenciaPymeDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var context = new AsistenciaPymeDbContext(options);

        // Sembrar categorías y criterios iniciales si no existen
        if (!context.CategoriasEvaluacion.Any())
        {
            var cat1 = new CategoriaEvaluacion { IdCategoriaEvaluacion = 1, Nombre = "Rendimiento", Ponderacion = 20m, Orden = 1, Activo = true };
            var cat2 = new CategoriaEvaluacion { IdCategoriaEvaluacion = 2, Nombre = "Objetivos", Ponderacion = 15m, Orden = 2, Activo = true };
            var cat3 = new CategoriaEvaluacion { IdCategoriaEvaluacion = 3, Nombre = "Responsabilidad", Ponderacion = 15m, Orden = 3, Activo = true };
            var cat4 = new CategoriaEvaluacion { IdCategoriaEvaluacion = 4, Nombre = "Competencias", Ponderacion = 15m, Orden = 4, Activo = true };
            var cat5 = new CategoriaEvaluacion { IdCategoriaEvaluacion = 5, Nombre = "Aptitudes", Ponderacion = 10m, Orden = 5, Activo = true };
            var cat6 = new CategoriaEvaluacion { IdCategoriaEvaluacion = 6, Nombre = "Iniciativa", Ponderacion = 15m, Orden = 6, Activo = true };
            var cat7 = new CategoriaEvaluacion { IdCategoriaEvaluacion = 7, Nombre = "Creatividad", Ponderacion = 10m, Orden = 7, Activo = true };

            context.CategoriasEvaluacion.AddRange(cat1, cat2, cat3, cat4, cat5, cat6, cat7);

            var crits = new[]
            {
                new CriterioEvaluacion { IdCriterioEvaluacion = 1, IdCategoriaEvaluacion = 1, Texto = "Pregunta R1", Orden = 1, Activo = true },
                new CriterioEvaluacion { IdCriterioEvaluacion = 2, IdCategoriaEvaluacion = 2, Texto = "Pregunta O1", Orden = 1, Activo = true },
                new CriterioEvaluacion { IdCriterioEvaluacion = 3, IdCategoriaEvaluacion = 3, Texto = "Pregunta Resp1", Orden = 1, Activo = true },
                new CriterioEvaluacion { IdCriterioEvaluacion = 4, IdCategoriaEvaluacion = 4, Texto = "Pregunta Comp1", Orden = 1, Activo = true },
                new CriterioEvaluacion { IdCriterioEvaluacion = 5, IdCategoriaEvaluacion = 5, Texto = "Pregunta Apt1", Orden = 1, Activo = true },
                new CriterioEvaluacion { IdCriterioEvaluacion = 6, IdCategoriaEvaluacion = 6, Texto = "Pregunta Ini1", Orden = 1, Activo = true },
                new CriterioEvaluacion { IdCriterioEvaluacion = 7, IdCategoriaEvaluacion = 7, Texto = "Pregunta Crea1", Orden = 1, Activo = true }
            };

            context.CriteriosEvaluacion.AddRange(crits);
            context.SaveChanges();
        }

        return context;
    }

    [Fact]
    public async Task Seeder_EjecucionCompleta_GeneraDatasetCoherenteEstructurado()
    {
        var dbName = Guid.NewGuid().ToString();
        var context = CrearContexto(dbName);
        var pinHasher = new PinHasher();
        var contrasenaHasher = new ContrasenaHasher();
        var calculadorHorasExtras = new CalculadorHorasExtras();

        var seeder = new HistoricalDataSeeder(context, pinHasher, contrasenaHasher, calculadorHorasExtras);

        // Ejecutar seeder
        await seeder.SeedHistoricalDataAsync(CancellationToken.None);

        // 1. Validar empleados
        var empleados = await context.Empleados
            .Include(e => e.Departamento)
            .Include(e => e.Cargo)
            .Include(e => e.JefeDirecto)
            .Include(e => e.Subordinados)
            .ToListAsync();

        Assert.NotEmpty(empleados);
        Assert.InRange(empleados.Count, 20, 35);
        Assert.Contains(empleados, e => e.Estado == EstadoEmpleado.Inactivo); // Empleados históricos con baja
        Assert.Contains(empleados, e => e.Estado == EstadoEmpleado.Activo);

        // 2. Validar que no existen ciclos en la jerarquía
        foreach (var emp in empleados)
        {
            if (emp.IdJefeDirecto.HasValue)
            {
                Assert.NotEqual(emp.IdEmpleado, emp.IdJefeDirecto.Value);
            }
        }

        // 3. Validar consistencia temporal y volumen de asistencias (~1800 - 2200)
        var asistencias = await context.Asistencias.ToListAsync();
        Assert.NotEmpty(asistencias);
        Assert.InRange(asistencias.Count, 1800, 2200);

        var fechaLimite = new DateTime(2026, 8, 19, 23, 59, 59, DateTimeKind.Utc);
        Assert.All(asistencias, a =>
        {
            Assert.True(a.HoraEntrada >= new DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            Assert.True(a.HoraEntrada <= fechaLimite, $"Asistencia {a.HoraEntrada} excede fecha límite");
        });

        // 4. Validar períodos de evaluación (2014-I hasta 2026-II)
        var periodos = await context.PeriodosEvaluacion.ToListAsync();
        Assert.Equal(26, periodos.Count); // 13 años (2014 a 2026) * 2 semestres = 26
        Assert.Contains(periodos, p => p.Nombre == "2014-I" && p.Estado == EstadoPeriodoEvaluacion.Finalizado);
        Assert.Contains(periodos, p => p.Nombre == "2026-I" && p.Estado == EstadoPeriodoEvaluacion.Finalizado);
        Assert.Contains(periodos, p => p.Nombre == "2026-II" && p.Estado == EstadoPeriodoEvaluacion.Activo);

        // 5. Validar evaluaciones 360°, estado de 2026-II y snapshots congelados
        var evaluaciones = await context.EvaluacionesDesempeno
            .Include(e => e.Detalles)
            .ToListAsync();

        Assert.NotEmpty(evaluaciones);
        Assert.Contains(evaluaciones, e => e.TipoEvaluador == TipoEvaluador.Autoevaluacion);
        Assert.Contains(evaluaciones, e => e.TipoEvaluador == TipoEvaluador.JefeDirecto);

        // Validar que 2026-II NO tenga evaluaciones completadas, pero sí en proceso y pendientes
        var periodo2026_2 = periodos.First(p => p.Nombre == "2026-II");
        var evals2026_2 = evaluaciones.Where(e => e.IdPeriodoEvaluacion == periodo2026_2.IdPeriodoEvaluacion).ToList();
        Assert.NotEmpty(evals2026_2);
        Assert.DoesNotContain(evals2026_2, e => e.Estado == EstadoEvaluacion.Completada);
        Assert.Contains(evals2026_2, e => e.Estado == EstadoEvaluacion.EnProceso);
        Assert.Contains(evals2026_2, e => e.Estado == EstadoEvaluacion.Pendiente);

        var detalles = await context.DetallesEvaluacionDesempeno.ToListAsync();
        Assert.NotEmpty(detalles);
        Assert.All(detalles, d =>
        {
            Assert.False(string.IsNullOrWhiteSpace(d.NombreCategoriaHistorica));
            Assert.False(string.IsNullOrWhiteSpace(d.TextoCriterioHistorico));
            Assert.True(d.PonderacionCategoriaHistorica > 0m);
            Assert.InRange(d.Puntuacion, 1, 5);
        });

        // 6. Validar planillas departamentales
        var planillas = await context.Planillas
            .Include(p => p.Detalles)
            .ToListAsync();

        Assert.NotEmpty(planillas);
        Assert.All(planillas, p =>
        {
            Assert.True(p.CantidadEmpleados > 0);
            Assert.True(p.TotalNeto > 0);
            Assert.Equal(p.CantidadEmpleados, p.Detalles.Count);
        });
    }

    [Fact]
    public async Task Seeder_EjecucionDoble_EsEstrictamenteIdempotente()
    {
        var dbName = Guid.NewGuid().ToString();
        var context = CrearContexto(dbName);
        var pinHasher = new PinHasher();
        var contrasenaHasher = new ContrasenaHasher();
        var calculadorHorasExtras = new CalculadorHorasExtras();

        var seeder = new HistoricalDataSeeder(context, pinHasher, contrasenaHasher, calculadorHorasExtras);

        // Primera ejecución
        await seeder.SeedHistoricalDataAsync(CancellationToken.None);
        int totalEmpleados1 = await context.Empleados.CountAsync();
        int totalPeriodos1 = await context.PeriodosEvaluacion.CountAsync();
        int totalAsistencias1 = await context.Asistencias.CountAsync();
        int totalPlanillas1 = await context.Planillas.CountAsync();
        int totalEvaluaciones1 = await context.EvaluacionesDesempeno.CountAsync();

        // Segunda ejecución inmediata
        await seeder.SeedHistoricalDataAsync(CancellationToken.None);
        int totalEmpleados2 = await context.Empleados.CountAsync();
        int totalPeriodos2 = await context.PeriodosEvaluacion.CountAsync();
        int totalAsistencias2 = await context.Asistencias.CountAsync();
        int totalPlanillas2 = await context.Planillas.CountAsync();
        int totalEvaluaciones2 = await context.EvaluacionesDesempeno.CountAsync();

        // Verificar que ningún registro fue duplicado
        Assert.Equal(totalEmpleados1, totalEmpleados2);
        Assert.Equal(totalPeriodos1, totalPeriodos2);
        Assert.Equal(totalAsistencias1, totalAsistencias2);
        Assert.Equal(totalPlanillas1, totalPlanillas2);
        Assert.Equal(totalEvaluaciones1, totalEvaluaciones2);
    }
}
