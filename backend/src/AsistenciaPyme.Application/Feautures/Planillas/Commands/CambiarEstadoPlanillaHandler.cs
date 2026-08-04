using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Planillas.DTOs;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Planillas.Commands;

public class CambiarEstadoPlanillaHandler
    : IRequestHandler<
        CambiarEstadoPlanillaCommand,
        PlanillaDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public CambiarEstadoPlanillaHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<PlanillaDto?> Handle(
        CambiarEstadoPlanillaCommand request,
        CancellationToken cancellationToken)
    {
        var planilla = await _context.Planillas
            .Include(p => p.Empleado)
            .Include(p => p.Administrador)
            .Include(p => p.Deducciones)
                .ThenInclude(d => d.TipoDeduccion)
            .FirstOrDefaultAsync(
                p =>
                    p.IdPlanilla ==
                    request.IdPlanilla,
                cancellationToken);

        if (planilla is null)
        {
            return null;
        }

        bool estadoValido = Enum.IsDefined(
            typeof(EstadoPlanilla),
            request.Estado);

        if (!estadoValido)
        {
            throw new InvalidOperationException(
                "El estado de la planilla no es válido.");
        }

        if (planilla.Estado ==
                EstadoPlanilla.Anulada &&
            request.Estado !=
                EstadoPlanilla.Anulada)
        {
            throw new InvalidOperationException(
                "Una planilla anulada no puede volver a activarse.");
        }

        if (planilla.Estado != request.Estado)
        {
            planilla.Estado =
                request.Estado;

            planilla.FechaActualizacion =
                DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        return new PlanillaDto
        {
            IdPlanilla =
                planilla.IdPlanilla,

            IdEmpleado =
                planilla.IdEmpleado,

            CodigoEmpleado =
                planilla.Empleado.CodigoEmpleado,

            NombreEmpleado =
                planilla.Empleado.Nombres + " " +
                planilla.Empleado.Apellidos,

            IdAdministrador =
                planilla.IdAdministrador,

            NombreAdministrador =
                planilla.Administrador.Nombres + " " +
                planilla.Administrador.Apellidos,

            FechaInicioPeriodo =
                planilla.FechaInicioPeriodo,

            FechaFinPeriodo =
                planilla.FechaFinPeriodo,

            SalarioBasePeriodo =
                planilla.SalarioBasePeriodo,

            IngresosAdicionales =
                planilla.IngresosAdicionales,

            SalarioBruto =
                planilla.SalarioBasePeriodo +
                planilla.IngresosAdicionales,

            TotalDeducciones =
                planilla.TotalDeducciones,

            SalarioNeto =
                planilla.SalarioNeto,

            Estado =
                planilla.Estado,

            FechaCreacion =
                planilla.FechaCreacion,

            FechaActualizacion =
                planilla.FechaActualizacion,

            Deducciones =
                planilla.Deducciones
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
        };
    }
}