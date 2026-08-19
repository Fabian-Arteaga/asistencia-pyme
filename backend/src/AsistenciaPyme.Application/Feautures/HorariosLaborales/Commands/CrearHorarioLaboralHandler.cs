using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AsistenciaPyme.Application.Feautures.HorariosLaborales.Commands;
public class CrearHorarioLaboralHandler : IRequestHandler<CrearHorarioLaboralCommand, int>
{
    private readonly IAsistenciaPymeDbContext _context;
    public CrearHorarioLaboralHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }
    public async Task<int> Handle(CrearHorarioLaboralCommand request, CancellationToken cancellationToken)
    {
        string nombre = request.Nombre.Trim();
        if (string.IsNullOrWhiteSpace(nombre)) throw new InvalidOperationException("El nombre del horario es obligatorio.");
        bool existe = await _context.HorariosLaborales.AnyAsync(h => h.Nombre.ToLower() == nombre.ToLower(), cancellationToken);
        if (existe) throw new InvalidOperationException("Ya existe un horario laboral con ese nombre.");
        var horario = new HorarioLaboral
        {
            Nombre = nombre,
            HoraEntrada = request.HoraEntrada,
            HoraSalida = request.HoraSalida,
            DiasLaborales = request.DiasLaborales,
            Activo = true
        };
        await _context.HorariosLaborales.AddAsync(horario, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return horario.IdHorarioLaboral;
    }
}
