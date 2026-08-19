using AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.Commands;
using AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.DTOs;
using AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.Queries;
using AsistenciaPyme.Domain.Entities;
using AsistenciaPyme.Domain.Enums;
using AsistenciaPyme.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AsistenciaPyme.Tests;

public class EvaluacionDesempenoCriteriosCrudTests
{
    private AsistenciaPymeDbContext CrearContextoEnMemoria(string nombreBd)
    {
        var options = new DbContextOptionsBuilder<AsistenciaPymeDbContext>()
            .UseInMemoryDatabase(databaseName: nombreBd)
            .Options;

        return new AsistenciaPymeDbContext(options);
    }

    [Fact]
    public async Task CrearCriterio_NuevoCriterio_SePersisteConEstadoActivoYOrden()
    {
        var context = CrearContextoEnMemoria(Guid.NewGuid().ToString());

        var categoria = new CategoriaEvaluacion
        {
            IdCategoriaEvaluacion = 1,
            Nombre = "Rendimiento",
            Ponderacion = 20.00m,
            Orden = 1,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };
        context.CategoriasEvaluacion.Add(categoria);
        await context.SaveChangesAsync();

        var handler = new CrearCriterioHandler(context);
        var command = new CrearCriterioCommand
        {
            IdCategoriaEvaluacion = 1,
            Texto = "¿Cumple con los plazos de entrega establecidos?",
            Descripcion = "Evaluar entregables semanales",
            Orden = 1
        };

        int idCriterio = await handler.Handle(command, CancellationToken.None);
        Assert.True(idCriterio > 0);

        var criterioEnBd = await context.CriteriosEvaluacion.FindAsync(idCriterio);
        Assert.NotNull(criterioEnBd);
        Assert.Equal("¿Cumple con los plazos de entrega establecidos?", criterioEnBd.Texto);
        Assert.True(criterioEnBd.Activo);
        Assert.Equal(1, criterioEnBd.Orden);
    }

    [Fact]
    public async Task ActualizarCriterio_ModificaTextoYDescripcion_SinAfectarOtrasPropiedades()
    {
        var context = CrearContextoEnMemoria(Guid.NewGuid().ToString());

        var criterio = new CriterioEvaluacion
        {
            IdCriterioEvaluacion = 5,
            IdCategoriaEvaluacion = 1,
            Texto = "Texto original",
            Descripcion = "Guia original",
            Orden = 2,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };
        context.CriteriosEvaluacion.Add(criterio);
        await context.SaveChangesAsync();

        var handler = new ActualizarCriterioHandler(context);
        var command = new ActualizarCriterioCommand
        {
            IdCriterioEvaluacion = 5,
            Texto = "Texto editado y mejorado",
            Descripcion = "Nueva guia detallada",
            Orden = 3
        };

        bool exito = await handler.Handle(command, CancellationToken.None);
        Assert.True(exito);

        var modificado = await context.CriteriosEvaluacion.FindAsync(5);
        Assert.Equal("Texto editado y mejorado", modificado!.Texto);
        Assert.Equal("Nueva guia detallada", modificado.Descripcion);
        Assert.Equal(3, modificado.Orden);
        Assert.True(modificado.Activo);
    }

    [Fact]
    public async Task CambiarEstadoCriterio_DesactivarYActivar_ActualizaCampoActivoCorrectamente()
    {
        var context = CrearContextoEnMemoria(Guid.NewGuid().ToString());

        var criterio = new CriterioEvaluacion
        {
            IdCriterioEvaluacion = 10,
            IdCategoriaEvaluacion = 1,
            Texto = "Criterio de prueba",
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };
        context.CriteriosEvaluacion.Add(criterio);
        await context.SaveChangesAsync();

        var handler = new CambiarEstadoCriterioHandler(context);

        // Desactivar (Soft Delete)
        await handler.Handle(new CambiarEstadoCriterioCommand { IdCriterioEvaluacion = 10, Activo = false }, CancellationToken.None);
        var des = await context.CriteriosEvaluacion.FindAsync(10);
        Assert.False(des!.Activo);

        // Reactivar
        await handler.Handle(new CambiarEstadoCriterioCommand { IdCriterioEvaluacion = 10, Activo = true }, CancellationToken.None);
        var act = await context.CriteriosEvaluacion.FindAsync(10);
        Assert.True(act!.Activo);
    }

