using AsistenciaPyme.Domain.Enums;
using System;
using System.Collections.Generic;

namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.DTOs;

public class EvaluacionDesempenoDto
{
    public int IdEvaluacionDesempeno { get; set; }
    public int IdPeriodoEvaluacion { get; set; }
    public string NombrePeriodo { get; set; } = string.Empty;

    public int IdEmpleadoEvaluado { get; set; }
    public string CodigoEmpleadoEvaluado { get; set; } = string.Empty;
    public string NombreCompletoEvaluado { get; set; } = string.Empty;
    public string CargoEvaluado { get; set; } = string.Empty;
    public string DepartamentoEvaluado { get; set; } = string.Empty;

    public int IdEvaluador { get; set; }
    public string CodigoEvaluador { get; set; } = string.Empty;
    public string NombreCompletoEvaluador { get; set; } = string.Empty;
    public string CargoEvaluador { get; set; } = string.Empty;

    public TipoEvaluador TipoEvaluador { get; set; }
    public string TipoEvaluadorTexto => TipoEvaluador switch
    {
        TipoEvaluador.Autoevaluacion => "Autoevaluación",
        TipoEvaluador.JefeDirecto => "Jefe directo",
        TipoEvaluador.Subordinado => "Subordinado",
        _ => "Desconocido"
    };

    public EstadoEvaluacion Estado { get; set; }
    public string EstadoTexto => Estado switch
    {
        EstadoEvaluacion.Pendiente => "Pendiente",
        EstadoEvaluacion.EnProceso => "En proceso",
        EstadoEvaluacion.Completada => "Completada",
        _ => "Desconocido"
    };

    public decimal? PuntajeFinal { get; set; }
    public string Clasificacion => ObtenerClasificacion(PuntajeFinal);
    public string? ObservacionesGenerales { get; set; }
    public DateTime FechaAsignacion { get; set; }
    public DateTime? FechaCompletada { get; set; }

    public List<DetalleEvaluacionDto> Detalles { get; set; } = new();

    public static string ObtenerClasificacion(decimal? puntaje)
    {
        if (!puntaje.HasValue) return "-";
        decimal p = puntaje.Value;
        if (p >= 90m) return "Excelente";
        if (p >= 80m) return "Muy bueno";
        if (p >= 70m) return "Bueno";
        if (p >= 60m) return "Aceptable";
        return "Necesita mejorar";
    }
}
