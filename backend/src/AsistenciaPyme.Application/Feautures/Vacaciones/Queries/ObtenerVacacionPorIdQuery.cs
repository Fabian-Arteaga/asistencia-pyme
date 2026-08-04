using AsistenciaPyme.Application.Features.Vacaciones.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Vacaciones.Queries;

public class ObtenerVacacionPorIdQuery
    : IRequest<VacacionDto?>
{
    public int IdVacacion { get; set; }
}