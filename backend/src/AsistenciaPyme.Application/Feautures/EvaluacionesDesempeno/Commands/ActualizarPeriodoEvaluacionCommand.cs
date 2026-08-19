using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.Commands;

public class ActualizarPeriodoEvaluacionCommand : IRequest<bool>
{
    public int IdPeriodoEvaluacion { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
}

public class ActualizarPeriodoEvaluacionHandler : IRequestHandler<ActualizarPeriodoEvaluacionCommand, bool>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ActualizarPeriodoEvaluacionHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ActualizarPeriodoEvaluacionCommand request, CancellationToken cancellationToken)
    {
        var periodo = await _context.PeriodosEvaluacion
            .FirstOrDefaultAsync(p => p.IdPeriodoEvaluacion == request.IdPeriodoEvaluacion, cancellationToken);

        if (periodo is null)
        {
            throw new InvalidOperationException("No se encontró el período de evaluación.");
        }

        if (periodo.Estado == EstadoPeriodoEvaluacion.Finalizado)
        {
            throw new InvalidOperationException("No se puede modificar un período finalizado.");
        }

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
            .AnyAsync(p => p.IdPeriodoEvaluacion != request.IdPeriodoEvaluacion && p.Nombre.ToLower() == nombre.ToLower(), cancellationToken);

        if (existe)
        {
            throw new InvalidOperationException("Ya existe otro período con ese nombre.");
        }

        periodo.Nombre = nombre;
        periodo.FechaInicio = request.FechaInicio;
        periodo.FechaFin = request.FechaFin;
        periodo.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
