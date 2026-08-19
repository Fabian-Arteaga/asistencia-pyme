using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.HorariosLaborales.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AsistenciaPyme.Application.Feautures.HorariosLaborales.Queries;
public class ObtenerHorarioLaboralPorIdHandler : IRequestHandler<ObtenerHorarioLaboralPorIdQuery, HorarioLaboralDto?>
{
    private readonly IAsistenciaPymeDbContext _context;
    public ObtenerHorarioLaboralPorIdHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }
    public async Task<HorarioLaboralDto?> Handle(ObtenerHorarioLaboralPorIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.HorariosLaborales
            .AsNoTracking()
            .Where(h => h.IdHorarioLaboral == request.IdHorarioLaboral)
            .Select(h => new HorarioLaboralDto
            {
                IdHorarioLaboral = h.IdHorarioLaboral,
                Nombre = h.Nombre,
                HoraEntrada = h.HoraEntrada,
                HoraSalida = h.HoraSalida,
                DiasLaborales = h.DiasLaborales,
                Activo = h.Activo
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
