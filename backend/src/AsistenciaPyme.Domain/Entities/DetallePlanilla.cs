using System;
using System.Collections.Generic;

namespace AsistenciaPyme.Domain.Entities
{
    public class DetallePlanilla
    {
        public int IdDetallePlanilla { get; set; }

        public int IdPlanilla { get; set; }

        public int IdEmpleado { get; set; }

        // Snapshot empleado
        public string CodigoEmpleado { get; set; } = string.Empty;

        public string NombreEmpleado { get; set; } = string.Empty;

        public string? NumeroINSS { get; set; }

        public string Cargo { get; set; } = string.Empty;

        public string Departamento { get; set; } = string.Empty;

        // Economico snapshot
        public decimal SalarioBase { get; set; }

        public int DiasLaborados { get; set; }

        public int MinutosLaborados { get; set; }

        // Asistencias / tardanzas
        public int CantidadTardanzas { get; set; }

        public int MinutosTardanza { get; set; }

        public decimal DescuentoTardanza { get; set; }

        // Horas extra
        public int MinutosExtrasDetectados { get; set; }

        public int MinutosExtrasAprobados { get; set; }

        public decimal MontoHorasExtras { get; set; }

        // Vacaciones
        public decimal VacacionesAcumuladasPeriodo { get; set; }

        public decimal SaldoVacaciones { get; set; }

        // Totales
        public decimal TotalIngresos { get; set; }

        public decimal TotalDeducciones { get; set; }

        public decimal SalarioNeto { get; set; }

        // Informativo
        public decimal? IndemnizacionProyectada { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public Planilla Planilla { get; set; } = null!;

        public Empleado Empleado { get; set; } = null!;

        public ICollection<DetalleConceptoPlanilla> Conceptos { get; set; } = new List<DetalleConceptoPlanilla>();
    }
}
