using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.Indemnizacion.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Feautures.Indemnizacion.Queries;

public class ObtenerIndemnizacionEmpleadoHandler : IRequestHandler<ObtenerIndemnizacionEmpleadoQuery, IndemnizacionEmpleadoDto>
{
    private readonly ICalculadorIndemnizacion _calculadorIndemnizacion;

    public ObtenerIndemnizacionEmpleadoHandler(ICalculadorIndemnizacion calculadorIndemnizacion)
    {
        _calculadorIndemnizacion = calculadorIndemnizacion;
    }

    public async Task<IndemnizacionEmpleadoDto> Handle(ObtenerIndemnizacionEmpleadoQuery request, CancellationToken cancellationToken)
    {
        return await _calculadorIndemnizacion.CalcularAsync(request.IdEmpleado, cancellationToken);
    }
}
