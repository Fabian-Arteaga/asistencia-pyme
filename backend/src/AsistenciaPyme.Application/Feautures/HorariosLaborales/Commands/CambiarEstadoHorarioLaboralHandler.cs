using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.HorariosLaborales.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AsistenciaPyme.Application.Feautures.HorariosLaborales.Commands;
public class CambiarEstadoHorarioLaboralHandler : IRequestHandler<CambiarEstadoHorarioLaboralCommand, HorarioLaboralDto?>
{
    private readonly IAsistenciaPymeDbContext _context;
    public CambiarEstadoHorarioLaboralHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }
    public async Task<HorarioLaboralDto?> Handle(CambiarEstadoHorarioLaboralCommand request, CancellationToken cancellationToken)
    {
        var horario = await _context.HorariosLaborales.FirstOrDefaultAsync(h => h.IdHorarioLaboral == request.IdHorarioLaboral, cancellationToken);
        if (horario is null) return null;
        horario.Activo = request.Activo;
        await _context.SaveChangesAsync(cancellationToken);
        return new HorarioLaboralDto
        {
            IdHorarioLaboral = horario.IdHorarioLaboral,
            Nombre = horario.Nombre,
            HoraEntrada = horario.HoraEntrada,
            HoraSalida = horario.HoraSalida,
            DiasLaborales = horario.DiasLaborales,
            Activo = horario.Activo
        };
    }
}
