using System;
using System.Collections.Generic;

namespace AsistenciaPyme.Domain.Entities;

public class CategoriaEvaluacion
{
    public int IdCategoriaEvaluacion { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public decimal Ponderacion { get; set; }

    public int Orden { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaActualizacion { get; set; }

    public ICollection<CriterioEvaluacion> Criterios { get; set; }
        = new List<CriterioEvaluacion>();
}
