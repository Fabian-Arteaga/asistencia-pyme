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
                Detalles = p.Detalles.Select(d => new DetallePlanillaDto
                {
                    IdDetallePlanilla = d.IdDetallePlanilla,
                    IdEmpleado = d.IdEmpleado,
                    CodigoEmpleado = d.CodigoEmpleado,
                    NombreEmpleado = d.NombreEmpleado,
                    NumeroINSS = d.NumeroINSS,
                    Cargo = d.Cargo,
                    Departamento = d.Departamento,
                    SalarioBase = d.SalarioBase,
                    DiasLaborados = d.DiasLaborados,
                    MinutosLaborados = d.MinutosLaborados,
                    CantidadTardanzas = d.CantidadTardanzas,
                    MinutosTardanza = d.MinutosTardanza,
                    DescuentoTardanza = d.DescuentoTardanza,
                    MinutosExtrasDetectados = d.MinutosExtrasDetectados,
                    MinutosExtrasAprobados = d.MinutosExtrasAprobados,
                    MontoHorasExtras = d.MontoHorasExtras,
                    VacacionesAcumuladasPeriodo = d.VacacionesAcumuladasPeriodo,
                    SaldoVacaciones = d.SaldoVacaciones,
                    TotalIngresos = d.TotalIngresos,
                    TotalDeducciones = d.TotalDeducciones,
                    SalarioNeto = d.SalarioNeto,
                    IndemnizacionProyectada = d.IndemnizacionProyectada
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}