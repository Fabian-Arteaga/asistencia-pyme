using System.Collections.Generic;

namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.DTOs;

public class CategoriaEvaluacionDto
{
    public int IdCategoriaEvaluacion { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Ponderacion { get; set; }
    public int Orden { get; set; }
    public bool Activo { get; set; }
    public List<CriterioEvaluacionDto> Criterios { get; set; } = new();
}
