using System;

namespace AsistenciaPyme.Domain.Entities
{
    public class DetalleConceptoPlanilla
    {
        public int IdDetalleConceptoPlanilla { get; set; }

        public int IdDetallePlanilla { get; set; }

        public int IdConceptoPlanilla { get; set; }

        public decimal Monto { get; set; }

        public string? Descripcion { get; set; }

        public DetallePlanilla DetallePlanilla { get; set; } = null!;

        public ConceptoPlanilla ConceptoPlanilla { get; set; } = null!;
    }
}
