using AsistenciaPyme.Application.Features.Vacaciones.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Vacaciones.Commands;

public class CrearVacacionCommand : IRequest<VacacionDto?>
{
    public string CodigoEmpleado { get; set; } = string.Empty;

    public int IdAdministrador { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public string? Motivo { get; set; }

    public string? Observacion { get; set; }
}