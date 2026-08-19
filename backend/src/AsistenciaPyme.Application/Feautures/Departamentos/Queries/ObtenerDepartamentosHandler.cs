using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.Departamentos.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AsistenciaPyme.Application.Feautures.Departamentos.Queries;
public class ObtenerDepartamentosHandler : IRequestHandler<ObtenerDepartamentosQuery, List<DepartamentoDto>>
{
    private readonly IAsistenciaPymeDbContext _context;
    public ObtenerDepartamentosHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }
    public async Task<List<DepartamentoDto>> Handle(ObtenerDepartamentosQuery request, CancellationToken cancellationToken)
    {
        return await _context.Departamentos
            .AsNoTracking()
            .OrderBy(d => d.Nombre)
            .Select(d => new DepartamentoDto
            {
                IdDepartamento = d.IdDepartamento,
                Nombre = d.Nombre,
                Descripcion = d.Descripcion,
                Activo = d.Activo,
                FechaCreacion = d.FechaCreacion,
                FechaActualizacion = d.FechaActualizacion
            })
            .ToListAsync(cancellationToken);
    }
}
