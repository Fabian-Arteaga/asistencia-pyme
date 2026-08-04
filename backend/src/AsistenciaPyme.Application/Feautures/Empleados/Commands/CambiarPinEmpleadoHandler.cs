using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Empleados.Commands;

public class CambiarPinEmpleadoHandler
    : IRequestHandler<CambiarPinEmpleadoCommand, bool>
{
    private readonly IAsistenciaPymeDbContext _context;
    private readonly IPinHasher _pinHasher;

    public CambiarPinEmpleadoHandler(
        IAsistenciaPymeDbContext context,
        IPinHasher pinHasher)
    {
        _context = context;
        _pinHasher = pinHasher;
    }

    public async Task<bool> Handle(
        CambiarPinEmpleadoCommand request,
        CancellationToken cancellationToken)
    {
        Empleado? empleado = await _context.Empleados
            .FirstOrDefaultAsync(
                e => e.IdEmpleado == request.IdEmpleado,
                cancellationToken);

        if (empleado is null)
        {
            return false;
        }

        bool esElMismoPin = _pinHasher.Verificar(
            request.NuevoPin,
            empleado.PinHash);

        if (esElMismoPin)
        {
            throw new InvalidOperationException(
                "El nuevo PIN debe ser diferente del PIN actual.");
        }

        empleado.PinHash = _pinHasher.CrearHash(
            request.NuevoPin);

        empleado.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(
            cancellationToken);

        return true;
    }
}