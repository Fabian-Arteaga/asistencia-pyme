using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Empleados.DTOs;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Empleados.Commands;

public class CambiarEstadoEmpleadoHandler
    : IRequestHandler<CambiarEstadoEmpleadoCommand, EmpleadoDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public CambiarEstadoEmpleadoHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<EmpleadoDto?> Handle(
        CambiarEstadoEmpleadoCommand request,
        CancellationToken cancellationToken)
    {
        var empleado = await _context.Empleados
            .Include(e => e.Cargo)
            .FirstOrDefaultAsync(
                e => e.IdEmpleado == request.IdEmpleado,
                cancellationToken);

        if (empleado is null)
        {
            return null;
        }

        empleado.Estado = request.Activo
            ? EstadoEmpleado.Activo
            : EstadoEmpleado.Inactivo;

        empleado.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new EmpleadoDto
        {
            IdEmpleado = empleado.IdEmpleado,
            IdCargo = empleado.IdCargo,
            NombreCargo = empleado.Cargo.Nombre,
            CodigoEmpleado = empleado.CodigoEmpleado,
            Identificacion = empleado.Identificacion,
            Nombres = empleado.Nombres,
            Apellidos = empleado.Apellidos,
            Telefono = empleado.Telefono,
            Correo = empleado.Correo,
            Direccion = empleado.Direccion,
            FechaContratacion = empleado.FechaContratacion,
            SalarioBase = empleado.SalarioBase,
            Activo = empleado.Estado == EstadoEmpleado.Activo,
            FechaCreacion = empleado.FechaCreacion,
            FechaActualizacion = empleado.FechaActualizacion
        };
    }
}