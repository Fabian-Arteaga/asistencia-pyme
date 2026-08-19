using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.HorasExtras.DTOs;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AsistenciaPyme.Application.Feautures.HorasExtras.Commands;
public class RechazarHoraExtraHandler : IRequestHandler<RechazarHoraExtraCommand, HoraExtraDto?>
{
    private readonly IAsistenciaPymeDbContext _context;
    public RechazarHoraExtraHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }
    public async Task<HoraExtraDto?> Handle(RechazarHoraExtraCommand request, CancellationToken cancellationToken)
    {
        var horaExtra = await _context.HorasExtras.FirstOrDefaultAsync(h => h.IdHoraExtra == request.IdHoraExtra, cancellationToken);
        if (horaExtra is null) return null;
        horaExtra.MinutosAprobados = 0;
        horaExtra.Estado = EstadoHoraExtra.Rechazada;
        horaExtra.FechaAprobacion = DateTime.UtcNow;
        horaExtra.Observacion = request.Observacion;
        await _context.SaveChangesAsync(cancellationToken);
        return new HoraExtraDto
        {
            IdHoraExtra = horaExtra.IdHoraExtra,
            IdEmpleado = horaExtra.IdEmpleado,
            NombreEmpleado = horaExtra.Empleado.Nombres + " " + horaExtra.Empleado.Apellidos,
            Fecha = horaExtra.Fecha,
            MinutosDetectados = horaExtra.MinutosDetectados,
            MinutosAprobados = horaExtra.MinutosAprobados,
            Estado = (int)horaExtra.Estado,
            Observacion = horaExtra.Observacion
        };
    }
}
