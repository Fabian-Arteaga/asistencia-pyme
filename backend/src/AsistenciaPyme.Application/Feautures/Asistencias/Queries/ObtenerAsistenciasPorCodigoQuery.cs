using AsistenciaPyme.Application.Features.Asistencias.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Asistencias.Queries;

public class ObtenerAsistenciasPorCodigoQuery
    : IRequest<List<AsistenciaDto>?>
{
    public string CodigoEmpleado { get; set; } = string.Empty;
}