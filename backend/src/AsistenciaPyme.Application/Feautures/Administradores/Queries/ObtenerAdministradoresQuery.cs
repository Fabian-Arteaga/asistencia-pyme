using AsistenciaPyme.Application.Features.Administradores.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Administradores.Queries;

public class ObtenerAdministradoresQuery
    : IRequest<List<AdministradorDto>>
{
}