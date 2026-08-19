using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.Embargos.DTOs;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AsistenciaPyme.Application.Feautures.Embargos.Commands;
public class ActualizarEmbargoHandler : IRequestHandler<ActualizarEmbargoCommand, EmbargoDto?>
{
    private readonly IAsistenciaPymeDbContext _context;
    public ActualizarEmbargoHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }
    public async Task<EmbargoDto?> Handle(ActualizarEmbargoCommand request, CancellationToken cancellationToken)
    {
        var embargo = await _context.Embargos.FirstOrDefaultAsync(e => e.IdEmbargo == request.IdEmbargo, cancellationToken);
        if (embargo is null) return null;
        embargo.IdEmpleado = request.IdEmpleado;
        embargo.FechaInicio = request.FechaInicio;
        embargo.FechaFin = request.FechaFin;
        embargo.TipoCalculo = (TipoCalculoEmbargo)request.TipoCalculo;
        embargo.Monto = request.Monto;
        embargo.Porcentaje = request.Porcentaje;
        embargo.SaldoPendiente = request.SaldoPendiente;
        embargo.Referencia = request.Referencia;
        embargo.Observacion = request.Observacion;
        await _context.SaveChangesAsync(cancellationToken);
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
