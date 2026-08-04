using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Administradores.DTOs;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Administradores.Commands;

public class CambiarEstadoAdministradorHandler
    : IRequestHandler<
        CambiarEstadoAdministradorCommand,
        AdministradorDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public CambiarEstadoAdministradorHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<AdministradorDto?> Handle(
        CambiarEstadoAdministradorCommand request,
        CancellationToken cancellationToken)
    {
        Administrador? administrador =
            await _context.Administradores
                .FirstOrDefaultAsync(
                    administrador =>
                        administrador.IdAdministrador ==
                        request.IdAdministrador,
                    cancellationToken);

        if (administrador is null)
        {
            return null;
        }

        if (administrador.Activo != request.Activo)
        {
            administrador.Activo =
                request.Activo;

            administrador.FechaActualizacion =
                DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        return new AdministradorDto
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
        };
    }
}