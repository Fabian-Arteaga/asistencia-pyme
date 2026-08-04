using AsistenciaPyme.Application.Features.Planillas.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Planillas.Queries;

public class ObtenerPlanillaPorIdQuery
    : IRequest<PlanillaDto?>
{
    public int IdPlanilla { get; set; }
}