using AsistenciaPyme.Application.Features.Vacaciones.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace AsistenciaPyme.Application.Features.Vacaciones.Commands;

public class ActualizarVacacionCommand
    : IRequest<VacacionDto?>
{
    [JsonIgnore]
    public int IdVacacion { get; set; }

    public string CodigoEmpleado { get; set; } = string.Empty;

    public int IdAdministrador { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public string? Motivo { get; set; }

    public string? Observacion { get; set; }
}