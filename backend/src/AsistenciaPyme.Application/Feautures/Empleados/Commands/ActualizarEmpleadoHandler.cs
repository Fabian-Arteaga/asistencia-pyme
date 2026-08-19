using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Empleados.DTOs;
using AsistenciaPyme.Domain.Entities;
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

        if (!string.IsNullOrWhiteSpace(request.NumeroINSS))
        {
            string numeroInss = request.NumeroINSS.Trim();
            bool inssDuplicado = await _context.Empleados
                .AnyAsync(e => e.IdEmpleado != request.IdEmpleado && e.NumeroINSS != null && e.NumeroINSS == numeroInss, cancellationToken);

            if (inssDuplicado)
            {
                throw new InvalidOperationException("Ya existe otro empleado con ese número de INSS.");
            }
        }

        var departamentoAnterior = empleado.IdDepartamento;

        empleado.IdCargo = request.IdCargo;
        empleado.IdDepartamento = request.IdDepartamento;
        empleado.IdHorarioLaboral = request.IdHorarioLaboral;
        empleado.CodigoEmpleado = codigoEmpleado;
        empleado.Identificacion = identificacion;
        empleado.NumeroINSS = string.IsNullOrWhiteSpace(request.NumeroINSS) ? null : request.NumeroINSS.Trim();
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

        if (departamentoAnterior.HasValue && request.IdDepartamento.HasValue && departamentoAnterior.Value != request.IdDepartamento.Value)
        {
            var historialAbierto = await _context.EmpleadoDepartamentoHistorials
                .FirstOrDefaultAsync(h => h.IdEmpleado == empleado.IdEmpleado && h.IdDepartamento == departamentoAnterior.Value && h.FechaFin == null, cancellationToken);

            if (historialAbierto is not null)
            {
                historialAbierto.FechaFin = DateTime.UtcNow;
            }

            _context.EmpleadoDepartamentoHistorials.Add(new EmpleadoDepartamentoHistorial
            {
                IdEmpleado = empleado.IdEmpleado,
                IdDepartamento = request.IdDepartamento.Value,
                FechaInicio = DateTime.UtcNow
            });
        }
        else if (!departamentoAnterior.HasValue && request.IdDepartamento.HasValue)
        {
            _context.EmpleadoDepartamentoHistorials.Add(new EmpleadoDepartamentoHistorial
            {
                IdEmpleado = empleado.IdEmpleado,
                IdDepartamento = request.IdDepartamento.Value,
                FechaInicio = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new EmpleadoDto
        {
            IdEmpleado = empleado.IdEmpleado,
            IdCargo = empleado.IdCargo,
            IdDepartamento = empleado.IdDepartamento,
            IdHorarioLaboral = empleado.IdHorarioLaboral,
            NombreCargo = cargo.Nombre,
            NombreDepartamento = empleado.Departamento?.Nombre,
            NombreHorarioLaboral = empleado.HorarioLaboral?.Nombre,
            CodigoEmpleado = empleado.CodigoEmpleado,
            Identificacion = empleado.Identificacion,
            NumeroINSS = empleado.NumeroINSS,
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