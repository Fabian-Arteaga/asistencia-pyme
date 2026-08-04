using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.Asistencias.DTOs;
using AsistenciaPyme.Domain.Entities;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Asistencias.Commands;

public class MarcarAsistenciaHandler
    : IRequestHandler<MarcarAsistenciaCommand, ResultadoMarcacionDto>
{
    private readonly IAsistenciaPymeDbContext _context;
    private readonly IPinHasher _pinHasher;

    public MarcarAsistenciaHandler(
        IAsistenciaPymeDbContext context,
        IPinHasher pinHasher)
    {
        _context = context;
        _pinHasher = pinHasher;
    }

    public async Task<ResultadoMarcacionDto> Handle(
        MarcarAsistenciaCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CodigoEmpleado) ||
            string.IsNullOrWhiteSpace(request.Pin))
        {
            throw new UnauthorizedAccessException(
                "Código de empleado o PIN incorrectos.");
        }

        string codigoEmpleado = request.CodigoEmpleado.Trim();

        Empleado? empleado = await _context.Empleados
            .AsNoTracking()
            .FirstOrDefaultAsync(
                e => e.CodigoEmpleado.ToLower() ==
                     codigoEmpleado.ToLower(),
                cancellationToken);

        if (empleado is null)
        {
            throw new UnauthorizedAccessException(
                "Código de empleado o PIN incorrectos.");
        }

        bool pinCorrecto = _pinHasher.Verificar(
            request.Pin,
            empleado.PinHash);

        if (!pinCorrecto)
        {
            throw new UnauthorizedAccessException(
                "Código de empleado o PIN incorrectos.");
        }

        if (empleado.Estado != EstadoEmpleado.Activo)
        {
            throw new InvalidOperationException(
                "El empleado está inactivo y no puede marcar asistencia.");
        }

        DateTime fechaHoraActual = DateTime.UtcNow;

        Asistencia? asistenciaAbierta = await _context.Asistencias
            .Where(a =>
                a.IdEmpleado == empleado.IdEmpleado &&
                a.HoraSalida == null)
            .OrderByDescending(a => a.HoraEntrada)
            .FirstOrDefaultAsync(cancellationToken);

        if (asistenciaAbierta is null)
        {
            var nuevaAsistencia = new Asistencia
            {
                IdEmpleado = empleado.IdEmpleado,
                HoraEntrada = fechaHoraActual,
                HoraSalida = null,
                Observacion = null,
                Corregida = false,
                FechaCreacion = fechaHoraActual
            };

            await _context.Asistencias.AddAsync(
                nuevaAsistencia,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return new ResultadoMarcacionDto
            {
                IdAsistencia = nuevaAsistencia.IdAsistencia,
                IdEmpleado = empleado.IdEmpleado,
                CodigoEmpleado = empleado.CodigoEmpleado,
                NombreEmpleado =
                    $"{empleado.Nombres} {empleado.Apellidos}",
                TipoMarcacion = "Entrada",
                FechaHoraMarcacion = fechaHoraActual,
                HoraEntrada = nuevaAsistencia.HoraEntrada,
                HoraSalida = null,
                Mensaje = "Entrada registrada correctamente."
            };
        }

        asistenciaAbierta.HoraSalida = fechaHoraActual;
        asistenciaAbierta.FechaActualizacion = fechaHoraActual;

        await _context.SaveChangesAsync(cancellationToken);

        return new ResultadoMarcacionDto
        {
            IdAsistencia = asistenciaAbierta.IdAsistencia,
            IdEmpleado = empleado.IdEmpleado,
            CodigoEmpleado = empleado.CodigoEmpleado,
            NombreEmpleado =
                $"{empleado.Nombres} {empleado.Apellidos}",
            TipoMarcacion = "Salida",
            FechaHoraMarcacion = fechaHoraActual,
            HoraEntrada = asistenciaAbierta.HoraEntrada,
            HoraSalida = asistenciaAbierta.HoraSalida,
            Mensaje = "Salida registrada correctamente."
        };
    }
}