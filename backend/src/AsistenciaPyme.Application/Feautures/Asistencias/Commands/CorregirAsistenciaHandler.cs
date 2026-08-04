using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Asistencias.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Asistencias.Commands;

public class CorregirAsistenciaHandler
    : IRequestHandler<CorregirAsistenciaCommand, AsistenciaDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public CorregirAsistenciaHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<AsistenciaDto?> Handle(
        CorregirAsistenciaCommand request,
        CancellationToken cancellationToken)
    {
        var asistencia = await _context.Asistencias
            .Include(a => a.Empleado)
            .FirstOrDefaultAsync(
                a => a.IdAsistencia == request.IdAsistencia,
                cancellationToken);

        if (asistencia is null)
        {
            return null;
        }

        DateTime horaEntradaUtc = request.HoraEntrada.UtcDateTime;

        DateTime? horaSalidaUtc =
            request.HoraSalida?.UtcDateTime;

        if (horaSalidaUtc.HasValue &&
            horaSalidaUtc.Value < horaEntradaUtc)
        {
            throw new InvalidOperationException(
                "La hora de salida no puede ser anterior a la hora de entrada.");
        }

        if (string.IsNullOrWhiteSpace(request.MotivoCorreccion))
        {
            throw new InvalidOperationException(
                "Debe indicar el motivo de la corrección.");
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

        DateTime fechaActual = DateTime.UtcNow;

        asistencia.HoraEntrada = horaEntradaUtc;
        asistencia.HoraSalida = horaSalidaUtc;

        asistencia.Observacion =
            string.IsNullOrWhiteSpace(request.Observacion)
                ? null
                : request.Observacion.Trim();

        asistencia.Corregida = true;
        asistencia.MotivoCorreccion =
            request.MotivoCorreccion.Trim();

        asistencia.IdAdministrador =
            request.IdAdministrador;

        asistencia.FechaCorreccion = fechaActual;
        asistencia.FechaActualizacion = fechaActual;

        await _context.SaveChangesAsync(cancellationToken);

        return new AsistenciaDto
        {
            IdAsistencia = asistencia.IdAsistencia,
            IdEmpleado = asistencia.IdEmpleado,
            CodigoEmpleado = asistencia.Empleado.CodigoEmpleado,
            NombreEmpleado =
                asistencia.Empleado.Nombres + " " +
                asistencia.Empleado.Apellidos,
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