using System;

namespace AsistenciaPyme.Domain.Entities;

public class DetalleEvaluacionDesempeno
{
    public int IdDetalleEvaluacionDesempeno { get; set; }

    public int IdEvaluacionDesempeno { get; set; }

    public int IdCriterioEvaluacion { get; set; }

    /// <summary>
    /// Puntuación del criterio en escala de 1 a 5:
    /// 1 = Muy deficiente, 2 = Deficiente, 3 = Aceptable, 4 = Bueno, 5 = Excelente
    /// </summary>
    public int Puntuacion { get; set; }

    public string? Comentario { get; set; }

    /// <summary>
    /// Snapshot de la ponderación de la categoría al momento de evaluar,
    /// para preservar la inmutabilidad histórica del cálculo.
    /// </summary>
    public decimal PonderacionCategoriaHistorica { get; set; }

    public string NombreCategoriaHistorica { get; set; } = string.Empty;

    public string TextoCriterioHistorico { get; set; } = string.Empty;

    public EvaluacionDesempeno Evaluacion { get; set; } = null!;

    public CriterioEvaluacion Criterio { get; set; } = null!;
}
