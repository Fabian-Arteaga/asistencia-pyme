using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.Commands;

public class CrearCriterioCommand : IRequest<int>
{
    public int IdCategoriaEvaluacion { get; set; }
    public string Texto { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int Orden { get; set; }
}

public class CrearCriterioHandler : IRequestHandler<CrearCriterioCommand, int>
{
    private readonly IAsistenciaPymeDbContext _context;

    public CrearCriterioHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CrearCriterioCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Texto))
        {
            throw new InvalidOperationException("El texto del criterio es obligatorio.");
        }

        var categoria = await _context.CategoriasEvaluacion
            .FirstOrDefaultAsync(c => c.IdCategoriaEvaluacion == request.IdCategoriaEvaluacion, cancellationToken);

        if (categoria is null)
        {
            throw new InvalidOperationException("No se encontró la categoría especificada.");
        }

        var criterio = new CriterioEvaluacion
        {
            IdCategoriaEvaluacion = request.IdCategoriaEvaluacion,
            Texto = request.Texto.Trim(),
            Descripcion = request.Descripcion?.Trim(),
            Orden = request.Orden > 0 ? request.Orden : (await _context.CriteriosEvaluacion.CountAsync(c => c.IdCategoriaEvaluacion == request.IdCategoriaEvaluacion, cancellationToken)) + 1,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        await _context.CriteriosEvaluacion.AddAsync(criterio, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return criterio.IdCriterioEvaluacion;
    }
}
