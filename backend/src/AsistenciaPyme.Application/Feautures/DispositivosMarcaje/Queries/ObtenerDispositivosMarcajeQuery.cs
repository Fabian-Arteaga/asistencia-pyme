using AsistenciaPyme.Application.Feautures.DispositivosMarcaje.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Feautures.DispositivosMarcaje.Queries;

public class ObtenerDispositivosMarcajeQuery : IRequest<List<DispositivoMarcajeDto>>
{
}
