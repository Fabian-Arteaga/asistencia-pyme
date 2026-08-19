using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Domain.Entities;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.Commands;

public class RespuestaCriterioDto
{
    public int IdCriterioEvaluacion { get; set; }
    public int Puntuacion { get; set; }
    public string? Comentario { get; set; }
}

public class GuardarRespuestasEvaluacionCommand : IRequest<bool>
{
    public int IdEvaluacionDesempeno { get; set; }
    public string? ObservacionesGenerales { get; set; }
    public List<RespuestaCriterioDto> Respuestas { get; set; } = new();
}

public class GuardarRespuestasEvaluacionHandler : IRequestHandler<GuardarRespuestasEvaluacionCommand, bool>
{
    private readonly IAsistenciaPymeDbContext _context;

    public GuardarRespuestasEvaluacionHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(GuardarRespuestasEvaluacionCommand request, CancellationToken cancellationToken)
    {
        var evaluacion = await _context.EvaluacionesDesempeno
            .Include(e => e.Detalles)
            .Include(e => e.Periodo)
            .FirstOrDefaultAsync(e => e.IdEvaluacionDesempeno == request.IdEvaluacionDesempeno, cancellationToken);

        if (evaluacion is null)
        {
            throw new InvalidOperationException("No se encontró la evaluación especificada.");
        }

        if (evaluacion.Estado == EstadoEvaluacion.Completada)
        {
            throw new InvalidOperationException("No se puede modificar una evaluación que ya ha sido completada.");
        }

        if (evaluacion.Periodo.Estado == EstadoPeriodoEvaluacion.Finalizado)
        {
            throw new InvalidOperationException("El período de evaluación ya ha finalizado.");
        }

        // Validar rango de puntuaciones si están presentes (1 a 5)
        foreach (var r in request.Respuestas)
        {
            if (r.Puntuacion < 1 || r.Puntuacion > 5)
            {
                throw new InvalidOperationException($"La puntuación para cada criterio debe estar en la escala de 1 a 5. Se recibió: {r.Puntuacion}.");
            }
        }

        var criterios = await _context.CriteriosEvaluacion
            .Include(c => c.Categoria)
            .Where(c => c.Activo && c.Categoria.Activo)
            .ToDictionaryAsync(c => c.IdCriterioEvaluacion, cancellationToken);

        evaluacion.ObservacionesGenerales = request.ObservacionesGenerales?.Trim();
        evaluacion.Estado = EstadoEvaluacion.EnProceso;
        evaluacion.FechaActualizacion = DateTime.UtcNow;

        foreach (var resp in request.Respuestas)
        {
            if (!criterios.TryGetValue(resp.IdCriterioEvaluacion, out var criterio))
            {
                continue;
            }

            var detalle = evaluacion.Detalles
                .FirstOrDefault(d => d.IdCriterioEvaluacion == resp.IdCriterioEvaluacion);

            if (detalle is null)
            {
                detalle = new DetalleEvaluacionDesempeno
                {
                    IdEvaluacionDesempeno = evaluacion.IdEvaluacionDesempeno,
                    IdCriterioEvaluacion = resp.IdCriterioEvaluacion,
                    Puntuacion = resp.Puntuacion,
                    Comentario = resp.Comentario?.Trim(),
                    PonderacionCategoriaHistorica = criterio.Categoria.Ponderacion,
                    NombreCategoriaHistorica = criterio.Categoria.Nombre,
                    TextoCriterioHistorico = criterio.Texto
                };
                evaluacion.Detalles.Add(detalle);
            }
            else
            {
                detalle.Puntuacion = resp.Puntuacion;
                detalle.Comentario = resp.Comentario?.Trim();
                detalle.PonderacionCategoriaHistorica = criterio.Categoria.Ponderacion;
                detalle.NombreCategoriaHistorica = criterio.Categoria.Nombre;
                detalle.TextoCriterioHistorico = criterio.Texto;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
