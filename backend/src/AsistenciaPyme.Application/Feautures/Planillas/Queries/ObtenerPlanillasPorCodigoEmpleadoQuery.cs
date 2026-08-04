using AsistenciaPyme.Application.Features.Planillas.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Planillas.Queries;

public class ObtenerPlanillasPorCodigoEmpleadoQuery
    : IRequest<List<PlanillaDto>?>
{
    public string CodigoEmpleado { get; set; } =
        string.Empty;
}