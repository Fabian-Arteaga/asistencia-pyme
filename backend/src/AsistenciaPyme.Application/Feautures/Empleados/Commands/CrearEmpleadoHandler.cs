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

        if (request.IdDepartamento.HasValue)
        {
            bool departamentoValido = await _context.Departamentos
                .AnyAsync(d => d.IdDepartamento == request.IdDepartamento.Value && d.Activo, cancellationToken);

            if (!departamentoValido)
            {
                throw new InvalidOperationException("El departamento seleccionado no existe o está inactivo.");
            }
        }

        if (request.IdHorarioLaboral.HasValue)
        {
            bool horarioValido = await _context.HorariosLaborales
                .AnyAsync(h => h.IdHorarioLaboral == request.IdHorarioLaboral.Value && h.Activo, cancellationToken);

            if (!horarioValido)
            {
                throw new InvalidOperationException("El horario laboral seleccionado no existe o está inactivo.");
            }
        }

        if (!string.IsNullOrWhiteSpace(request.NumeroINSS))
        {
            string numeroInss = request.NumeroINSS.Trim();
            bool inssDuplicado = await _context.Empleados
                .AnyAsync(e => e.NumeroINSS != null && e.NumeroINSS == numeroInss, cancellationToken);

            if (inssDuplicado)
            {
                throw new InvalidOperationException("Ya existe un empleado con ese número de INSS.");
            }
        }

        var empleado = new Empleado
        {
            IdCargo = request.IdCargo,
            IdDepartamento = request.IdDepartamento,
            IdHorarioLaboral = request.IdHorarioLaboral,
            CodigoEmpleado = codigoEmpleado,
            PinHash = _pinHasher.CrearHash(request.Pin),
            Identificacion = identificacion,
            NumeroINSS = string.IsNullOrWhiteSpace(request.NumeroINSS) ? null : request.NumeroINSS.Trim(),
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

        if (empleado.IdDepartamento.HasValue)
        {
            _context.EmpleadoDepartamentoHistorials.Add(new EmpleadoDepartamentoHistorial
            {
                IdEmpleado = empleado.IdEmpleado,
                IdDepartamento = empleado.IdDepartamento.Value,
                FechaInicio = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return empleado.IdEmpleado;
    }
}