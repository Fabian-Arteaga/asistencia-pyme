using MediatR;
using AsistenciaPyme.Application.Feautures.HorariosLaborales.DTOs;
namespace AsistenciaPyme.Application.Feautures.HorariosLaborales.Queries;
public class ObtenerHorariosLaboralesQuery : IRequest<List<HorarioLaboralDto>>
{
}
