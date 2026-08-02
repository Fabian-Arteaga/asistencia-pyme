using AsistenciaPyme.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsistenciaPyme.Domain.Entities
{
    public class Planilla
    {
        public int IdPlanilla { get; set; }

        public int IdEmpleado { get; set; }

        public int IdAdministrador { get; set; }

        public DateOnly FechaInicioPeriodo { get; set; }

        public DateOnly FechaFinPeriodo { get; set; }

        public decimal SalarioBasePeriodo { get; set; }

        public decimal IngresosAdicionales { get; set; }

        public decimal TotalDeducciones { get; set; }

        public decimal SalarioNeto { get; set; }

        public EstadoPlanilla Estado { get; set; } = EstadoPlanilla.Borrador;

        public DateTime? FechaGeneracion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaActualizacion { get; set; }

        public Empleado Empleado { get; set; } = null!;

        public Administrador Administrador { get; set; } = null!;

        public ICollection<DeduccionPlanilla> Deducciones { get; set; }
            = new List<DeduccionPlanilla>();
    }
}
