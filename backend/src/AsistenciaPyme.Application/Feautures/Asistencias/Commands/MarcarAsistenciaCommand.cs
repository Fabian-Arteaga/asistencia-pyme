using AsistenciaPyme.Application.Feautures.Asistencias.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Asistencias.Commands;

public class MarcarAsistenciaCommand
    : IRequest<ResultadoMarcacionDto>
{
    public string CodigoEmpleado { get; set; } = string.Empty;

    public string Pin { get; set; } = string.Empty;
}