using AsistenciaPyme.Application.Features.Planillas.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Planillas.Commands;

public class EnviarPlanillaRevisionCommand : IRequest<PlanillaDto?>
{
    public int IdPlanilla { get; set; }
    public int IdAdministrador { get; set; }
}
