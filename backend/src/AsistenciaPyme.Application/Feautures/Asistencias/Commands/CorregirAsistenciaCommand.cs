using AsistenciaPyme.Application.Features.Asistencias.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace AsistenciaPyme.Application.Features.Asistencias.Commands;

public class CorregirAsistenciaCommand
    : IRequest<AsistenciaDto?>
{
    [JsonIgnore]
    public int IdAsistencia { get; set; }

    public DateTimeOffset HoraEntrada { get; set; }

    public DateTimeOffset? HoraSalida { get; set; }

    public string? Observacion { get; set; }

    public string MotivoCorreccion { get; set; } = string.Empty;

    public int? IdAdministrador { get; set; }
}