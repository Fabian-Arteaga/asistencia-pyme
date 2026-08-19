using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.DTOs;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.Queries;

public class ObtenerPeriodoActivoQuery : IRequest<PeriodoEvaluacionDto?>
{
}

public class ObtenerPeriodoActivoHandler : IRequestHandler<ObtenerPeriodoActivoQuery, PeriodoEvaluacionDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerPeriodoActivoHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<PeriodoEvaluacionDto?> Handle(ObtenerPeriodoActivoQuery request, CancellationToken cancellationToken)
    {
        var periodo = await _context.PeriodosEvaluacion
            .Include(p => p.Evaluaciones)
            .Where(p => p.Estado == EstadoPeriodoEvaluacion.Activo)
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
            .FirstOrDefaultAsync(cancellationToken);

        return periodo;
    }
}
