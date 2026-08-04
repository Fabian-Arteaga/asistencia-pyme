using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Administradores.DTOs;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Administradores.Commands;

public class ActualizarAdministradorHandler
    : IRequestHandler<
        ActualizarAdministradorCommand,
        AdministradorDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ActualizarAdministradorHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<AdministradorDto?> Handle(
        ActualizarAdministradorCommand request,
        CancellationToken cancellationToken)
    {
        Administrador? administrador =
            await _context.Administradores
                .FirstOrDefaultAsync(
                    a =>
                        a.IdAdministrador ==
                        request.IdAdministrador,
                    cancellationToken);

        if (administrador is null)
        {
            return null;
        }

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

        string correo = request.Correo
            .Trim()
            .ToLower();

        bool correoDuplicado =
            await _context.Administradores
                .AsNoTracking()
                .AnyAsync(
                    otro =>
                        otro.IdAdministrador !=
                            request.IdAdministrador &&
                        otro.Correo.ToLower() == correo,
                    cancellationToken);

        if (correoDuplicado)
        {
            throw new InvalidOperationException(
                "Ya existe otro administrador con ese correo.");
        }

        administrador.Nombres =
            request.Nombres.Trim();

        administrador.Apellidos =
            request.Apellidos.Trim();

        administrador.Correo = correo;

        administrador.FechaActualizacion =
            DateTime.UtcNow;

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