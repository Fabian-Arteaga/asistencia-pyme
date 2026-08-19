using System;
using System.Collections.Generic;

namespace AsistenciaPyme.Domain.Entities
{
    public enum TipoConceptoPlanilla
    {
        Ingreso = 1,
        Deduccion = 2,
        Informativo = 3
    }

    public class ConceptoPlanilla
    {
        public int IdConceptoPlanilla { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public TipoConceptoPlanilla Tipo { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<DetalleConceptoPlanilla> Detalles { get; set; } = new List<DetalleConceptoPlanilla>();
    }
}
