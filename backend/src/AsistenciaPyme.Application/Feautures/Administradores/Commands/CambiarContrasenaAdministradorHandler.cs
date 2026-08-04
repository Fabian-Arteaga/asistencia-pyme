using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Administradores.Commands;

public class CambiarContrasenaAdministradorHandler
    : IRequestHandler<
        CambiarContrasenaAdministradorCommand,
        bool>
{
    private readonly IAsistenciaPymeDbContext _context;
    private readonly IContrasenaHasher _contrasenaHasher;

    public CambiarContrasenaAdministradorHandler(
        IAsistenciaPymeDbContext context,
        IContrasenaHasher contrasenaHasher)
    {
        _context = context;
        _contrasenaHasher = contrasenaHasher;
    }

    public async Task<bool> Handle(
        CambiarContrasenaAdministradorCommand request,
        CancellationToken cancellationToken)
    {
        Administrador? administrador =
            await _context.Administradores
                .FirstOrDefaultAsync(
                    a =>
                        a.IdAdministrador ==
                            request.IdAdministrador &&
                        a.Activo,
                    cancellationToken);

        if (administrador is null)
        {
            return false;
        }

        bool contrasenaActualCorrecta =
            _contrasenaHasher.Verificar(
                request.ContrasenaActual,
                administrador.ContrasenaHash);

        if (!contrasenaActualCorrecta)
        {
            throw new InvalidOperationException(
                "La contraseña actual es incorrecta.");
        }

        bool esLaMismaContrasena =
            _contrasenaHasher.Verificar(
                request.NuevaContrasena,
                administrador.ContrasenaHash);

        if (esLaMismaContrasena)
        {
            throw new InvalidOperationException(
                "La nueva contraseña debe ser diferente " +
                "de la contraseña actual.");
        }

        administrador.ContrasenaHash =
            _contrasenaHasher.CrearHash(
                request.NuevaContrasena);

        administrador.FechaActualizacion =
            DateTime.UtcNow;

        await _context.SaveChangesAsync(
            cancellationToken);

        return true;
    }
}