using AsistenciaPyme.Application.Feautures.HorasExtras.DTOs;
using MediatR;
namespace AsistenciaPyme.Application.Feautures.HorasExtras.Queries;
public class ObtenerHorasExtrasPendientesQuery : IRequest<List<HoraExtraDto>>
{
}
