using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Vacaciones.DTOs;
using AsistenciaPyme.Domain.Entities;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Vacaciones.Commands;

public class CrearVacacionHandler
    : IRequestHandler<CrearVacacionCommand, VacacionDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public CrearVacacionHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<VacacionDto?> Handle(
        CrearVacacionCommand request,
        CancellationToken cancellationToken)
    {
        string codigoEmpleado = request.CodigoEmpleado.Trim();

        Empleado? empleado = await _context.Empleados
            .AsNoTracking()
            .FirstOrDefaultAsync(
                e => e.CodigoEmpleado.ToLower() ==
                     codigoEmpleado.ToLower(),
                cancellationToken);

        if (empleado is null)
        {
            return null;
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
                        a.IdAdministrador == request.IdAdministrador &&
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

        bool vacacionesCruzadas = await _context.Vacaciones
            .AsNoTracking()
            .AnyAsync(
                v =>
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

        DateTime fechaActual = DateTime.UtcNow;

        var vacacion = new Vacacion
        {
            IdEmpleado = empleado.IdEmpleado,
            IdAdministrador = administrador.IdAdministrador,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,

            Motivo = string.IsNullOrWhiteSpace(request.Motivo)
                ? null
                : request.Motivo.Trim(),

            Observacion = string.IsNullOrWhiteSpace(request.Observacion)
                ? null
                : request.Observacion.Trim(),

            Cancelada = false,
            FechaCreacion = fechaActual,
            FechaActualizacion = null
        };

        await _context.Vacaciones.AddAsync(
            vacacion,
            cancellationToken);

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