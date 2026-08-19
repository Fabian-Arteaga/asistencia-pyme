using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Planillas.DTOs;
using AsistenciaPyme.Domain.Entities;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Planillas.Commands;

public class CambiarEstadoPlanillaHandler
    : IRequestHandler<CambiarEstadoPlanillaCommand, PlanillaDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public CambiarEstadoPlanillaHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<PlanillaDto?> Handle(CambiarEstadoPlanillaCommand request, CancellationToken cancellationToken)
    {
        var planilla = await _context.Planillas
            .Include(p => p.Departamento)
            .Include(p => p.Detalles)
            .FirstOrDefaultAsync(p => p.IdPlanilla == request.IdPlanilla, cancellationToken);

        if (planilla is null)
        {
            return null;
        }

        if (!Enum.IsDefined(typeof(EstadoPlanilla), request.Estado))
        {
            throw new InvalidOperationException("El estado de la planilla no es válido.");
        }

        ValidarTransicion(planilla.Estado, request.Estado);

        planilla.Estado = request.Estado;
        planilla.FechaActualizacion = DateTime.UtcNow;

        if (request.Estado == EstadoPlanilla.Cerrada)
        {
            planilla.FechaCierre = DateTime.UtcNow;
            planilla.CerradoPor = planilla.IdAdministrador ?? planilla.CerradoPor;
        }

        if (request.Estado == EstadoPlanilla.Anulada)
        {
            planilla.FechaActualizacion = DateTime.UtcNow;
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

    private static void ValidarTransicion(EstadoPlanilla estadoActual, EstadoPlanilla nuevoEstado)
    {
        if (estadoActual == EstadoPlanilla.Anulada && nuevoEstado != EstadoPlanilla.Anulada)
        {
            throw new InvalidOperationException("Una planilla anulada no puede volver a activarse.");
        }

        if (estadoActual == EstadoPlanilla.Pagada && nuevoEstado != EstadoPlanilla.Pagada)
        {
            throw new InvalidOperationException("Una planilla pagada no puede modificarse.");
        }

        if (estadoActual == EstadoPlanilla.Cerrada && nuevoEstado is EstadoPlanilla.Calculada or EstadoPlanilla.Borrador or EstadoPlanilla.EnRevision)
        {
            throw new InvalidOperationException("Una planilla cerrada no puede recalcularse ni volver a revisión.");
        }

        if (estadoActual == EstadoPlanilla.Pagada && nuevoEstado == EstadoPlanilla.Anulada)
        {
            throw new InvalidOperationException("Una planilla pagada no puede anularse.");
        }

        var transicionesValidas = new Dictionary<EstadoPlanilla, EstadoPlanilla[]>
        {
            [EstadoPlanilla.Borrador] = new[] { EstadoPlanilla.Calculada, EstadoPlanilla.Anulada },
            [EstadoPlanilla.Calculada] = new[] { EstadoPlanilla.EnRevision, EstadoPlanilla.Anulada, EstadoPlanilla.Calculada },
            [EstadoPlanilla.EnRevision] = new[] { EstadoPlanilla.Cerrada, EstadoPlanilla.Anulada, EstadoPlanilla.Calculada },
            [EstadoPlanilla.Cerrada] = new[] { EstadoPlanilla.Pagada, EstadoPlanilla.Anulada },
            [EstadoPlanilla.Pagada] = new[] { EstadoPlanilla.Pagada },
            [EstadoPlanilla.Anulada] = new[] { EstadoPlanilla.Anulada }
        };

        if (!transicionesValidas.TryGetValue(estadoActual, out var validos) || !validos.Contains(nuevoEstado))
        {
            throw new InvalidOperationException($"La transición de {estadoActual} a {nuevoEstado} no está permitida.");
        }
    }
}
