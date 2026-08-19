using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.Embargos.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AsistenciaPyme.Application.Feautures.Embargos.Commands;
public class CambiarEstadoEmbargoHandler : IRequestHandler<CambiarEstadoEmbargoCommand, EmbargoDto?>
{
    private readonly IAsistenciaPymeDbContext _context;
    public CambiarEstadoEmbargoHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }
    public async Task<EmbargoDto?> Handle(CambiarEstadoEmbargoCommand request, CancellationToken cancellationToken)
    {
        var embargo = await _context.Embargos.FirstOrDefaultAsync(e => e.IdEmbargo == request.IdEmbargo, cancellationToken);
        if (embargo is null) return null;
        embargo.Activo = request.Activo;
        await _context.SaveChangesAsync(cancellationToken);
        return new EmbargoDto
        {
            IdEmbargo = embargo.IdEmbargo,
            IdEmpleado = embargo.IdEmpleado,
            NombreEmpleado = embargo.Empleado.Nombres + " " + embargo.Empleado.Apellidos,
            FechaInicio = embargo.FechaInicio,
            FechaFin = embargo.FechaFin,
            TipoCalculo = (int)embargo.TipoCalculo,
            Monto = embargo.Monto,
            Porcentaje = embargo.Porcentaje,
            SaldoPendiente = embargo.SaldoPendiente,
            Activo = embargo.Activo,
            Referencia = embargo.Referencia,
            Observacion = embargo.Observacion
        };
    }
}
