using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.DTOs;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.Queries;

public class ObtenerPeriodosEvaluacionQuery : IRequest<List<PeriodoEvaluacionDto>>
{
}

public class ObtenerPeriodosEvaluacionHandler : IRequestHandler<ObtenerPeriodosEvaluacionQuery, List<PeriodoEvaluacionDto>>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerPeriodosEvaluacionHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<List<PeriodoEvaluacionDto>> Handle(ObtenerPeriodosEvaluacionQuery request, CancellationToken cancellationToken)
    {
        var periodos = await _context.PeriodosEvaluacion
            .Include(p => p.Evaluaciones)
            .OrderByDescending(p => p.FechaInicio)
            .Select(p => new PeriodoEvaluacionDto
            {
                IdPeriodoEvaluacion = p.IdPeriodoEvaluacion,
                Nombre = p.Nombre,
                FechaInicio = p.FechaInicio,
                FechaFin = p.FechaFin,
                Estado = p.Estado,
                FechaCreacion = p.FechaCreacion,
                TotalEvaluaciones = p.Evaluaciones.Count,
                EvaluacionesCompletadas = p.Evaluaciones.Count(e => e.Estado == EstadoEvaluacion.Completada),
                EvaluacionesPendientes = p.Evaluaciones.Count(e => e.Estado != EstadoEvaluacion.Completada)
            })
            .ToListAsync(cancellationToken);

        return periodos;
    }
}
