using AsistenciaPyme.Domain.Enums;

namespace AsistenciaPyme.Application.Features.Planillas.DTOs;

public class PlanillaDto
{
    public int IdPlanilla { get; set; }

    public int IdEmpleado { get; set; }

    public string CodigoEmpleado { get; set; } =
        string.Empty;

    public string NombreEmpleado { get; set; } =
        string.Empty;

    public int IdAdministrador { get; set; }

    public string NombreAdministrador { get; set; } =
        string.Empty;

    public DateOnly FechaInicioPeriodo { get; set; }

    public DateOnly FechaFinPeriodo { get; set; }

    public decimal SalarioBasePeriodo { get; set; }

    public decimal IngresosAdicionales { get; set; }

    public decimal SalarioBruto { get; set; }

    public decimal TotalDeducciones { get; set; }

    public decimal SalarioNeto { get; set; }

    public EstadoPlanilla Estado { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public List<DeduccionPlanillaDto> Deducciones { get; set; } =
        new List<DeduccionPlanillaDto>();
}