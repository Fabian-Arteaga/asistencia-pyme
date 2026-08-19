using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.Asistencias;
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
            .Include(e => e.HorarioLaboral)
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

        var configuracion = await _context.ConfiguracionesNomina
            .OrderBy(c => c.IdConfiguracionNomina)
            .FirstOrDefaultAsync(cancellationToken);

        if (configuracion is null)
        {
            configuracion = new ConfiguracionNomina();
            await _context.ConfiguracionesNomina.AddAsync(configuracion, cancellationToken);
        }

        DateTime fechaHoraActual = DateTime.UtcNow;
        DateTime horaProgramadaEntrada = ObtenerHoraProgramada(fechaHoraActual, empleado.HorarioLaboral?.HoraEntrada ?? new TimeOnly(8, 0));
        DateTime horaProgramadaSalida = ObtenerHoraProgramada(fechaHoraActual, empleado.HorarioLaboral?.HoraSalida ?? new TimeOnly(17, 0));

        Asistencia? asistenciaAbierta = await _context.Asistencias
            .Where(a =>
                a.IdEmpleado == empleado.IdEmpleado &&
                a.HoraSalida == null)
            .OrderByDescending(a => a.HoraEntrada)
            .FirstOrDefaultAsync(cancellationToken);

        if (asistenciaAbierta is null)
        {
            var minutosTardanza = AsistenciaReglas.CalcularMinutosTardanza(fechaHoraActual, horaProgramadaEntrada, configuracion.MinutosToleranciaEntrada);
            var esEntradaTardia = minutosTardanza > 0;

            var nuevaAsistencia = new Asistencia
            {
                IdEmpleado = empleado.IdEmpleado,
                HoraEntrada = fechaHoraActual,
                HoraProgramadaEntrada = horaProgramadaEntrada,
                HoraProgramadaSalida = horaProgramadaSalida,
                EsEntradaTardia = esEntradaTardia,
                MinutosTardanza = minutosTardanza,
                HoraSalida = null,
                Observacion = esEntradaTardia ? "Entrada tardía registrada según horario asignado." : null,
                Corregida = false,
                FechaCreacion = fechaHoraActual
            };

            await _context.Asistencias.AddAsync(nuevaAsistencia, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new ResultadoMarcacionDto
            {
                IdAsistencia = nuevaAsistencia.IdAsistencia,
                IdEmpleado = empleado.IdEmpleado,
                CodigoEmpleado = empleado.CodigoEmpleado,
                NombreEmpleado = $"{empleado.Nombres} {empleado.Apellidos}",
                TipoMarcacion = "Entrada",
                FechaHoraMarcacion = fechaHoraActual,
                HoraEntrada = nuevaAsistencia.HoraEntrada,
                HoraSalida = null,
                HoraProgramadaEntrada = nuevaAsistencia.HoraProgramadaEntrada,
                HoraProgramadaSalida = nuevaAsistencia.HoraProgramadaSalida,
                EsEntradaTardia = nuevaAsistencia.EsEntradaTardia,
                MinutosTardanza = nuevaAsistencia.MinutosTardanza,
                Mensaje = esEntradaTardia ? "Entrada tardía registrada correctamente." : "Entrada registrada correctamente."
            };
        }

        asistenciaAbierta.HoraSalida = fechaHoraActual;
        asistenciaAbierta.HoraProgramadaSalida = horaProgramadaSalida;
        asistenciaAbierta.FechaActualizacion = fechaHoraActual;

        var minutosExtra = AsistenciaReglas.CalcularMinutosExtra(fechaHoraActual, horaProgramadaSalida);
        if (minutosExtra > 0)
        {
            var horaExtra = new HoraExtra
            {
                IdEmpleado = empleado.IdEmpleado,
                IdAsistencia = asistenciaAbierta.IdAsistencia,
                Fecha = fechaHoraActual.Date,
                MinutosDetectados = minutosExtra,
                MinutosAprobados = 0,
                Estado = EstadoHoraExtra.Pendiente,
                Observacion = "Horas extra detectadas al registrar salida.",
                Empleado = empleado,
                Asistencia = asistenciaAbierta
            };

            await _context.HorasExtras.AddAsync(horaExtra, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new ResultadoMarcacionDto
        {
            IdAsistencia = asistenciaAbierta.IdAsistencia,
            IdEmpleado = empleado.IdEmpleado,
            CodigoEmpleado = empleado.CodigoEmpleado,
            NombreEmpleado = $"{empleado.Nombres} {empleado.Apellidos}",
            TipoMarcacion = "Salida",
            FechaHoraMarcacion = fechaHoraActual,
            HoraEntrada = asistenciaAbierta.HoraEntrada,
            HoraSalida = asistenciaAbierta.HoraSalida,
            HoraProgramadaEntrada = asistenciaAbierta.HoraProgramadaEntrada,
            HoraProgramadaSalida = asistenciaAbierta.HoraProgramadaSalida,
            EsEntradaTardia = asistenciaAbierta.EsEntradaTardia,
            MinutosTardanza = asistenciaAbierta.MinutosTardanza,
            Mensaje = minutosExtra > 0 ? "Salida registrada correctamente. Se detectaron horas extra pendientes." : "Salida registrada correctamente."
        };
    }

    private static DateTime ObtenerHoraProgramada(DateTime fechaReferencia, TimeOnly horaProgramada)
    {
        return fechaReferencia.Date.AddHours(horaProgramada.Hour).AddMinutes(horaProgramada.Minute);
    }
}