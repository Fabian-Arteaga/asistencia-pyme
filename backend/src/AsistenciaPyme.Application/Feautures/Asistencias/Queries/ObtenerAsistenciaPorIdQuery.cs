using AsistenciaPyme.Application.Features.Asistencias.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Asistencias.Queries;

public class ObtenerAsistenciaPorIdQuery
    : IRequest<AsistenciaDto?>
{
    public int IdAsistencia { get; set; }
}