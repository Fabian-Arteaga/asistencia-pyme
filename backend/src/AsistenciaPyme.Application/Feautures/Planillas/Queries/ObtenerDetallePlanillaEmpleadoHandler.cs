using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Planillas.DTOs;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace AsistenciaPyme.Application.Features.Planillas.Queries;

public class ObtenerDetallePlanillaEmpleadoHandler : IRequestHandler<ObtenerDetallePlanillaEmpleadoQuery, DetallePlanillaEmpleadoDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerDetallePlanillaEmpleadoHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<DetallePlanillaEmpleadoDto?> Handle(ObtenerDetallePlanillaEmpleadoQuery request, CancellationToken cancellationToken)
    {
        var detalle = await _context.DetallesPlanilla
            .AsNoTracking()
            .Include(d => d.Planilla)
            .Include(d => d.Empleado)
            .ThenInclude(e => e.Cargo)
            .Include(d => d.Empleado)
            .ThenInclude(e => e.Departamento)
            .Include(d => d.Conceptos)
            .ThenInclude(c => c.ConceptoPlanilla)
            .FirstOrDefaultAsync(d => d.IdPlanilla == request.IdPlanilla && d.IdEmpleado == request.IdEmpleado, cancellationToken);

        if (detalle is null)
        {
            return null;
        }

        var conceptos = detalle.Conceptos
            .Select(c => new ConceptoDetallePlanillaDto
            {
                IdConceptoPlanilla = c.IdConceptoPlanilla,
                Codigo = c.ConceptoPlanilla.Codigo,
                Nombre = c.ConceptoPlanilla.Nombre,
                Monto = c.Monto,
                Descripcion = c.Descripcion
            }).ToList();

        return new DetallePlanillaEmpleadoDto
        {
            IdPlanilla = detalle.IdPlanilla,
            IdEmpleado = detalle.IdEmpleado,
            CodigoEmpleado = detalle.CodigoEmpleado,
            NombreEmpleado = detalle.NombreEmpleado,
            NumeroINSS = detalle.NumeroINSS,
            Departamento = detalle.Departamento,
            Cargo = detalle.Cargo,
            SalarioBase = detalle.SalarioBase,
            DiasLaborados = detalle.DiasLaborados,
            MinutosLaborados = detalle.MinutosLaborados,
            CantidadTardanzas = detalle.CantidadTardanzas,
            MinutosTardanza = detalle.MinutosTardanza,
            DescuentoTardanza = detalle.DescuentoTardanza,
            MinutosExtrasDetectados = detalle.MinutosExtrasDetectados,
            MinutosExtrasAprobados = detalle.MinutosExtrasAprobados,
            MontoHorasExtras = detalle.MontoHorasExtras,
            VacacionesAcumuladasPeriodo = detalle.VacacionesAcumuladasPeriodo,
            SaldoVacaciones = detalle.SaldoVacaciones,
            TotalIngresos = detalle.TotalIngresos,
            TotalDeducciones = detalle.TotalDeducciones,
            SalarioNeto = detalle.SalarioNeto,
            IndemnizacionProyectada = detalle.IndemnizacionProyectada,
            Conceptos = conceptos,
            INSS = conceptos.Where(c => c.Codigo == "INSS").Sum(c => c.Monto),
            Embargos = conceptos.Where(c => c.Codigo == "EMBARGO").Sum(c => c.Monto),
            OtrasDeducciones = conceptos.Where(c => c.Codigo == "OTRA_DEDUCCION" || c.Codigo == "TARDANZA").Sum(c => c.Monto)
        };
    }
}
