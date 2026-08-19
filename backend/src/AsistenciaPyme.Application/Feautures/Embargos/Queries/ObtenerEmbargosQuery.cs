using AsistenciaPyme.Application.Feautures.Embargos.DTOs;
using MediatR;
namespace AsistenciaPyme.Application.Feautures.Embargos.Queries;
public class ObtenerEmbargosQuery : IRequest<List<EmbargoDto>>
{
}
