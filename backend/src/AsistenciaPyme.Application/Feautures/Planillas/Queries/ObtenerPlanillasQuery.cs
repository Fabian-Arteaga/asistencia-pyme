using AsistenciaPyme.Application.Features.Planillas.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Planillas.Queries;

public class ObtenerPlanillasQuery
    : IRequest<List<PlanillaDto>>
{
}