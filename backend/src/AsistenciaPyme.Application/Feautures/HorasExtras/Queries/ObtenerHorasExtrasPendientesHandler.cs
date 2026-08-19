using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.HorasExtras.DTOs;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AsistenciaPyme.Application.Feautures.HorasExtras.Queries;
public class ObtenerHorasExtrasPendientesHandler : IRequestHandler<ObtenerHorasExtrasPendientesQuery, List<HoraExtraDto>>
{
    private readonly IAsistenciaPymeDbContext _context;
    public ObtenerHorasExtrasPendientesHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }
    public async Task<List<HoraExtraDto>> Handle(ObtenerHorasExtrasPendientesQuery request, CancellationToken cancellationToken)
    {
        return await _context.HorasExtras.AsNoTracking().Include(h => h.Empleado).Where(h => h.Estado == EstadoHoraExtra.Pendiente).OrderByDescending(h => h.Fecha).Select(h => new HoraExtraDto
        {
            IdHoraExtra = h.IdHoraExtra,
            IdEmpleado = h.IdEmpleado,
            NombreEmpleado = h.Empleado.Nombres + " " + h.Empleado.Apellidos,
            Fecha = h.Fecha,
            MinutosDetectados = h.MinutosDetectados,
            MinutosAprobados = h.MinutosAprobados,
            Estado = (int)h.Estado,
            Observacion = h.Observacion
        }).ToListAsync(cancellationToken);
    }
}
