using AsistenciaPyme.Application.Features.Asistencias.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Asistencias.Commands;

public class RegistrarAsistenciaManualCommand
    : IRequest<AsistenciaDto?>
{
    public string CodigoEmpleado { get; set; } = string.Empty;

    public DateTimeOffset HoraEntrada { get; set; }

    public DateTimeOffset? HoraSalida { get; set; }

    public string? Observacion { get; set; }

    public int? IdAdministrador { get; set; }
}