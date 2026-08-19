using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.DTOs;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.Queries;

public class ObtenerResultadoConsolidado360Query : IRequest<ResultadoConsolidado360Dto?>
{
    public int IdEmpleado { get; set; }
    public int IdPeriodoEvaluacion { get; set; }
}

public class ObtenerResultadoConsolidado360Handler : IRequestHandler<ObtenerResultadoConsolidado360Query, ResultadoConsolidado360Dto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerResultadoConsolidado360Handler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<ResultadoConsolidado360Dto?> Handle(ObtenerResultadoConsolidado360Query request, CancellationToken cancellationToken)
    {
        var empleado = await _context.Empleados
            .Include(e => e.Cargo)
            .Include(e => e.Departamento)
            .FirstOrDefaultAsync(e => e.IdEmpleado == request.IdEmpleado, cancellationToken);

        if (empleado is null)
        {
            return null;
        }

        var periodo = await _context.PeriodosEvaluacion
            .FirstOrDefaultAsync(p => p.IdPeriodoEvaluacion == request.IdPeriodoEvaluacion, cancellationToken);

        if (periodo is null)
        {
            return null;
        }

        var evaluaciones = await _context.EvaluacionesDesempeno
            .Include(e => e.Evaluador).ThenInclude(eval => eval.Cargo)
            .Include(e => e.Detalles).ThenInclude(d => d.Criterio).ThenInclude(c => c.Categoria)
            .Where(e => e.IdEmpleadoEvaluado == request.IdEmpleado && e.IdPeriodoEvaluacion == request.IdPeriodoEvaluacion)
            .ToListAsync(cancellationToken);

        var completadas = evaluaciones.Where(e => e.Estado == EstadoEvaluacion.Completada && e.PuntajeFinal.HasValue).ToList();

        // 1. Promedios por perspectiva 360°
        var autos = completadas.Where(e => e.TipoEvaluador == TipoEvaluador.Autoevaluacion).ToList();
        var jefes = completadas.Where(e => e.TipoEvaluador == TipoEvaluador.JefeDirecto).ToList();
        var subs = completadas.Where(e => e.TipoEvaluador == TipoEvaluador.Subordinado).ToList();

        decimal? promAuto = autos.Any() ? Math.Round(autos.Average(e => e.PuntajeFinal!.Value), 2) : null;
        decimal? promJefe = jefes.Any() ? Math.Round(jefes.Average(e => e.PuntajeFinal!.Value), 2) : null;
        decimal? promSubs = subs.Any() ? Math.Round(subs.Average(e => e.PuntajeFinal!.Value), 2) : null;

        // 2. Consolidado general (promedio equitativo de las perspectivas disponibles)
        var listaPerspectivasValores = new List<decimal>();
        if (promAuto.HasValue) listaPerspectivasValores.Add(promAuto.Value);
        if (promJefe.HasValue) listaPerspectivasValores.Add(promJefe.Value);
        if (promSubs.HasValue) listaPerspectivasValores.Add(promSubs.Value);

        decimal? puntajeConsolidado = listaPerspectivasValores.Any()
            ? Math.Round(listaPerspectivasValores.Average(), 2)
            : null;

        // 3. Resumen de perspectivas
        var perspectivasDto = new List<Perspectiva360Dto>
        {
            new Perspectiva360Dto
            {
                Perspectiva = "Autoevaluación",
                TipoEvaluador = "Autoevaluacion",
                CantidadEvaluadores = evaluaciones.Count(e => e.TipoEvaluador == TipoEvaluador.Autoevaluacion),
                PuntajePromedio = promAuto,
                Completada = autos.Any()
            },
            new Perspectiva360Dto
            {
                Perspectiva = "Jefe directo",
                TipoEvaluador = "JefeDirecto",
                CantidadEvaluadores = evaluaciones.Count(e => e.TipoEvaluador == TipoEvaluador.JefeDirecto),
                PuntajePromedio = promJefe,
                Completada = jefes.Any()
            },
            new Perspectiva360Dto
            {
                Perspectiva = "Subordinados",
                TipoEvaluador = "Subordinado",
                CantidadEvaluadores = evaluaciones.Count(e => e.TipoEvaluador == TipoEvaluador.Subordinado),
                PuntajePromedio = promSubs,
                Completada = subs.Any()
            }
        };

        // 4. Desglose consolidado por categorías
        var categoriasConsolidadas = new List<PuntajeCategoriaDto>();
        var categoriasActivas = await _context.CategoriasEvaluacion
            .Where(c => c.Activo)
            .OrderBy(c => c.Orden)
            .ToListAsync(cancellationToken);

        // Agrupar todos los detalles de evaluaciones completadas
        var todosDetallesCompletados = completadas.SelectMany(e => e.Detalles).ToList();

        foreach (var cat in categoriasActivas)
        {
            var detallesCat = todosDetallesCompletados
                .Where(d => d.NombreCategoriaHistorica == cat.Nombre || (d.Criterio != null && d.Criterio.IdCategoriaEvaluacion == cat.IdCategoriaEvaluacion))
                .ToList();

            decimal ponderacion = cat.Ponderacion;
            decimal promedioRespuestas = detallesCat.Any() ? Math.Round((decimal)detallesCat.Average(d => d.Puntuacion), 2) : 0m;
            decimal rendimientoPorcentaje = Math.Round((promedioRespuestas / 5.0m) * 100m, 2);
            decimal puntosObtenidos = Math.Round((promedioRespuestas / 5.0m) * ponderacion, 2);

            categoriasConsolidadas.Add(new PuntajeCategoriaDto
            {
                IdCategoriaEvaluacion = cat.IdCategoriaEvaluacion,
                NombreCategoria = cat.Nombre,
                Ponderacion = ponderacion,
                PromedioRespuestas = promedioRespuestas,
                RendimientoPorcentaje = rendimientoPorcentaje,
                PuntosObtenidos = puntosObtenidos
            });
        }

        // 5. Lista individual de evaluaciones
        var evaluacionesIndividuales = evaluaciones.Select(e => new EvaluacionDesempenoDto
        {
            IdEvaluacionDesempeno = e.IdEvaluacionDesempeno,
            IdPeriodoEvaluacion = e.IdPeriodoEvaluacion,
            NombrePeriodo = periodo.Nombre,
            IdEmpleadoEvaluado = e.IdEmpleadoEvaluado,
            CodigoEmpleadoEvaluado = empleado.CodigoEmpleado,
            NombreCompletoEvaluado = $"{empleado.Nombres} {empleado.Apellidos}".Trim(),
            CargoEvaluado = empleado.Cargo.Nombre,
            DepartamentoEvaluado = empleado.Departamento?.Nombre ?? "Sin departamento",
            IdEvaluador = e.IdEvaluador,
            CodigoEvaluador = e.Evaluador.CodigoEmpleado,
            NombreCompletoEvaluador = $"{e.Evaluador.Nombres} {e.Evaluador.Apellidos}".Trim(),
            CargoEvaluador = e.Evaluador.Cargo.Nombre,
            TipoEvaluador = e.TipoEvaluador,
            Estado = e.Estado,
            PuntajeFinal = e.PuntajeFinal,
            ObservacionesGenerales = e.ObservacionesGenerales,
            FechaAsignacion = e.FechaAsignacion,
            FechaCompletada = e.FechaCompletada
        }).ToList();

        return new ResultadoConsolidado360Dto
        {
            IdEmpleado = empleado.IdEmpleado,
            CodigoEmpleado = empleado.CodigoEmpleado,
            NombreCompleto = $"{empleado.Nombres} {empleado.Apellidos}".Trim(),
            Cargo = empleado.Cargo.Nombre,
            Departamento = empleado.Departamento?.Nombre ?? "Sin departamento",
            IdPeriodoEvaluacion = periodo.IdPeriodoEvaluacion,
            NombrePeriodo = periodo.Nombre,
            PromedioAutoevaluacion = promAuto,
            PromedioJefeDirecto = promJefe,
            PromedioSubordinados = promSubs,
            PuntajeConsolidado = puntajeConsolidado,
            Perspectivas = perspectivasDto,
            CategoriasConsolidadas = categoriasConsolidadas,
            EvaluacionesIndividuales = evaluacionesIndividuales
        };
    }
}
