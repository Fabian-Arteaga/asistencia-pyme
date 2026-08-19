namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.DTOs;

public class CriterioEvaluacionDto
{
    public int IdCriterioEvaluacion { get; set; }
    public int IdCategoriaEvaluacion { get; set; }
    public string Texto { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int Orden { get; set; }
    public bool Activo { get; set; }
}
