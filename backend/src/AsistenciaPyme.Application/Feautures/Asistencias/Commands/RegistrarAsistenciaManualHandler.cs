using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Asistencias.DTOs;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Asistencias.Commands;

public class RegistrarAsistenciaManualHandler
    : IRequestHandler<RegistrarAsistenciaManualCommand, AsistenciaDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public RegistrarAsistenciaManualHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<AsistenciaDto?> Handle(
        RegistrarAsistenciaManualCommand request,
        CancellationToken cancellationToken)
    {
        string codigoEmpleado = request.CodigoEmpleado.Trim();

        Empleado? empleado = await _context.Empleados
            .AsNoTracking()
            .FirstOrDefaultAsync(
                e => e.CodigoEmpleado.ToLower()
                     == codigoEmpleado.ToLower(),
                cancellationToken);

        if (empleado is null)
        {
            return null;
        }

        DateTime horaEntradaUtc = request.HoraEntrada.UtcDateTime;
        DateTime? horaSalidaUtc = request.HoraSalida?.UtcDateTime;

        if (horaSalidaUtc.HasValue &&
            horaSalidaUtc.Value < horaEntradaUtc)
        {
            throw new InvalidOperationException(
                "La hora de salida no puede ser anterior a la hora de entrada.");
        }

        if (request.IdAdministrador.HasValue)
        {
            bool administradorExiste =
                await _context.Administradores
                    .AsNoTracking()
                    .AnyAsync(
                        a =>
                            a.IdAdministrador ==
                            request.IdAdministrador.Value &&
                            a.Activo,
                        cancellationToken);

            if (!administradorExiste)
            {
                throw new InvalidOperationException(
                    "El administrador no existe o está inactivo.");
            }
        }

        bool existeSolapamiento;

        if (horaSalidaUtc.HasValue)
        {
            existeSolapamiento = await _context.Asistencias
                .AsNoTracking()
                .AnyAsync(
                    a =>
                        a.IdEmpleado == empleado.IdEmpleado &&
                        a.HoraEntrada < horaSalidaUtc.Value &&
                        (a.HoraSalida == null ||
                         a.HoraSalida > horaEntradaUtc),
                    cancellationToken);
        }
        else
        {
            existeSolapamiento = await _context.Asistencias
                .AsNoTracking()
                .AnyAsync(
                    a =>
                        a.IdEmpleado == empleado.IdEmpleado &&
                        (a.HoraSalida == null ||
                         a.HoraSalida > horaEntradaUtc),
                    cancellationToken);
        }

        if (existeSolapamiento)
        {
            throw new InvalidOperationException(
                "El empleado ya tiene una asistencia dentro de ese horario.");
        }

        DateTime fechaActual = DateTime.UtcNow;

        var asistencia = new Asistencia
        {
            IdEmpleado = empleado.IdEmpleado,
            HoraEntrada = horaEntradaUtc,
            HoraSalida = horaSalidaUtc,

            Observacion = string.IsNullOrWhiteSpace(request.Observacion)
                ? null
                : request.Observacion.Trim(),

            Corregida = false,
            MotivoCorreccion = null,
            IdAdministrador = request.IdAdministrador,
            FechaCorreccion = null,
            FechaCreacion = fechaActual,
            FechaActualizacion = null
        };

        await _context.Asistencias.AddAsync(
            asistencia,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return new AsistenciaDto
        {
            IdAsistencia = asistencia.IdAsistencia,
            IdEmpleado = empleado.IdEmpleado,
            CodigoEmpleado = empleado.CodigoEmpleado,
            NombreEmpleado =
                empleado.Nombres + " " + empleado.Apellidos,
            HoraEntrada = asistencia.HoraEntrada,
            HoraSalida = asistencia.HoraSalida,
            Observacion = asistencia.Observacion,
            Corregida = asistencia.Corregida,
            MotivoCorreccion = asistencia.MotivoCorreccion,
            IdAdministrador = asistencia.IdAdministrador,
            FechaCorreccion = asistencia.FechaCorreccion,
            FechaCreacion = asistencia.FechaCreacion,
            FechaActualizacion = asistencia.FechaActualizacion
        };
    }
}