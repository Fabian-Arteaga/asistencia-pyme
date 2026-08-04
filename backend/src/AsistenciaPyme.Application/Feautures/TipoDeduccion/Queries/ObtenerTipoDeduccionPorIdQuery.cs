using AsistenciaPyme.Application.Features.TiposDeduccion.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.TiposDeduccion.Queries;

public class ObtenerTipoDeduccionPorIdQuery
    : IRequest<TipoDeduccionDto?>
{
    public int IdTipoDeduccion { get; set; }
}