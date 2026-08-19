using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Planillas.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Planillas.Queries;

public class ObtenerPlanillasHandler
    : IRequestHandler<
        ObtenerPlanillasQuery,
        List<PlanillaDto>>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerPlanillasHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<List<PlanillaDto>> Handle(
        ObtenerPlanillasQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Planillas
            .AsNoTracking()
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