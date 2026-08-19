using System;

namespace AsistenciaPyme.Domain.Entities
{
    public enum TipoCalculoEmbargo
    {
        MontoFijo = 1,
        Porcentaje = 2
    }

    public class Embargo
    {
        public int IdEmbargo { get; set; }

        public int IdEmpleado { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public TipoCalculoEmbargo TipoCalculo { get; set; }

        public decimal? Monto { get; set; }

        public decimal? Porcentaje { get; set; }

        public decimal SaldoPendiente { get; set; }

        public bool Activo { get; set; } = true;

        public string? Referencia { get; set; }

        public string? Observacion { get; set; }

        public Empleado Empleado { get; set; } = null!;
    }
}
