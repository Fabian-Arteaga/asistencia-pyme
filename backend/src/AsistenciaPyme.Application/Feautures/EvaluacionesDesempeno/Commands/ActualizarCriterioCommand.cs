using AsistenciaPyme.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.Commands;

public class ActualizarCriterioCommand : IRequest<bool>
{
    public int IdCriterioEvaluacion { get; set; }
    public string Texto { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int Orden { get; set; }
}

public class ActualizarCriterioHandler : IRequestHandler<ActualizarCriterioCommand, bool>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ActualizarCriterioHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ActualizarCriterioCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Texto))
        {
            throw new InvalidOperationException("El texto del criterio es obligatorio.");
        }

        var criterio = await _context.CriteriosEvaluacion
            .FirstOrDefaultAsync(c => c.IdCriterioEvaluacion == request.IdCriterioEvaluacion, cancellationToken);

        if (criterio is null)
        {
            throw new InvalidOperationException("No se encontró el criterio de evaluación.");
        }

        criterio.Texto = request.Texto.Trim();
        criterio.Descripcion = request.Descripcion?.Trim();
        if (request.Orden > 0)
        {
            criterio.Orden = request.Orden;
        }
        criterio.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
