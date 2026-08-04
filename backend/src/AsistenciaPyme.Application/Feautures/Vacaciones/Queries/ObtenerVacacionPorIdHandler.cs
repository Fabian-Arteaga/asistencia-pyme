using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Vacaciones.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Vacaciones.Queries;

public class ObtenerVacacionPorIdHandler
    : IRequestHandler<ObtenerVacacionPorIdQuery, VacacionDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerVacacionPorIdHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<VacacionDto?> Handle(
        ObtenerVacacionPorIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Vacaciones
            .AsNoTracking()
            .Where(v => v.IdVacacion == request.IdVacacion)
            .Select(v => new VacacionDto
            {
                IdVacacion = v.IdVacacion,
                IdEmpleado = v.IdEmpleado,
                CodigoEmpleado = v.Empleado.CodigoEmpleado,
                NombreEmpleado =
                    v.Empleado.Nombres + " " +
                    v.Empleado.Apellidos,
                IdAdministrador = v.IdAdministrador,
                NombreAdministrador =
                    v.Administrador.Nombres + " " +
                    v.Administrador.Apellidos,
                FechaInicio = v.FechaInicio,
                FechaFin = v.FechaFin,
                Motivo = v.Motivo,
                Observacion = v.Observacion,
                Cancelada = v.Cancelada,
                FechaCreacion = v.FechaCreacion,
                FechaActualizacion = v.FechaActualizacion
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}