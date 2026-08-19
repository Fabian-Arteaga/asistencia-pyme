using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Domain.Entities;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.Commands;

public class AsignarEvaluacionCommand : IRequest<int>
{
    public int IdPeriodoEvaluacion { get; set; }
    public int IdEmpleadoEvaluado { get; set; }
    public int IdEvaluador { get; set; }
    public TipoEvaluador TipoEvaluador { get; set; }
}

public class AsignarEvaluacionHandler : IRequestHandler<AsignarEvaluacionCommand, int>
{
    private readonly IAsistenciaPymeDbContext _context;

    public AsignarEvaluacionHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(AsignarEvaluacionCommand request, CancellationToken cancellationToken)
    {
        var periodo = await _context.PeriodosEvaluacion
            .FirstOrDefaultAsync(p => p.IdPeriodoEvaluacion == request.IdPeriodoEvaluacion, cancellationToken);

        if (periodo is null)
        {
            throw new InvalidOperationException("No se encontró el período de evaluación especificado.");
        }

        if (periodo.Estado == EstadoPeriodoEvaluacion.Finalizado)
        {
            throw new InvalidOperationException("No se pueden asignar evaluaciones en un período finalizado.");
        }

        var empleadoEvaluado = await _context.Empleados
            .FirstOrDefaultAsync(e => e.IdEmpleado == request.IdEmpleadoEvaluado, cancellationToken);

        if (empleadoEvaluado is null)
        {
            throw new InvalidOperationException("No se encontró el empleado a evaluar.");
        }

        var evaluador = await _context.Empleados
            .FirstOrDefaultAsync(e => e.IdEmpleado == request.IdEvaluador, cancellationToken);

        if (evaluador is null)
        {
            throw new InvalidOperationException("No se encontró el empleado evaluador.");
        }

        // Reglas de negocio 360°
        switch (request.TipoEvaluador)
        {
            case TipoEvaluador.Autoevaluacion:
                if (request.IdEvaluador != request.IdEmpleadoEvaluado)
                {
                    throw new InvalidOperationException("En una autoevaluación, el evaluador debe ser el mismo empleado evaluado.");
                }
                break;

            case TipoEvaluador.JefeDirecto:
                if (request.IdEvaluador == request.IdEmpleadoEvaluado)
                {
                    throw new InvalidOperationException("El empleado no puede evaluarse a sí mismo como Jefe Directo.");
                }
                if (!empleadoEvaluado.IdJefeDirecto.HasValue || empleadoEvaluado.IdJefeDirecto.Value != request.IdEvaluador)
                {
                    throw new InvalidOperationException("El evaluador seleccionado no está registrado como el jefe directo de este colaborador.");
                }
                break;

            case TipoEvaluador.Subordinado:
                if (request.IdEvaluador == request.IdEmpleadoEvaluado)
                {
                    throw new InvalidOperationException("El empleado no puede evaluarse a sí mismo como Subordinado.");
                }
                if (!evaluador.IdJefeDirecto.HasValue || evaluador.IdJefeDirecto.Value != request.IdEmpleadoEvaluado)
                {
                    throw new InvalidOperationException("El evaluador seleccionado no es un subordinado registrado de este colaborador.");
                }
                break;

            default:
                throw new InvalidOperationException("Tipo de evaluador no válido.");
        }

        // Validar que no exista ya una evaluación del mismo tipo asignada a este evaluador para el mismo empleado y período
        bool yaExiste = await _context.EvaluacionesDesempeno
            .AnyAsync(e => e.IdPeriodoEvaluacion == request.IdPeriodoEvaluacion &&
                           e.IdEmpleadoEvaluado == request.IdEmpleadoEvaluado &&
                           e.IdEvaluador == request.IdEvaluador &&
                           e.TipoEvaluador == request.TipoEvaluador, cancellationToken);

        if (yaExiste)
        {
            throw new InvalidOperationException("Ya existe una evaluación asignada con estos mismos parámetros para este período.");
        }

        var evaluacion = new EvaluacionDesempeno
        {
            IdPeriodoEvaluacion = request.IdPeriodoEvaluacion,
            IdEmpleadoEvaluado = request.IdEmpleadoEvaluado,
            IdEvaluador = request.IdEvaluador,
            TipoEvaluador = request.TipoEvaluador,
            Estado = EstadoEvaluacion.Pendiente,
            FechaAsignacion = DateTime.UtcNow
        };

        await _context.EvaluacionesDesempeno.AddAsync(evaluacion, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return evaluacion.IdEvaluacionDesempeno;
    }
}
