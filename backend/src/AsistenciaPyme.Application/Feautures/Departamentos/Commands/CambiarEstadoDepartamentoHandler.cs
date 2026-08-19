using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.Departamentos.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AsistenciaPyme.Application.Feautures.Departamentos.Commands;
public class CambiarEstadoDepartamentoHandler : IRequestHandler<CambiarEstadoDepartamentoCommand, DepartamentoDto?>
{
    private readonly IAsistenciaPymeDbContext _context;
    public CambiarEstadoDepartamentoHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }
    public async Task<DepartamentoDto?> Handle(CambiarEstadoDepartamentoCommand request, CancellationToken cancellationToken)
    {
        var departamento = await _context.Departamentos.FirstOrDefaultAsync(d => d.IdDepartamento == request.IdDepartamento, cancellationToken);
        if (departamento is null)
        {
            return null;
        }
        departamento.Activo = request.Activo;
        departamento.FechaActualizacion = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return new DepartamentoDto
        {
            IdDepartamento = departamento.IdDepartamento,
            Nombre = departamento.Nombre,
            Descripcion = departamento.Descripcion,
            Activo = departamento.Activo,
            FechaCreacion = departamento.FechaCreacion,
            FechaActualizacion = departamento.FechaActualizacion
        };
    }
}
