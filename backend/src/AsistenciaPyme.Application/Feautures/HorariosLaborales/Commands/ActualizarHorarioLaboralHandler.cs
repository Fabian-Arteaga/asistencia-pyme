using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.HorariosLaborales.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AsistenciaPyme.Application.Feautures.HorariosLaborales.Commands;
public class ActualizarHorarioLaboralHandler : IRequestHandler<ActualizarHorarioLaboralCommand, HorarioLaboralDto?>
{
    private readonly IAsistenciaPymeDbContext _context;
    public ActualizarHorarioLaboralHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }
    public async Task<HorarioLaboralDto?> Handle(ActualizarHorarioLaboralCommand request, CancellationToken cancellationToken)
    {
        var horario = await _context.HorariosLaborales.FirstOrDefaultAsync(h => h.IdHorarioLaboral == request.IdHorarioLaboral, cancellationToken);
        if (horario is null) return null;
        string nombre = request.Nombre.Trim();
        if (string.IsNullOrWhiteSpace(nombre)) throw new InvalidOperationException("El nombre del horario es obligatorio.");
        bool nombreDuplicado = await _context.HorariosLaborales.AnyAsync(h => h.IdHorarioLaboral != request.IdHorarioLaboral && h.Nombre.ToLower() == nombre.ToLower(), cancellationToken);
        if (nombreDuplicado) throw new InvalidOperationException("Ya existe otro horario laboral con ese nombre.");
        horario.Nombre = nombre;
        horario.HoraEntrada = request.HoraEntrada;
        horario.HoraSalida = request.HoraSalida;
        horario.DiasLaborales = request.DiasLaborales;
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
