using AsistenciaPyme.Domain.Enums;

namespace AsistenciaPyme.Application.Features.Planillas.DTOs;

public class PlanillaDto
{
    public int IdPlanilla { get; set; }

    // Departmental planilla fields
    public int? IdDepartamento { get; set; }
    public string? NombreDepartamento { get; set; }

    public DateOnly FechaInicioPeriodo { get; set; }

    public DateOnly FechaFinPeriodo { get; set; }

    public int CantidadEmpleados { get; set; }

    public decimal TotalSalarioBase { get; set; }

    public decimal TotalHorasExtras { get; set; }

    public decimal TotalIngresos { get; set; }

    public decimal TotalDeducciones { get; set; }

    public decimal TotalNeto { get; set; }

    public EstadoPlanilla Estado { get; set; }

    public DateTime? FechaGeneracion { get; set; }

    public DateTime? FechaCierre { get; set; }

    public int? CerradoPor { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public List<DetallePlanillaDto> Detalles { get; set; } = new List<DetallePlanillaDto>();
}