using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Administradores.DTOs;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Administradores.Commands;

public class CrearAdministradorHandler
    : IRequestHandler<
        CrearAdministradorCommand,
        AdministradorDto>
{
    private readonly IAsistenciaPymeDbContext _context;
    private readonly IContrasenaHasher _contrasenaHasher;

    public CrearAdministradorHandler(
        IAsistenciaPymeDbContext context,
        IContrasenaHasher contrasenaHasher)
    {
        _context = context;
        _contrasenaHasher = contrasenaHasher;
    }

    public async Task<AdministradorDto> Handle(
        CrearAdministradorCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nombres))
        {
            throw new InvalidOperationException(
                "Los nombres son obligatorios.");
        }

        if (string.IsNullOrWhiteSpace(request.Apellidos))
        {
            throw new InvalidOperationException(
                "Los apellidos son obligatorios.");
        }

        if (string.IsNullOrWhiteSpace(request.Correo))
        {
            throw new InvalidOperationException(
                "El correo es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(request.Contrasena))
        {
            throw new InvalidOperationException(
                "La contraseña es obligatoria.");
        }

        string correo =
            request.Correo.Trim().ToLower();

        bool correoDuplicado =
            await _context.Administradores
                .AsNoTracking()
                .AnyAsync(
                    administrador =>
                        administrador.Correo.ToLower() ==
                        correo,
                    cancellationToken);

        if (correoDuplicado)
        {
            throw new InvalidOperationException(
                "Ya existe un administrador con ese correo.");
        }

        var administrador = new Administrador
        {
            Nombres = request.Nombres.Trim(),
            Apellidos = request.Apellidos.Trim(),
            Correo = correo,

            ContrasenaHash =
                _contrasenaHasher.CrearHash(
                    request.Contrasena),

            Activo = true,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = null
        };

        await _context.Administradores.AddAsync(
            administrador,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

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