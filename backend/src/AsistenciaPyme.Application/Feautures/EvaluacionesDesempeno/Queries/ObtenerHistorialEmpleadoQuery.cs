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

public class ObtenerHistorialEmpleadoQuery : IRequest<HistorialEvaluacionEmpleadoDto?>
{
    public int IdEmpleado { get; set; }
}

public class ObtenerHistorialEmpleadoHandler : IRequestHandler<ObtenerHistorialEmpleadoQuery, HistorialEvaluacionEmpleadoDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerHistorialEmpleadoHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<HistorialEvaluacionEmpleadoDto?> Handle(ObtenerHistorialEmpleadoQuery request, CancellationToken cancellationToken)
    {
        var empleado = await _context.Empleados
            .Include(e => e.Cargo)
            .Include(e => e.Departamento)
            .FirstOrDefaultAsync(e => e.IdEmpleado == request.IdEmpleado, cancellationToken);

        if (empleado is null)
        {
            return null;
        }

        var evaluaciones = await _context.EvaluacionesDesempeno
            .Include(e => e.Periodo)
            .Where(e => e.IdEmpleadoEvaluado == request.IdEmpleado)
            .ToListAsync(cancellationToken);

        var periodosAgrupados = evaluaciones
            .GroupBy(e => e.Periodo)
            .OrderByDescending(g => g.Key.FechaInicio)
            .Select(g =>
            {
                var completadas = g.Where(e => e.Estado == EstadoEvaluacion.Completada && e.PuntajeFinal.HasValue).ToList();

                var autos = completadas.Where(e => e.TipoEvaluador == TipoEvaluador.Autoevaluacion).ToList();
                var jefes = completadas.Where(e => e.TipoEvaluador == TipoEvaluador.JefeDirecto).ToList();
                var subs = completadas.Where(e => e.TipoEvaluador == TipoEvaluador.Subordinado).ToList();

                var persps = new List<decimal>();
                if (autos.Any()) persps.Add(autos.Average(e => e.PuntajeFinal!.Value));
                if (jefes.Any()) persps.Add(jefes.Average(e => e.PuntajeFinal!.Value));
                if (subs.Any()) persps.Add(subs.Average(e => e.PuntajeFinal!.Value));

                decimal? consolidado = persps.Any() ? Math.Round(persps.Average(), 2) : null;

                return new PeriodoHistorialDto
                {
                    IdPeriodoEvaluacion = g.Key.IdPeriodoEvaluacion,
                    NombrePeriodo = g.Key.Nombre,
                    FechaInicio = g.Key.FechaInicio,
                    FechaFin = g.Key.FechaFin,
                    PuntajeConsolidado = consolidado,
                    EvaluacionesRealizadas = completadas.Count
                };
            })
            .ToList();

        return new HistorialEvaluacionEmpleadoDto
        {
            IdEmpleado = empleado.IdEmpleado,
            CodigoEmpleado = empleado.CodigoEmpleado,
            NombreCompleto = $"{empleado.Nombres} {empleado.Apellidos}".Trim(),
            Cargo = empleado.Cargo.Nombre,
            Departamento = empleado.Departamento?.Nombre ?? "Sin departamento",
            Periodos = periodosAgrupados
        };
    }
}
