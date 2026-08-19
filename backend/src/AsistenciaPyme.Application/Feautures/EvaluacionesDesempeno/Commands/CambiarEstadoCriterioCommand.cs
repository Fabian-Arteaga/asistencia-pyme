using AsistenciaPyme.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.Commands;

public class CambiarEstadoCriterioCommand : IRequest<bool>
{
    public int IdCriterioEvaluacion { get; set; }
    public bool Activo { get; set; }
}

public class CambiarEstadoCriterioHandler : IRequestHandler<CambiarEstadoCriterioCommand, bool>
{
    private readonly IAsistenciaPymeDbContext _context;

    public CambiarEstadoCriterioHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CambiarEstadoCriterioCommand request, CancellationToken cancellationToken)
    {
        var criterio = await _context.CriteriosEvaluacion
            .FirstOrDefaultAsync(c => c.IdCriterioEvaluacion == request.IdCriterioEvaluacion, cancellationToken);

        if (criterio is null)
        {
            throw new InvalidOperationException("No se encontró el criterio de evaluación.");
        }

        criterio.Activo = request.Activo;
        criterio.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
