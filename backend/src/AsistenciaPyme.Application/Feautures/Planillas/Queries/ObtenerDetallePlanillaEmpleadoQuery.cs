using AsistenciaPyme.Application.Features.Planillas.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Planillas.Queries;

public class ObtenerDetallePlanillaEmpleadoQuery : IRequest<DetallePlanillaEmpleadoDto?>
{
    public int IdPlanilla { get; set; }
    public int IdEmpleado { get; set; }
}
