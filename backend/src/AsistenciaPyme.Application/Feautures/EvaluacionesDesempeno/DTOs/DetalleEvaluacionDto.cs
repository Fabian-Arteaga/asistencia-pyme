namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.DTOs;

public class DetalleEvaluacionDto
{
    public int IdDetalleEvaluacionDesempeno { get; set; }
    public int IdEvaluacionDesempeno { get; set; }
    public int IdCriterioEvaluacion { get; set; }
    public int IdCategoriaEvaluacion { get; set; }
    public string NombreCategoria { get; set; } = string.Empty;
    public string TextoCriterio { get; set; } = string.Empty;
    public decimal PonderacionCategoria { get; set; }
    public int Puntuacion { get; set; }
    public string? Comentario { get; set; }
}
