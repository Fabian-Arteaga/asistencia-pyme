using System;
using System.Collections.Generic;
using System.Text;

namespace AsistenciaPyme.Domain.Entities
{
    public class DeduccionPlanilla
    {
        public int IdDeduccionPlanilla { get; set; }

        public int IdPlanilla { get; set; }

        public int IdTipoDeduccion { get; set; }

        public decimal ValorAplicado { get; set; }

        public decimal MontoCalculado { get; set; }

        public string? Observacion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public Planilla Planilla { get; set; } = null!;

        public TipoDeduccion TipoDeduccion { get; set; } = null!;
    }
}
