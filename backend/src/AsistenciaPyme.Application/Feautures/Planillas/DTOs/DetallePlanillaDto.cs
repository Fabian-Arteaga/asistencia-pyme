using System;

namespace AsistenciaPyme.Application.Features.Planillas.DTOs
{
    public class DetallePlanillaDto
    {
        public int IdDetallePlanilla { get; set; }

        public int IdEmpleado { get; set; }

        public string CodigoEmpleado { get; set; } = string.Empty;

        public string NombreEmpleado { get; set; } = string.Empty;

        public string? NumeroINSS { get; set; }

        public string Cargo { get; set; } = string.Empty;

        public string Departamento { get; set; } = string.Empty;

        public decimal SalarioBase { get; set; }

        public int DiasLaborados { get; set; }

        public int MinutosLaborados { get; set; }

        public int CantidadTardanzas { get; set; }

        public int MinutosTardanza { get; set; }

        public decimal DescuentoTardanza { get; set; }

        public int MinutosExtrasDetectados { get; set; }

        public int MinutosExtrasAprobados { get; set; }

        public decimal MontoHorasExtras { get; set; }

        public decimal VacacionesAcumuladasPeriodo { get; set; }

        public decimal SaldoVacaciones { get; set; }

        public decimal TotalIngresos { get; set; }

        public decimal TotalDeducciones { get; set; }

        public decimal SalarioNeto { get; set; }

        public decimal? IndemnizacionProyectada { get; set; }
    }
}
