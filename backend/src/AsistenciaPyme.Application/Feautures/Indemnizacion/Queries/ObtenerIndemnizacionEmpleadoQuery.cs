using AsistenciaPyme.Application.Feautures.Indemnizacion.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Feautures.Indemnizacion.Queries;

public class ObtenerIndemnizacionEmpleadoQuery : IRequest<IndemnizacionEmpleadoDto>
{
    public int IdEmpleado { get; set; }
}
