using AsistenciaPyme.Application.Features.Vacaciones.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Vacaciones.Queries;

public class ObtenerVacacionesPorCodigoEmpleadoQuery
    : IRequest<List<VacacionDto>?>
{
    public string CodigoEmpleado { get; set; } = string.Empty;
}