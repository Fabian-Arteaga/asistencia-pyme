using AsistenciaPyme.Application.Features.Planillas.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Planillas.Commands;

public class RecalcularPlanillaCommand : IRequest<PlanillaDto?>
{
    public int IdPlanilla { get; set; }
    public int IdAdministrador { get; set; }
}
