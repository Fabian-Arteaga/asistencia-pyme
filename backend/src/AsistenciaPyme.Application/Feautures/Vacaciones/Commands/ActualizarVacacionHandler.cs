using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Vacaciones.DTOs;
using AsistenciaPyme.Domain.Entities;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Vacaciones.Commands;

public class ActualizarVacacionHandler
    : IRequestHandler<ActualizarVacacionCommand, VacacionDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ActualizarVacacionHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<VacacionDto?> Handle(
        ActualizarVacacionCommand request,
        CancellationToken cancellationToken)
    {
        Vacacion? vacacion = await _context.Vacaciones
            .FirstOrDefaultAsync(
                v => v.IdVacacion == request.IdVacacion,
                cancellationToken);

        if (vacacion is null)
        {
            return null;
        }

        if (vacacion.Cancelada)
        {
            throw new InvalidOperationException(
                "No se puede actualizar una vacación cancelada.");
        }

        string codigoEmpleado =
            request.CodigoEmpleado.Trim();

        Empleado? empleado = await _context.Empleados
            .AsNoTracking()
            .FirstOrDefaultAsync(
                e =>
                    e.CodigoEmpleado.ToLower() ==
                    codigoEmpleado.ToLower(),
                cancellationToken);

        if (empleado is null)
        {
            throw new InvalidOperationException(
                "El empleado no existe.");
        }

        if (empleado.Estado != EstadoEmpleado.Activo)
        {
            throw new InvalidOperationException(
                "El empleado está inactivo.");
        }

        Administrador? administrador =
            await _context.Administradores
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    a =>
                        a.IdAdministrador ==
                        request.IdAdministrador &&
                        a.Activo,
                    cancellationToken);

        if (administrador is null)
        {
            throw new InvalidOperationException(
                "El administrador no existe o está inactivo.");
        }

        if (request.FechaFin < request.FechaInicio)
        {
            throw new InvalidOperationException(
                "La fecha final no puede ser anterior a la fecha inicial.");
        }

        bool vacacionesCruzadas =
            await _context.Vacaciones
                .AsNoTracking()
                .AnyAsync(
                    v =>
                        v.IdVacacion != request.IdVacacion &&
                        v.IdEmpleado == empleado.IdEmpleado &&
                        !v.Cancelada &&
                        v.FechaInicio <= request.FechaFin &&
                        v.FechaFin >= request.FechaInicio,
                    cancellationToken);

        if (vacacionesCruzadas)
        {
            throw new InvalidOperationException(
                "El empleado ya tiene vacaciones registradas dentro de esas fechas.");
        }

        vacacion.IdEmpleado = empleado.IdEmpleado;
        vacacion.IdAdministrador =
            administrador.IdAdministrador;
        vacacion.FechaInicio = request.FechaInicio;
        vacacion.FechaFin = request.FechaFin;

        vacacion.Motivo =
            string.IsNullOrWhiteSpace(request.Motivo)
                ? null
                : request.Motivo.Trim();

        vacacion.Observacion =
            string.IsNullOrWhiteSpace(request.Observacion)
                ? null
                : request.Observacion.Trim();

        vacacion.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new VacacionDto
        {
            IdVacacion = vacacion.IdVacacion,
            IdEmpleado = empleado.IdEmpleado,
            CodigoEmpleado = empleado.CodigoEmpleado,
            NombreEmpleado =
                empleado.Nombres + " " + empleado.Apellidos,
            IdAdministrador = administrador.IdAdministrador,
            NombreAdministrador =
                administrador.Nombres + " " +
                administrador.Apellidos,
            FechaInicio = vacacion.FechaInicio,
            FechaFin = vacacion.FechaFin,
            Motivo = vacacion.Motivo,
            Observacion = vacacion.Observacion,
            Cancelada = vacacion.Cancelada,
            FechaCreacion = vacacion.FechaCreacion,
            FechaActualizacion = vacacion.FechaActualizacion
        };
    }
}