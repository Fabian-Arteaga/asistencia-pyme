using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.HorasExtras.DTOs;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AsistenciaPyme.Application.Feautures.HorasExtras.Commands;
public class AprobarHoraExtraHandler : IRequestHandler<AprobarHoraExtraCommand, HoraExtraDto?>
{
    private readonly IAsistenciaPymeDbContext _context;
    public AprobarHoraExtraHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }
    public async Task<HoraExtraDto?> Handle(AprobarHoraExtraCommand request, CancellationToken cancellationToken)
    {
        var horaExtra = await _context.HorasExtras
            .Include(h => h.Empleado)
            .FirstOrDefaultAsync(h => h.IdHoraExtra == request.IdHoraExtra, cancellationToken);

        if (horaExtra is null) return null;
        if (request.MinutosAprobados > horaExtra.MinutosDetectados) throw new InvalidOperationException("Los minutos aprobados no pueden ser mayores a los detectados.");

        horaExtra.MinutosAprobados = request.MinutosAprobados;
        horaExtra.Estado = EstadoHoraExtra.Aprobada;
        horaExtra.FechaAprobacion = DateTime.UtcNow;
        horaExtra.AprobadoPor = request.AprobadoPor;
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
            Observacion = horaExtra.Observacion,
            AprobadoPor = horaExtra.AprobadoPor,
            FechaAprobacion = horaExtra.FechaAprobacion
        };
    }
}
