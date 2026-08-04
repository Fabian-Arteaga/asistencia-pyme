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
                IdPlanilla =
                    p.IdPlanilla,

                IdEmpleado =
                    p.IdEmpleado,

                CodigoEmpleado =
                    p.Empleado.CodigoEmpleado,

                NombreEmpleado =
                    p.Empleado.Nombres + " " +
                    p.Empleado.Apellidos,

                IdAdministrador =
                    p.IdAdministrador,

                NombreAdministrador =
                    p.Administrador.Nombres + " " +
                    p.Administrador.Apellidos,

                FechaInicioPeriodo =
                    p.FechaInicioPeriodo,

                FechaFinPeriodo =
                    p.FechaFinPeriodo,

                SalarioBasePeriodo =
                    p.SalarioBasePeriodo,

                IngresosAdicionales =
                    p.IngresosAdicionales,

                SalarioBruto =
                    p.SalarioBasePeriodo +
                    p.IngresosAdicionales,

                TotalDeducciones =
                    p.TotalDeducciones,

                SalarioNeto =
                    p.SalarioNeto,

                Estado =
                    p.Estado,

                FechaCreacion =
                    p.FechaCreacion,

                FechaActualizacion =
                    p.FechaActualizacion,

                Deducciones =
                    p.Deducciones
                        .OrderBy(
                            d =>
                                d.IdDeduccionPlanilla)
                        .Select(
                            d =>
                                new DeduccionPlanillaDto
                                {
                                    IdDeduccionPlanilla =
                                        d.IdDeduccionPlanilla,

                                    IdTipoDeduccion =
                                        d.IdTipoDeduccion,

                                    NombreTipoDeduccion =
                                        d.TipoDeduccion.Nombre,

                                    TipoCalculo =
                                        d.TipoDeduccion.TipoCalculo,

                                    ValorAplicado =
                                        d.ValorAplicado,

                                    MontoCalculado =
                                        d.MontoCalculado,

                                    Observacion =
                                        d.Observacion
                                })
                        .ToList()
            })
            .ToListAsync(cancellationToken);
    }
}