using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.DispositivosMarcaje.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Feautures.DispositivosMarcaje.Queries;

public class ObtenerDispositivosMarcajeHandler : IRequestHandler<ObtenerDispositivosMarcajeQuery, List<DispositivoMarcajeDto>>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerDispositivosMarcajeHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<List<DispositivoMarcajeDto>> Handle(ObtenerDispositivosMarcajeQuery request, CancellationToken cancellationToken)
    {
        return await _context.DispositivosMarcaje
            .AsNoTracking()
            .Include(d => d.Departamento)
            .OrderByDescending(d => d.IdDispositivoMarcaje)
            .Select(d => new DispositivoMarcajeDto
            {
                IdDispositivoMarcaje = d.IdDispositivoMarcaje,
                Nombre = d.Nombre,
                Tipo = (int)d.Tipo,
                IdDepartamento = d.IdDepartamento,
                NombreDepartamento = d.Departamento != null ? d.Departamento.Nombre : string.Empty,
                Identificador = d.Identificador,
                Activo = d.Activo
            })
            .ToListAsync(cancellationToken);
    }
}
