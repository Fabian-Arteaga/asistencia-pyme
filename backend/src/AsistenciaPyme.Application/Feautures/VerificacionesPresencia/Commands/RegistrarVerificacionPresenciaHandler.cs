using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.VerificacionesPresencia.Commands;
using AsistenciaPyme.Application.Feautures.VerificacionesPresencia.DTOs;
using AsistenciaPyme.Application.Feautures.VerificacionesPresencia.Validators;
using AsistenciaPyme.Domain.Entities;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Feautures.VerificacionesPresencia.Commands;

public class RegistrarVerificacionPresenciaHandler : IRequestHandler<RegistrarVerificacionPresenciaCommand, VerificacionPresenciaDto>
{
    private readonly IAsistenciaPymeDbContext _context;

    public RegistrarVerificacionPresenciaHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<VerificacionPresenciaDto> Handle(RegistrarVerificacionPresenciaCommand request, CancellationToken cancellationToken)
    {
        var validator = new RegistrarVerificacionPresenciaValidator();
        validator.Validate(request);

        var empleado = await _context.Empleados
            .Include(e => e.Departamento)
            .FirstOrDefaultAsync(e => e.IdEmpleado == request.IdEmpleado && e.Estado == EstadoEmpleado.Activo, cancellationToken);

        if (empleado is null)
        {
            throw new InvalidOperationException("El empleado no existe o está inactivo.");
        }

        var dispositivo = await _context.DispositivosMarcaje
            .FirstOrDefaultAsync(d => d.IdDispositivoMarcaje == request.IdDispositivoMarcaje && d.Activo, cancellationToken);

        if (dispositivo is null)
        {
            throw new InvalidOperationException("El dispositivo no existe o está inactivo.");
        }

        if (empleado.IdDepartamento != dispositivo.IdDepartamento)
        {
            throw new InvalidOperationException("El empleado y el dispositivo no pertenecen al mismo departamento.");
        }

        var asistencia = await _context.Asistencias
            .Where(a => a.IdEmpleado == request.IdEmpleado && a.HoraSalida == null)
            .OrderByDescending(a => a.HoraEntrada)
            .FirstOrDefaultAsync(cancellationToken);

        if (asistencia is null)
        {
            throw new InvalidOperationException("No existe una asistencia abierta para el empleado.");
        }

        var configuracion = await _context.ConfiguracionesNomina
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        var tiemposMaximos = configuracion?.MinutosMaximosVerificacionDepartamento ?? 10;
        var diferencia = (int)Math.Max(0, (request.FechaHora - asistencia.HoraEntrada).TotalMinutes);
        var estado = diferencia <= tiemposMaximos
            ? EstadoVerificacionPresencia.Confirmada
            : EstadoVerificacionPresencia.FueraDeTiempo;

        var verificacion = new VerificacionPresencia
        {
            IdEmpleado = empleado.IdEmpleado,
            IdAsistencia = asistencia.IdAsistencia,
            IdDepartamento = empleado.IdDepartamento,
            IdDispositivoMarcaje = dispositivo.IdDispositivoMarcaje,
            FechaHora = request.FechaHora,
            Estado = estado,
            Empleado = empleado,
            Asistencia = asistencia,
            Departamento = empleado.Departamento,
            Dispositivo = dispositivo
        };

        await _context.VerificacionesPresencia.AddAsync(verificacion, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new VerificacionPresenciaDto
        {
            IdVerificacionPresencia = verificacion.IdVerificacionPresencia,
            IdEmpleado = verificacion.IdEmpleado,
            IdAsistencia = verificacion.IdAsistencia,
            IdDispositivoMarcaje = verificacion.IdDispositivoMarcaje,
            IdDepartamento = verificacion.IdDepartamento,
            FechaHora = verificacion.FechaHora,
            Estado = (int)verificacion.Estado,
            EstadoNombre = verificacion.Estado.ToString(),
            Mensaje = estado == EstadoVerificacionPresencia.Confirmada
                ? "Verificación confirmada dentro del tiempo permitido."
                : "La verificación excedió el tiempo permitido para el departamento."
        };
    }
}
