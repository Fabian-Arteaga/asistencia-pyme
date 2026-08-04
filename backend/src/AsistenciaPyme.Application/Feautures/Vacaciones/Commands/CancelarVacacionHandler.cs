using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Vacaciones.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Vacaciones.Commands;

public class CancelarVacacionHandler
    : IRequestHandler<CancelarVacacionCommand, VacacionDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public CancelarVacacionHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<VacacionDto?> Handle(
        CancelarVacacionCommand request,
        CancellationToken cancellationToken)
    {
        var vacacion = await _context.Vacaciones
            .Include(v => v.Empleado)
            .Include(v => v.Administrador)
            .FirstOrDefaultAsync(
                v => v.IdVacacion == request.IdVacacion,
                cancellationToken);

        if (vacacion is null)
        {
            return null;
        }

        if (!vacacion.Cancelada)
        {
            vacacion.Cancelada = true;
            vacacion.FechaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        return new VacacionDto
        {
            IdVacacion = vacacion.IdVacacion,
            IdEmpleado = vacacion.IdEmpleado,
            CodigoEmpleado =
                vacacion.Empleado.CodigoEmpleado,
            NombreEmpleado =
                vacacion.Empleado.Nombres + " " +
                vacacion.Empleado.Apellidos,
            IdAdministrador = vacacion.IdAdministrador,
            NombreAdministrador =
                vacacion.Administrador.Nombres + " " +
                vacacion.Administrador.Apellidos,
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