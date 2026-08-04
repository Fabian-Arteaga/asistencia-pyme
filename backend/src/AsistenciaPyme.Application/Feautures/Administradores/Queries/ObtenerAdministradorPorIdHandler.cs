using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Administradores.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Administradores.Queries;

public class ObtenerAdministradorPorIdHandler
    : IRequestHandler<
        ObtenerAdministradorPorIdQuery,
        AdministradorDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerAdministradorPorIdHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<AdministradorDto?> Handle(
        ObtenerAdministradorPorIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Administradores
            .AsNoTracking()
            .Where(administrador =>
                administrador.IdAdministrador ==
                request.IdAdministrador)
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}