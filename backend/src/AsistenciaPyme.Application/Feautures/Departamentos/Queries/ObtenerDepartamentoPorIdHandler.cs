using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.Departamentos.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AsistenciaPyme.Application.Feautures.Departamentos.Queries;
public class ObtenerDepartamentoPorIdHandler : IRequestHandler<ObtenerDepartamentoPorIdQuery, DepartamentoDto?>
{
    private readonly IAsistenciaPymeDbContext _context;
    public ObtenerDepartamentoPorIdHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }
    public async Task<DepartamentoDto?> Handle(ObtenerDepartamentoPorIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Departamentos
            .AsNoTracking()
            .Where(d => d.IdDepartamento == request.IdDepartamento)
            .Select(d => new DepartamentoDto
            {
                IdDepartamento = d.IdDepartamento,
                Nombre = d.Nombre,
                Descripcion = d.Descripcion,
                Activo = d.Activo,
                FechaCreacion = d.FechaCreacion,
                FechaActualizacion = d.FechaActualizacion
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
