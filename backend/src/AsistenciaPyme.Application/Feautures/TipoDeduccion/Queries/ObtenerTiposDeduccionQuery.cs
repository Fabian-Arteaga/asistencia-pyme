using AsistenciaPyme.Application.Features.TiposDeduccion.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.TiposDeduccion.Queries;

public class ObtenerTiposDeduccionQuery
    : IRequest<List<TipoDeduccionDto>>
{
}