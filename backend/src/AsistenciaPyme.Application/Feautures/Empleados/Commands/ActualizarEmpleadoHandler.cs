using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Empleados.DTOs;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Empleados.Commands;

public class ActualizarEmpleadoHandler
    : IRequestHandler<ActualizarEmpleadoCommand, EmpleadoDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ActualizarEmpleadoHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<EmpleadoDto?> Handle(
        ActualizarEmpleadoCommand request,
        CancellationToken cancellationToken)
    {
        var empleado = await _context.Empleados
            .FirstOrDefaultAsync(
                e => e.IdEmpleado == request.IdEmpleado,
                cancellationToken);

        if (empleado is null)
        {
            return null;
        }

        var cargo = await _context.Cargos
            .AsNoTracking()
            .FirstOrDefaultAsync(
                c => c.IdCargo == request.IdCargo,
                cancellationToken);

        if (cargo is null)
        {
            throw new InvalidOperationException(
                "El cargo seleccionado no existe.");
        }

        if (!cargo.Activo && empleado.IdCargo != request.IdCargo)
        {
            throw new InvalidOperationException(
                "No se puede asignar un cargo inactivo.");
        }

        string codigoEmpleado = request.CodigoEmpleado.Trim();
        string identificacion = request.Identificacion.Trim();

        bool codigoDuplicado = await _context.Empleados
            .AnyAsync(
                e => e.IdEmpleado != request.IdEmpleado &&
                     e.CodigoEmpleado.ToLower() ==
                     codigoEmpleado.ToLower(),
                cancellationToken);

        if (codigoDuplicado)
        {
            throw new InvalidOperationException(
                "Ya existe otro empleado con ese código.");
        }

        bool identificacionDuplicada = await _context.Empleados
            .AnyAsync(
                e => e.IdEmpleado != request.IdEmpleado &&
                     e.Identificacion.ToLower() ==
                     identificacion.ToLower(),
                cancellationToken);

        if (identificacionDuplicada)
        {
            throw new InvalidOperationException(
                "Ya existe otro empleado con esa identificación.");
        }

        empleado.IdCargo = request.IdCargo;
        empleado.CodigoEmpleado = codigoEmpleado;
        empleado.Identificacion = identificacion;
        empleado.Nombres = request.Nombres.Trim();
        empleado.Apellidos = request.Apellidos.Trim();

        empleado.Telefono =
            string.IsNullOrWhiteSpace(request.Telefono)
                ? null
                : request.Telefono.Trim();

        empleado.Correo =
            string.IsNullOrWhiteSpace(request.Correo)
                ? null
                : request.Correo.Trim();

        empleado.Direccion =
            string.IsNullOrWhiteSpace(request.Direccion)
                ? null
                : request.Direccion.Trim();

        empleado.FechaContratacion = request.FechaContratacion;
        empleado.SalarioBase = request.SalarioBase;
        empleado.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new EmpleadoDto
        {
            IdEmpleado = empleado.IdEmpleado,
            IdCargo = empleado.IdCargo,
            NombreCargo = cargo.Nombre,
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