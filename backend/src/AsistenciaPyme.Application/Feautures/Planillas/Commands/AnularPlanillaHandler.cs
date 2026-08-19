using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Planillas.DTOs;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Planillas.Commands;

public class AnularPlanillaHandler : IRequestHandler<AnularPlanillaCommand, PlanillaDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public AnularPlanillaHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<PlanillaDto?> Handle(AnularPlanillaCommand request, CancellationToken cancellationToken)
    {
        var planilla = await _context.Planillas
            .Include(p => p.Departamento)
            .Include(p => p.Detalles)
            .FirstOrDefaultAsync(p => p.IdPlanilla == request.IdPlanilla, cancellationToken);

        if (planilla is null)
        {
            return null;
        }

        if (planilla.Estado == EstadoPlanilla.Anulada)
        {
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
                FechaCierre = planilla.FechaCierre,
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

        if (planilla.Estado == EstadoPlanilla.Pagada)
        {
            throw new InvalidOperationException("Una planilla pagada no puede anularse.");
        }

        var administrador = await _context.Administradores
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.IdAdministrador == request.IdAdministrador && a.Activo, cancellationToken);

        if (administrador is null)
        {
            throw new InvalidOperationException("El administrador no existe o está inactivo.");
        }

        planilla.Estado = EstadoPlanilla.Anulada;
        planilla.IdAdministrador = administrador.IdAdministrador;
        planilla.FechaActualizacion = DateTime.UtcNow;

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
            FechaCierre = planilla.FechaCierre,
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
