using AsistenciaPyme.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsistenciaPyme.Domain.Entities
{
    public class Planilla
    {
        public int IdPlanilla { get; set; }
        // Legacy: keep for backward compatibility (old planillas per empleado)
        public int? IdEmpleado { get; set; }

        // New: planilla por departamento
        public int? IdDepartamento { get; set; }

        public int? IdAdministrador { get; set; }
        public DateOnly FechaInicioPeriodo { get; set; }

        public DateOnly FechaFinPeriodo { get; set; }

        public decimal SalarioBasePeriodo { get; set; }

        public decimal IngresosAdicionales { get; set; }

        public decimal SalarioNeto { get; set; }

        public EstadoPlanilla Estado { get; set; } = EstadoPlanilla.Borrador;

        public DateTime? FechaGeneracion { get; set; }

        // New summary fields
        public int CantidadEmpleados { get; set; }

        public decimal TotalSalarioBase { get; set; }

        public decimal TotalHorasExtras { get; set; }

        public decimal TotalIngresos { get; set; }

        public decimal TotalDeducciones { get; set; }

        public decimal TotalNeto { get; set; }

        public DateTime? FechaCierre { get; set; }

        public int? CerradoPor { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaActualizacion { get; set; }

        public Empleado Empleado { get; set; } = null!;

        public Administrador Administrador { get; set; } = null!;

        public Departamento? Departamento { get; set; }

        public ICollection<DeduccionPlanilla> Deducciones { get; set; }
            = new List<DeduccionPlanilla>();

        // New: detalles por empleado en la planilla departamental
        public ICollection<DetallePlanilla> Detalles { get; set; } = new List<DetallePlanilla>();
    }
}
