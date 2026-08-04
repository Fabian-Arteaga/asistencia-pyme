using AsistenciaPyme.Application.Features.Asistencias.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Asistencias.Queries;

public class ObtenerAsistenciasQuery
    : IRequest<List<AsistenciaDto>>
{
}