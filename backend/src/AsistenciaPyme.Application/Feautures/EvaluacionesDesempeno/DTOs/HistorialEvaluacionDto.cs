using System;
using System.Collections.Generic;

namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.DTOs;

public class PeriodoHistorialDto
{
    public int IdPeriodoEvaluacion { get; set; }
    public string NombrePeriodo { get; set; } = string.Empty;
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public decimal? PuntajeConsolidado { get; set; }
    public string Clasificacion => EvaluacionDesempenoDto.ObtenerClasificacion(PuntajeConsolidado);
    public int EvaluacionesRealizadas { get; set; }
}

public class HistorialEvaluacionEmpleadoDto
{
    public int IdEmpleado { get; set; }
    public string CodigoEmpleado { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public List<PeriodoHistorialDto> Periodos { get; set; } = new();
}
