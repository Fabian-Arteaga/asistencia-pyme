using AsistenciaPyme.Application.Features.Vacaciones.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Vacaciones.Commands;

public class CancelarVacacionCommand
    : IRequest<VacacionDto?>
{
    public int IdVacacion { get; set; }
}