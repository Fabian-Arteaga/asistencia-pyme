using AsistenciaPyme.Application.Feautures.Indemnizacion.DTOs;

namespace AsistenciaPyme.Application.Common.Interfaces;

public interface ICalculadorIndemnizacion
{
    Task<IndemnizacionEmpleadoDto> CalcularAsync(int idEmpleado, CancellationToken cancellationToken = default);
}
