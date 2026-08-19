using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AsistenciaPyme.Application.Feautures.Embargos.Commands;
public class CrearEmbargoHandler : IRequestHandler<CrearEmbargoCommand, int>
{
    private readonly IAsistenciaPymeDbContext _context;
    public CrearEmbargoHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }
    public async Task<int> Handle(CrearEmbargoCommand request, CancellationToken cancellationToken)
    {
        bool empleadoExiste = await _context.Empleados.AnyAsync(e => e.IdEmpleado == request.IdEmpleado && e.Estado == AsistenciaPyme.Domain.Enums.EstadoEmpleado.Activo, cancellationToken);
        if (!empleadoExiste) throw new InvalidOperationException("El empleado no existe o está inactivo.");
        var embargo = new Embargo
        {
            IdEmpleado = request.IdEmpleado,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            TipoCalculo = (TipoCalculoEmbargo)request.TipoCalculo,
            Monto = request.Monto,
            Porcentaje = request.Porcentaje,
            SaldoPendiente = request.SaldoPendiente,
            Activo = true,
            Referencia = request.Referencia,
            Observacion = request.Observacion
        };
        await _context.Embargos.AddAsync(embargo, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return embargo.IdEmbargo;
    }
}
