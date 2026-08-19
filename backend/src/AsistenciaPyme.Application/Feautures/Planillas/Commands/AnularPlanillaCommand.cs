using AsistenciaPyme.Application.Features.Planillas.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Planillas.Commands;

public class AnularPlanillaCommand : IRequest<PlanillaDto?>
{
    public int IdPlanilla { get; set; }
    public int IdAdministrador { get; set; }
    public string? Motivo { get; set; }
}
