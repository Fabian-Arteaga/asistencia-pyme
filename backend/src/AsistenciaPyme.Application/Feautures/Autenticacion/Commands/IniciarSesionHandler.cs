using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Common.Models;
using AsistenciaPyme.Application
    .Features.Autenticacion.DTOs;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application
    .Features.Autenticacion.Commands;

public class IniciarSesionHandler
    : IRequestHandler<
        IniciarSesionCommand,
        LoginResponseDto?>
{
    private readonly
        IAsistenciaPymeDbContext _context;

    private readonly
        IContrasenaHasher _contrasenaHasher;

    private readonly
        IJwtTokenGenerator _jwtTokenGenerator;

    public IniciarSesionHandler(
        IAsistenciaPymeDbContext context,
        IContrasenaHasher contrasenaHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _contrasenaHasher =
            contrasenaHasher;
        _jwtTokenGenerator =
            jwtTokenGenerator;
    }

    public async Task<LoginResponseDto?> Handle(
        IniciarSesionCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(
                request.Correo) ||
            string.IsNullOrWhiteSpace(
                request.Contrasena))
        {
            return null;
        }

        string correo = request.Correo
            .Trim()
            .ToLower();

        Administrador? administrador =
            await _context.Administradores
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    administrador =>
                        administrador
                            .Correo
                            .ToLower() == correo &&
                        administrador.Activo,
                    cancellationToken);

        if (administrador is null)
        {
            return null;
        }

        bool contrasenaCorrecta =
            _contrasenaHasher.Verificar(
                request.Contrasena,
                administrador.ContrasenaHash);

        if (!contrasenaCorrecta)
        {
            return null;
        }

        TokenGenerado tokenGenerado =
            _jwtTokenGenerator.Generar(
                administrador);

        return new LoginResponseDto
        {
            Token = tokenGenerado.Token,

            TipoToken = "Bearer",

            ExpiracionUtc =
                tokenGenerado.ExpiracionUtc,

            IdAdministrador =
                administrador.IdAdministrador,

            NombreCompleto =
                administrador.Nombres + " " +
                administrador.Apellidos,

            Correo =
                administrador.Correo,

            Rol =
                "Administrador"
        };
    }
}