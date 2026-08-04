using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Administradores.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Administradores.Queries;

public class ObtenerAdministradoresHandler
    : IRequestHandler<
        ObtenerAdministradoresQuery,
        List<AdministradorDto>>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerAdministradoresHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<List<AdministradorDto>> Handle(
        ObtenerAdministradoresQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Administradores
            .AsNoTracking()
            .OrderBy(administrador =>
                administrador.Nombres)
            .ThenBy(administrador =>
                administrador.Apellidos)
            .Select(administrador =>
                new AdministradorDto
                {
                    IdAdministrador =
                        administrador.IdAdministrador,

                    Nombres =
                        administrador.Nombres,

                    Apellidos =
                        administrador.Apellidos,

                    Correo =
                        administrador.Correo,

                    Activo =
                        administrador.Activo,

                    FechaCreacion =
                        administrador.FechaCreacion,

                    FechaActualizacion =
                        administrador.FechaActualizacion
                })
            .ToListAsync(cancellationToken);
    }
}