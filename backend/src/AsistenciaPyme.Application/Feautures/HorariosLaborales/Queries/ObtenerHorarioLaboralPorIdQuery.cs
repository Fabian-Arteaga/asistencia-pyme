using MediatR;
using AsistenciaPyme.Application.Feautures.HorariosLaborales.DTOs;
namespace AsistenciaPyme.Application.Feautures.HorariosLaborales.Queries;
public class ObtenerHorarioLaboralPorIdQuery : IRequest<HorarioLaboralDto?>
{
    public int IdHorarioLaboral { get; set; }
}