    [Fact]
    public async Task ConfigurarPonderaciones_SumaExacta100_ActualizaCorrectamente()
    {
        var context = CrearContextoEnMemoria(Guid.NewGuid().ToString());

        context.CategoriasEvaluacion.AddRange(
            new CategoriaEvaluacion { IdCategoriaEvaluacion = 1, Nombre = "Cat 1", Ponderacion = 50m, Activo = true, FechaCreacion = DateTime.UtcNow },
            new CategoriaEvaluacion { IdCategoriaEvaluacion = 2, Nombre = "Cat 2", Ponderacion = 50m, Activo = true, FechaCreacion = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var handler = new ConfigurarPonderacionesHandler(context);
        var command = new ConfigurarPonderacionesCommand
        {
            Ponderaciones = new List<PonderacionItemDto>
            {
                new PonderacionItemDto { IdCategoriaEvaluacion = 1, Ponderacion = 40.00m },
                new PonderacionItemDto { IdCategoriaEvaluacion = 2, Ponderacion = 60.00m }
            }
        };

        bool exito = await handler.Handle(command, CancellationToken.None);
        Assert.True(exito);

        var cat1 = await context.CategoriasEvaluacion.FindAsync(1);
        var cat2 = await context.CategoriasEvaluacion.FindAsync(2);
        Assert.Equal(40.00m, cat1!.Ponderacion);
        Assert.Equal(60.00m, cat2!.Ponderacion);
    }

    [Fact]
    public async Task ConfigurarPonderaciones_SumaDiferenteDe100_LanzaInvalidOperationException()
    {
        var context = CrearContextoEnMemoria(Guid.NewGuid().ToString());

        context.CategoriasEvaluacion.Add(new CategoriaEvaluacion { IdCategoriaEvaluacion = 1, Nombre = "Cat 1", Ponderacion = 50m, Activo = true, FechaCreacion = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var handler = new ConfigurarPonderacionesHandler(context);
        var command = new ConfigurarPonderacionesCommand
        {
            Ponderaciones = new List<PonderacionItemDto>
            {
                new PonderacionItemDto { IdCategoriaEvaluacion = 1, Ponderacion = 85.00m } // Suma != 100
            }
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(command, CancellationToken.None));

        Assert.Contains("100%", exception.Message);
    }

    [Fact]
    public async Task InmutabilidadHistorica_CompletarEvaluacion_CongelaSnapshotSinModificarsePorCambiosPosteriores()
    {
        var context = CrearContextoEnMemoria(Guid.NewGuid().ToString());

        var periodo = new PeriodoEvaluacion { IdPeriodoEvaluacion = 1, Nombre = "I Semestre 2026", FechaInicio = new DateOnly(2026, 1, 1), FechaFin = new DateOnly(2026, 6, 30), Estado = EstadoPeriodoEvaluacion.Activo, FechaCreacion = DateTime.UtcNow };
        var depto = new Departamento { IdDepartamento = 1, Nombre = "TI", Activo = true };
        var cargo = new Cargo { IdCargo = 1, Nombre = "Dev", Activo = true };
        var horario = new HorarioLaboral { IdHorarioLaboral = 1, Nombre = "D", Activo = true };

        var emp1 = new Empleado { IdEmpleado = 1, IdDepartamento = 1, IdCargo = 1, IdHorarioLaboral = 1, CodigoEmpleado = "E1", Identificacion = "001-000000-0001A", Nombres = "Colaborador", Apellidos = "Uno", SalarioBase = 20000, Estado = EstadoEmpleado.Activo, FechaContratacion = new DateOnly(2025, 1, 1) };
        var emp2 = new Empleado { IdEmpleado = 2, IdDepartamento = 1, IdCargo = 1, IdHorarioLaboral = 1, CodigoEmpleado = "E2", Identificacion = "001-000000-0002B", Nombres = "Jefe", Apellidos = "Dos", SalarioBase = 35000, Estado = EstadoEmpleado.Activo, FechaContratacion = new DateOnly(2025, 1, 1) };

        var cat = new CategoriaEvaluacion { IdCategoriaEvaluacion = 1, Nombre = "Rendimiento Original", Ponderacion = 100.00m, Orden = 1, Activo = true, FechaCreacion = DateTime.UtcNow };
        var crit = new CriterioEvaluacion { IdCriterioEvaluacion = 1, IdCategoriaEvaluacion = 1, Texto = "Pregunta Original", Orden = 1, Activo = true, FechaCreacion = DateTime.UtcNow };

        context.PeriodosEvaluacion.Add(periodo);
        context.Departamentos.Add(depto);
        context.Cargos.Add(cargo);
        context.HorariosLaborales.Add(horario);
        context.Empleados.AddRange(emp1, emp2);
        context.CategoriasEvaluacion.Add(cat);
        context.CriteriosEvaluacion.Add(crit);

        var eval = new EvaluacionDesempeno
        {
            IdEvaluacionDesempeno = 1,
            IdPeriodoEvaluacion = 1,
            IdEmpleadoEvaluado = 1,
            IdEvaluador = 2,
            TipoEvaluador = TipoEvaluador.JefeDirecto,
            Estado = EstadoEvaluacion.Pendiente,
            FechaAsignacion = DateTime.UtcNow
        };
        context.EvaluacionesDesempeno.Add(eval);
        await context.SaveChangesAsync();

        var completarHandler = new CompletarEvaluacionHandler(context);
        var completarCommand = new CompletarEvaluacionCommand
        {
            IdEvaluacionDesempeno = 1,
            Respuestas = new List<RespuestaCriterioDto>
            {
                new RespuestaCriterioDto { IdCriterioEvaluacion = 1, Puntuacion = 5, Comentario = "Excelente labor" }
            },
            ObservacionesGenerales = "Sin novedades"
        };

        decimal puntajeFinal = await completarHandler.Handle(completarCommand, CancellationToken.None);
        Assert.Equal(100.00m, puntajeFinal);
        Assert.Equal("Excelente", EvaluacionDesempenoDto.ObtenerClasificacion(puntajeFinal));

        // Verificar el snapshot guardado en detalle
        var detalleCongelado = await context.DetallesEvaluacionDesempeno.FirstOrDefaultAsync(d => d.IdEvaluacionDesempeno == 1 && d.IdCriterioEvaluacion == 1);
        Assert.NotNull(detalleCongelado);
        Assert.Equal("Pregunta Original", detalleCongelado.TextoCriterioHistorico);
        Assert.Equal("Rendimiento Original", detalleCongelado.NombreCategoriaHistorica);
        Assert.Equal(100.00m, detalleCongelado.PonderacionCategoriaHistorica);

        // AHORA simulamos que el administrador cambia el texto del criterio y cambia las ponderaciones a futuro
        crit.Texto = "Pregunta Modificada a Futuro";
        cat.Nombre = "Rendimiento Renombrado";
        cat.Ponderacion = 50.00m;
        await context.SaveChangesAsync();

        // Verificar que el detalle de la evaluación completada permanece intacto
        var detallePosterior = await context.DetallesEvaluacionDesempeno.FindAsync(detalleCongelado.IdDetalleEvaluacionDesempeno);
        Assert.Equal("Pregunta Original", detallePosterior!.TextoCriterioHistorico);
        Assert.Equal("Rendimiento Original", detallePosterior.NombreCategoriaHistorica);
        Assert.Equal(100.00m, detallePosterior.PonderacionCategoriaHistorica);
    }
}
