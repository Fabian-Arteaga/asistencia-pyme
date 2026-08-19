using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.HorariosLaborales.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AsistenciaPyme.Application.Feautures.HorariosLaborales.Queries;
public class ObtenerHorariosLaboralesHandler : IRequestHandler<ObtenerHorariosLaboralesQuery, List<HorarioLaboralDto>>
{
    private readonly IAsistenciaPymeDbContext _context;
    public ObtenerHorariosLaboralesHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }
    public async Task<List<HorarioLaboralDto>> Handle(ObtenerHorariosLaboralesQuery request, CancellationToken cancellationToken)
    {
        return await _context.HorariosLaborales
            .AsNoTracking()
            .OrderBy(h => h.Nombre)
            .Select(h => new HorarioLaboralDto
            {
                IdHorarioLaboral = h.IdHorarioLaboral,
                Nombre = h.Nombre,
                HoraEntrada = h.HoraEntrada,
                HoraSalida = h.HoraSalida,
                DiasLaborales = h.DiasLaborales,
                Activo = h.Activo
            })
            .ToListAsync(cancellationToken);
    }
}
