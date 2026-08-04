using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Asistencias.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Asistencias.Queries;

public class ObtenerAsistenciasPorCodigoHandler
    : IRequestHandler<
        ObtenerAsistenciasPorCodigoQuery,
        List<AsistenciaDto>?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerAsistenciasPorCodigoHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<List<AsistenciaDto>?> Handle(
        ObtenerAsistenciasPorCodigoQuery request,
        CancellationToken cancellationToken)
    {
        string codigoEmpleado = request.CodigoEmpleado.Trim();

        var empleado = await _context.Empleados
            .AsNoTracking()
            .Where(e =>
                e.CodigoEmpleado.ToLower() ==
                codigoEmpleado.ToLower())
            .Select(e => new
            {
                e.IdEmpleado
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (empleado is null)
        {
            return null;
        }

        return await _context.Asistencias
            .AsNoTracking()
            .Where(a => a.IdEmpleado == empleado.IdEmpleado)
            .OrderByDescending(a => a.HoraEntrada)
            .Select(a => new AsistenciaDto
            {
                IdAsistencia = a.IdAsistencia,
                IdEmpleado = a.IdEmpleado,
                CodigoEmpleado = a.Empleado.CodigoEmpleado,
                NombreEmpleado =
                    a.Empleado.Nombres + " " +
                    a.Empleado.Apellidos,
                HoraEntrada = a.HoraEntrada,
                HoraSalida = a.HoraSalida,
                Observacion = a.Observacion,
                Corregida = a.Corregida,
                MotivoCorreccion = a.MotivoCorreccion,
                IdAdministrador = a.IdAdministrador,
                FechaCorreccion = a.FechaCorreccion,
                FechaCreacion = a.FechaCreacion,
                FechaActualizacion = a.FechaActualizacion
            })
            .ToListAsync(cancellationToken);
    }
}