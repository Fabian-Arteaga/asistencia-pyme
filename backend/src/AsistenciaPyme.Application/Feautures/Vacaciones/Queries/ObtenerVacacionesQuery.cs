using AsistenciaPyme.Application.Features.Vacaciones.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Vacaciones.Queries;

public class ObtenerVacacionesQuery
    : IRequest<List<VacacionDto>>
{
}