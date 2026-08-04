using AsistenciaPyme.Application.Features.Planillas.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Planillas.Commands;

public class GenerarPlanillaCommand : IRequest<PlanillaDto?>
{
    public string CodigoEmpleado { get; set; } =
        string.Empty;

    public int IdAdministrador { get; set; }

    public DateOnly FechaInicioPeriodo { get; set; }

    public DateOnly FechaFinPeriodo { get; set; }

    public decimal IngresosAdicionales { get; set; }

    public List<DeduccionAplicarDto> Deducciones { get; set; } =
        new List<DeduccionAplicarDto>();
}