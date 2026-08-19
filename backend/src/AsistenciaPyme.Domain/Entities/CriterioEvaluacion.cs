using System;

namespace AsistenciaPyme.Domain.Entities;

public class CriterioEvaluacion
{
    public int IdCriterioEvaluacion { get; set; }

    public int IdCategoriaEvaluacion { get; set; }

    public string Texto { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public int Orden { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaActualizacion { get; set; }

    public CategoriaEvaluacion Categoria { get; set; } = null!;
}
