using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Domain.Entities;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Empleados.Commands;

public class CrearEmpleadoHandler
    : IRequestHandler<CrearEmpleadoCommand, int>
{
    private readonly IAsistenciaPymeDbContext _context;
    private readonly IPinHasher _pinHasher;

    public CrearEmpleadoHandler(
        IAsistenciaPymeDbContext context,
        IPinHasher pinHasher)
    {
        _context = context;
        _pinHasher = pinHasher;
    }

    public async Task<int> Handle(
        CrearEmpleadoCommand request,
        CancellationToken cancellationToken)
    {
        string codigoEmpleado = request.CodigoEmpleado.Trim();
        string identificacion = request.Identificacion.Trim();

        bool cargoExiste = await _context.Cargos
            .AnyAsync(
                c => c.IdCargo == request.IdCargo && c.Activo,
                cancellationToken);

        if (!cargoExiste)
        {
            throw new InvalidOperationException(
                "El cargo seleccionado no existe o está inactivo.");
        }

        bool codigoDuplicado = await _context.Empleados
            .AnyAsync(
                e => e.CodigoEmpleado.ToLower() ==
                     codigoEmpleado.ToLower(),
                cancellationToken);

        if (codigoDuplicado)
        {
            throw new InvalidOperationException(
                "Ya existe un empleado con ese código.");
        }

        bool identificacionDuplicada = await _context.Empleados
            .AnyAsync(
                e => e.Identificacion.ToLower() ==
                     identificacion.ToLower(),
                cancellationToken);

        if (identificacionDuplicada)
        {
            throw new InvalidOperationException(
                "Ya existe un empleado con esa identificación.");
        }

        if (string.IsNullOrWhiteSpace(request.Pin))
        {
            throw new InvalidOperationException(
                "El PIN es obligatorio.");
        }

        var empleado = new Empleado
        {
            IdCargo = request.IdCargo,
            CodigoEmpleado = codigoEmpleado,
            PinHash = _pinHasher.CrearHash(request.Pin),
            Identificacion = identificacion,
            Nombres = request.Nombres.Trim(),
            Apellidos = request.Apellidos.Trim(),

            Telefono = string.IsNullOrWhiteSpace(request.Telefono)
                ? null
                : request.Telefono.Trim(),

            Correo = string.IsNullOrWhiteSpace(request.Correo)
                ? null
                : request.Correo.Trim(),

            Direccion = string.IsNullOrWhiteSpace(request.Direccion)
                ? null
                : request.Direccion.Trim(),

            FechaContratacion = request.FechaContratacion,
            SalarioBase = request.SalarioBase,
            Estado = EstadoEmpleado.Activo,
            FechaCreacion = DateTime.UtcNow
        };

        await _context.Empleados.AddAsync(
            empleado,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return empleado.IdEmpleado;
    }
}