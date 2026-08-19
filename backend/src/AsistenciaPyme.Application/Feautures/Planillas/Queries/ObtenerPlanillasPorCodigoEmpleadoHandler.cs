using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Planillas.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Planillas.Queries;

public class ObtenerPlanillasPorCodigoEmpleadoHandler
    : IRequestHandler<
        ObtenerPlanillasPorCodigoEmpleadoQuery,
        List<PlanillaDto>?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerPlanillasPorCodigoEmpleadoHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<List<PlanillaDto>?> Handle(
        ObtenerPlanillasPorCodigoEmpleadoQuery request,
        CancellationToken cancellationToken)
    {
        string codigoEmpleado =
            request.CodigoEmpleado.Trim();

        var empleado = await _context.Empleados
            .AsNoTracking()
            .Where(
                e =>
                    e.CodigoEmpleado.ToLower() ==
                    codigoEmpleado.ToLower())
            .Select(
                e => new
                {
                    e.IdEmpleado
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (empleado is null)
        {
            return null;
        }

        return await _context.Planillas
            .AsNoTracking()
            .Where(
                p =>
                    p.IdEmpleado ==
                    empleado.IdEmpleado)
            .OrderByDescending(
                p => p.FechaInicioPeriodo)
            .Select(p => new PlanillaDto
            {
                IdPlanilla = p.IdPlanilla,
                IdDepartamento = p.IdDepartamento,
                NombreDepartamento = p.Departamento != null ? p.Departamento.Nombre : null,
                FechaInicioPeriodo = p.FechaInicioPeriodo,
                FechaFinPeriodo = p.FechaFinPeriodo,
                CantidadEmpleados = p.CantidadEmpleados,
                TotalSalarioBase = p.TotalSalarioBase,
                TotalHorasExtras = p.TotalHorasExtras,
                TotalIngresos = p.TotalIngresos,
                TotalDeducciones = p.TotalDeducciones,
                TotalNeto = p.TotalNeto,
                Estado = p.Estado,
                FechaGeneracion = p.FechaGeneracion,
                FechaCierre = p.FechaCierre,
                FechaCreacion = p.FechaCreacion,
                FechaActualizacion = p.FechaActualizacion,
                Detalles = new List<DetallePlanillaDto>()
            })
            .ToListAsync(cancellationToken);
    }
}