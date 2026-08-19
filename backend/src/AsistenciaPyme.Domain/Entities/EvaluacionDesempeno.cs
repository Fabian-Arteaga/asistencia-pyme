using AsistenciaPyme.Domain.Enums;
using System;
using System.Collections.Generic;

namespace AsistenciaPyme.Domain.Entities;

public class EvaluacionDesempeno
{
    public int IdEvaluacionDesempeno { get; set; }

    public int IdPeriodoEvaluacion { get; set; }

    public int IdEmpleadoEvaluado { get; set; }

    public int IdEvaluador { get; set; }

    public TipoEvaluador TipoEvaluador { get; set; }

    public EstadoEvaluacion Estado { get; set; } = EstadoEvaluacion.Pendiente;

    public decimal? PuntajeFinal { get; set; }

    public string? ObservacionesGenerales { get; set; }

    public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaCompletada { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public PeriodoEvaluacion Periodo { get; set; } = null!;

    public Empleado EmpleadoEvaluado { get; set; } = null!;

    public Empleado Evaluador { get; set; } = null!;

    public ICollection<DetalleEvaluacionDesempeno> Detalles { get; set; }
        = new List<DetalleEvaluacionDesempeno>();
}
