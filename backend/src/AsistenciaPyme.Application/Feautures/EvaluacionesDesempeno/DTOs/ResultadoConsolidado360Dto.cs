using System.Collections.Generic;

namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.DTOs;

public class PuntajeCategoriaDto
{
    public int IdCategoriaEvaluacion { get; set; }
    public string NombreCategoria { get; set; } = string.Empty;
    public decimal Ponderacion { get; set; }
    public decimal PromedioRespuestas { get; set; } // Promedio escala 1-5
    public decimal RendimientoPorcentaje { get; set; } // (Promedio / 5) * 100
    public decimal PuntosObtenidos { get; set; } // (Promedio / 5) * Ponderacion
}

public class Perspectiva360Dto
{
    public string Perspectiva { get; set; } = string.Empty;
    public string TipoEvaluador { get; set; } = string.Empty;
    public int CantidadEvaluadores { get; set; }
    public decimal? PuntajePromedio { get; set; }
    public bool Completada { get; set; }
}

public class ResultadoConsolidado360Dto
{
    public int IdEmpleado { get; set; }
    public string CodigoEmpleado { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;

    public int IdPeriodoEvaluacion { get; set; }
    public string NombrePeriodo { get; set; } = string.Empty;

    public decimal? PromedioAutoevaluacion { get; set; }
    public decimal? PromedioJefeDirecto { get; set; }
    public decimal? PromedioSubordinados { get; set; }

    public decimal? PuntajeConsolidado { get; set; }
    public string Clasificacion => EvaluacionDesempenoDto.ObtenerClasificacion(PuntajeConsolidado);

    public List<Perspectiva360Dto> Perspectivas { get; set; } = new();
    public List<PuntajeCategoriaDto> CategoriasConsolidadas { get; set; } = new();
    public List<EvaluacionDesempenoDto> EvaluacionesIndividuales { get; set; } = new();
}
