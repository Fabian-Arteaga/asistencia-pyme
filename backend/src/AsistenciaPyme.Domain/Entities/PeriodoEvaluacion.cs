using AsistenciaPyme.Domain.Enums;
using System;
using System.Collections.Generic;

namespace AsistenciaPyme.Domain.Entities;

public class PeriodoEvaluacion
{
    public int IdPeriodoEvaluacion { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public EstadoPeriodoEvaluacion Estado { get; set; } = EstadoPeriodoEvaluacion.Pendiente;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaActualizacion { get; set; }

    public ICollection<EvaluacionDesempeno> Evaluaciones { get; set; }
        = new List<EvaluacionDesempeno>();
}
