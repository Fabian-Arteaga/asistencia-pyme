using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Planillas.DTOs;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Planillas.Commands;

public class RecalcularPlanillaHandler : IRequestHandler<RecalcularPlanillaCommand, PlanillaDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public RecalcularPlanillaHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<PlanillaDto?> Handle(RecalcularPlanillaCommand request, CancellationToken cancellationToken)
    {
        var planilla = await _context.Planillas
            .Include(p => p.Departamento)
            .Include(p => p.Detalles)
            .FirstOrDefaultAsync(p => p.IdPlanilla == request.IdPlanilla, cancellationToken);

        if (planilla is null)
        {
            return null;
        }

        if (planilla.Estado is EstadoPlanilla.Cerrada or EstadoPlanilla.Pagada or EstadoPlanilla.Anulada)
        {
            throw new InvalidOperationException("La planilla no puede recalcularse en el estado actual.");
        }

        if (request.IdAdministrador > 0)
        {
            var administrador = await _context.Administradores
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.IdAdministrador == request.IdAdministrador && a.Activo, cancellationToken);

            if (administrador is null)
            {
                throw new InvalidOperationException("El administrador no existe o está inactivo.");
            }

            planilla.IdAdministrador = administrador.IdAdministrador;
        }

        planilla.Estado = EstadoPlanilla.Calculada;
        planilla.FechaActualizacion = DateTime.UtcNow;

        var totales = planilla.Detalles; 
        if (totales.Any())
        {
            planilla.CantidadEmpleados = totales.Count;
            planilla.TotalSalarioBase = totales.Sum(d => d.SalarioBase);
            planilla.TotalHorasExtras = totales.Sum(d => d.MontoHorasExtras);
            planilla.TotalIngresos = totales.Sum(d => d.TotalIngresos);
            planilla.TotalDeducciones = totales.Sum(d => d.TotalDeducciones);
            planilla.TotalNeto = totales.Sum(d => d.SalarioNeto);
            planilla.SalarioBasePeriodo = totales.Sum(d => d.SalarioBase);
            planilla.IngresosAdicionales = totales.Sum(d => d.TotalIngresos);
            planilla.SalarioNeto = totales.Sum(d => d.SalarioNeto);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new PlanillaDto
        {
            IdPlanilla = planilla.IdPlanilla,
            IdDepartamento = planilla.IdDepartamento,
            NombreDepartamento = planilla.Departamento?.Nombre,
            FechaInicioPeriodo = planilla.FechaInicioPeriodo,
            FechaFinPeriodo = planilla.FechaFinPeriodo,
            CantidadEmpleados = planilla.CantidadEmpleados,
            TotalSalarioBase = planilla.TotalSalarioBase,
            TotalHorasExtras = planilla.TotalHorasExtras,
            TotalIngresos = planilla.TotalIngresos,
            TotalDeducciones = planilla.TotalDeducciones,
            TotalNeto = planilla.TotalNeto,
            Estado = planilla.Estado,
            FechaGeneracion = planilla.FechaGeneracion,
            FechaCreacion = planilla.FechaCreacion,
            FechaActualizacion = planilla.FechaActualizacion,
            Detalles = planilla.Detalles.Select(d => new DetallePlanillaDto
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
        };
    }
}
