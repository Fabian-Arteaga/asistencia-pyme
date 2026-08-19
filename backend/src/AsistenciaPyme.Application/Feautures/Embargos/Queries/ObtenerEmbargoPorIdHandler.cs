using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.Embargos.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AsistenciaPyme.Application.Feautures.Embargos.Queries;
public class ObtenerEmbargoPorIdHandler : IRequestHandler<ObtenerEmbargoPorIdQuery, EmbargoDto?>
{
    private readonly IAsistenciaPymeDbContext _context;
    public ObtenerEmbargoPorIdHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }
    public async Task<EmbargoDto?> Handle(ObtenerEmbargoPorIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Embargos.AsNoTracking().Include(e => e.Empleado).Where(e => e.IdEmbargo == request.IdEmbargo).Select(e => new EmbargoDto
        {
            IdEmbargo = e.IdEmbargo,
            IdEmpleado = e.IdEmpleado,
            NombreEmpleado = e.Empleado.Nombres + " " + e.Empleado.Apellidos,
            FechaInicio = e.FechaInicio,
            FechaFin = e.FechaFin,
            TipoCalculo = (int)e.TipoCalculo,
            Monto = e.Monto,
            Porcentaje = e.Porcentaje,
            SaldoPendiente = e.SaldoPendiente,
            Activo = e.Activo,
            Referencia = e.Referencia,
            Observacion = e.Observacion
        }).FirstOrDefaultAsync(cancellationToken);
    }
}
