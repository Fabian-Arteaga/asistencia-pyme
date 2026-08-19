using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Planillas.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Planillas.Queries;

public class ObtenerPlanillaPorIdHandler
    : IRequestHandler<
        ObtenerPlanillaPorIdQuery,
        PlanillaDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerPlanillaPorIdHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<PlanillaDto?> Handle(
        ObtenerPlanillaPorIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Planillas
            .AsNoTracking()
            .Where(
                p =>
                    p.IdPlanilla ==
                    request.IdPlanilla)
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}