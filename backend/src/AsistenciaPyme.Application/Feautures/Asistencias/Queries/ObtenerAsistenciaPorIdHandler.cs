using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Asistencias.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Asistencias.Queries;

public class ObtenerAsistenciaPorIdHandler
    : IRequestHandler<ObtenerAsistenciaPorIdQuery, AsistenciaDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerAsistenciaPorIdHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<AsistenciaDto?> Handle(
        ObtenerAsistenciaPorIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Asistencias
            .AsNoTracking()
            .Where(a => a.IdAsistencia == request.IdAsistencia)
            .Select(a => new AsistenciaDto
            {
                IdAsistencia = a.IdAsistencia,
                IdEmpleado = a.IdEmpleado,
                CodigoEmpleado = a.Empleado.CodigoEmpleado,
                NombreEmpleado =
                    a.Empleado.Nombres + " " + a.Empleado.Apellidos,
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}