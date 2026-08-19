using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.Commands;

public class CambiarEstadoPeriodoEvaluacionCommand : IRequest<bool>
{
    public int IdPeriodoEvaluacion { get; set; }
    public EstadoPeriodoEvaluacion NuevoEstado { get; set; }
}

public class CambiarEstadoPeriodoEvaluacionHandler : IRequestHandler<CambiarEstadoPeriodoEvaluacionCommand, bool>
{
    private readonly IAsistenciaPymeDbContext _context;

    public CambiarEstadoPeriodoEvaluacionHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CambiarEstadoPeriodoEvaluacionCommand request, CancellationToken cancellationToken)
    {
        var periodo = await _context.PeriodosEvaluacion
            .FirstOrDefaultAsync(p => p.IdPeriodoEvaluacion == request.IdPeriodoEvaluacion, cancellationToken);

        if (periodo is null)
        {
            throw new InvalidOperationException("No se encontró el período de evaluación.");
        }

        // Si se activa este período, asegurar que no existan múltiples activos a la vez si es la regla
        if (request.NuevoEstado == EstadoPeriodoEvaluacion.Activo)
        {
            var otrosActivos = await _context.PeriodosEvaluacion
                .Where(p => p.IdPeriodoEvaluacion != request.IdPeriodoEvaluacion && p.Estado == EstadoPeriodoEvaluacion.Activo)
                .ToListAsync(cancellationToken);

            foreach (var otro in otrosActivos)
            {
                otro.Estado = EstadoPeriodoEvaluacion.Finalizado;
                otro.FechaActualizacion = DateTime.UtcNow;
            }
        }

        periodo.Estado = request.NuevoEstado;
        periodo.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
