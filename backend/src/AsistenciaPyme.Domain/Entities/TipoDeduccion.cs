using AsistenciaPyme.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsistenciaPyme.Domain.Entities
{
    public class TipoDeduccion
    {
        public int IdTipoDeduccion { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public TipoCalculoDeduccion TipoCalculo { get; set; }

        public decimal? ValorPredeterminado { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaActualizacion { get; set; }

        public ICollection<DeduccionPlanilla> DeduccionesPlanilla { get; set; }
            = new List<DeduccionPlanilla>();
    }
}
