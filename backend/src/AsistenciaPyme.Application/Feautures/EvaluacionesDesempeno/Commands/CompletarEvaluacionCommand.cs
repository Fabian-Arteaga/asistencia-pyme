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

public class CompletarEvaluacionCommand : IRequest<decimal>
{
    public int IdEvaluacionDesempeno { get; set; }
    public string? ObservacionesGenerales { get; set; }
    public List<RespuestaCriterioDto> Respuestas { get; set; } = new();
}

public class CompletarEvaluacionHandler : IRequestHandler<CompletarEvaluacionCommand, decimal>
{
    private readonly IAsistenciaPymeDbContext _context;

    public CompletarEvaluacionHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> Handle(CompletarEvaluacionCommand request, CancellationToken cancellationToken)
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
            throw new InvalidOperationException("Esta evaluación ya ha sido completada previamente y no puede modificarse.");
        }

        if (evaluacion.Periodo.Estado == EstadoPeriodoEvaluacion.Finalizado)
        {
            throw new InvalidOperationException("El período de evaluación ya ha finalizado.");
        }

        // Obtener todas las categorías y criterios activos
        var categoriasActivas = await _context.CategoriasEvaluacion
            .Include(c => c.Criterios)
            .Where(c => c.Activo)
            .ToListAsync(cancellationToken);

        if (!categoriasActivas.Any())
        {
            throw new InvalidOperationException("No existen categorías activas para realizar la evaluación.");
        }

        // Validar que las ponderaciones sumen 100%
        decimal sumaPonderaciones = categoriasActivas.Sum(c => c.Ponderacion);
        if (Math.Round(sumaPonderaciones, 2) != 100.00m)
        {
            throw new InvalidOperationException($"La configuración de categorías no es válida. La suma de ponderaciones debe ser 100%, actualmente es {sumaPonderaciones}%.");
        }

        var todosCriteriosActivos = categoriasActivas
            .SelectMany(c => c.Criterios.Where(cr => cr.Activo))
            .ToList();

        if (!todosCriteriosActivos.Any())
        {
            throw new InvalidOperationException("No existen criterios de evaluación activos para completar la evaluación.");
        }

        // Combinar respuestas existentes en DB con las recibidas en el request
        var respuestasDict = new Dictionary<int, (int Puntuacion, string? Comentario)>();

        // Cargar respuestas ya guardadas
        foreach (var det in evaluacion.Detalles)
        {
            respuestasDict[det.IdCriterioEvaluacion] = (det.Puntuacion, det.Comentario);
        }

        // Sobrescribir o añadir con las nuevas respuestas del request
        foreach (var r in request.Respuestas)
        {
            if (r.Puntuacion < 1 || r.Puntuacion > 5)
            {
                throw new InvalidOperationException($"La puntuación para cada criterio debe estar entre 1 y 5. Puntuación recibida: {r.Puntuacion}.");
            }
            respuestasDict[r.IdCriterioEvaluacion] = (r.Puntuacion, r.Comentario);
        }

        // Validar que TODOS los criterios activos hayan sido respondidos
        foreach (var criterio in todosCriteriosActivos)
        {
            if (!respuestasDict.TryGetValue(criterio.IdCriterioEvaluacion, out var resp) || resp.Puntuacion < 1 || resp.Puntuacion > 5)
            {
                throw new InvalidOperationException($"No se puede completar la evaluación porque faltan preguntas por responder (ej: \"{criterio.Texto}\").");
            }
        }

        // Realizar el cálculo del resultado ponderado (escala 0 - 100)
        decimal puntajeFinal = 0m;

        foreach (var categoria in categoriasActivas)
        {
            var criteriosDeCategoria = categoria.Criterios.Where(c => c.Activo).ToList();
            if (!criteriosDeCategoria.Any())
            {
                continue;
            }

            decimal sumaRespuestas = 0m;
            foreach (var crit in criteriosDeCategoria)
            {
                sumaRespuestas += respuestasDict[crit.IdCriterioEvaluacion].Puntuacion;
            }

            decimal promedioCategoria = sumaRespuestas / criteriosDeCategoria.Count;
            decimal puntajePonderadoCategoria = (promedioCategoria / 5.0m) * categoria.Ponderacion;
            puntajeFinal += puntajePonderadoCategoria;
        }

        puntajeFinal = Math.Round(puntajeFinal, 2);

        // Guardar detalles y congelar snapshots históricos
        foreach (var crit in todosCriteriosActivos)
        {
            var (puntuacion, comentario) = respuestasDict[crit.IdCriterioEvaluacion];
            var detalle = evaluacion.Detalles.FirstOrDefault(d => d.IdCriterioEvaluacion == crit.IdCriterioEvaluacion);

            if (detalle is null)
            {
                detalle = new DetalleEvaluacionDesempeno
                {
                    IdEvaluacionDesempeno = evaluacion.IdEvaluacionDesempeno,
                    IdCriterioEvaluacion = crit.IdCriterioEvaluacion,
                    Puntuacion = puntuacion,
                    Comentario = comentario?.Trim(),
                    PonderacionCategoriaHistorica = crit.Categoria.Ponderacion,
                    NombreCategoriaHistorica = crit.Categoria.Nombre,
                    TextoCriterioHistorico = crit.Texto
                };
                evaluacion.Detalles.Add(detalle);
            }
            else
            {
                detalle.Puntuacion = puntuacion;
                detalle.Comentario = comentario?.Trim();
                detalle.PonderacionCategoriaHistorica = crit.Categoria.Ponderacion;
                detalle.NombreCategoriaHistorica = crit.Categoria.Nombre;
                detalle.TextoCriterioHistorico = crit.Texto;
            }
        }

        evaluacion.PuntajeFinal = puntajeFinal;
        evaluacion.ObservacionesGenerales = request.ObservacionesGenerales?.Trim() ?? evaluacion.ObservacionesGenerales;
        evaluacion.Estado = EstadoEvaluacion.Completada;
        evaluacion.FechaCompletada = DateTime.UtcNow;
        evaluacion.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return puntajeFinal;
    }
}
