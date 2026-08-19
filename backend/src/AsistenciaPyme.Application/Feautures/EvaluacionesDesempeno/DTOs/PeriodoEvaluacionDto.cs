using AsistenciaPyme.Domain.Enums;
using System;

namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.DTOs;

public class PeriodoEvaluacionDto
{
    public int IdPeriodoEvaluacion { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public EstadoPeriodoEvaluacion Estado { get; set; }
    public string EstadoTexto => Estado switch
    {
        EstadoPeriodoEvaluacion.Pendiente => "Pendiente",
        EstadoPeriodoEvaluacion.Activo => "Activo",
        EstadoPeriodoEvaluacion.Finalizado => "Finalizado",
        _ => "Desconocido"
    };
    public DateTime FechaCreacion { get; set; }
    public int TotalEvaluaciones { get; set; }
    public int EvaluacionesCompletadas { get; set; }
    public int EvaluacionesPendientes { get; set; }
}
