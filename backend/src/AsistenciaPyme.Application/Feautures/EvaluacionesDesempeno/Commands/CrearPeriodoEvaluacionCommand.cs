using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Domain.Entities;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.Commands;

public class CrearPeriodoEvaluacionCommand : IRequest<int>
{
    public string Nombre { get; set; } = string.Empty;
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
}

public class CrearPeriodoEvaluacionHandler : IRequestHandler<CrearPeriodoEvaluacionCommand, int>
{
    private readonly IAsistenciaPymeDbContext _context;

    public CrearPeriodoEvaluacionHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CrearPeriodoEvaluacionCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            throw new InvalidOperationException("El nombre del período es obligatorio.");
        }

        if (request.FechaFin <= request.FechaInicio)
        {
            throw new InvalidOperationException("La fecha final debe ser posterior a la fecha inicial.");
        }

        string nombre = request.Nombre.Trim();

        bool existe = await _context.PeriodosEvaluacion
            .AnyAsync(p => p.Nombre.ToLower() == nombre.ToLower(), cancellationToken);

        if (existe)
        {
            throw new InvalidOperationException("Ya existe un período con ese nombre.");
        }

        var periodo = new PeriodoEvaluacion
        {
            Nombre = nombre,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            Estado = EstadoPeriodoEvaluacion.Pendiente,
            FechaCreacion = DateTime.UtcNow
        };

        await _context.PeriodosEvaluacion.AddAsync(periodo, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return periodo.IdPeriodoEvaluacion;
    }
}
