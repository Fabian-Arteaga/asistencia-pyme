using AsistenciaPyme.Application.Feautures.Embargos.DTOs;
using MediatR;
namespace AsistenciaPyme.Application.Feautures.Embargos.Queries;
public class ObtenerEmbargoPorIdQuery : IRequest<EmbargoDto?>
{
    public int IdEmbargo { get; set; }
}
