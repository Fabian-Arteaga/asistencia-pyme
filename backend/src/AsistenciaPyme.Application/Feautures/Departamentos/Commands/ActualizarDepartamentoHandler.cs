using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.Departamentos.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AsistenciaPyme.Application.Feautures.Departamentos.Commands;
public class ActualizarDepartamentoHandler : IRequestHandler<ActualizarDepartamentoCommand, DepartamentoDto?>
{
    private readonly IAsistenciaPymeDbContext _context;
    public ActualizarDepartamentoHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }
    public async Task<DepartamentoDto?> Handle(ActualizarDepartamentoCommand request, CancellationToken cancellationToken)
    {
        var departamento = await _context.Departamentos.FirstOrDefaultAsync(d => d.IdDepartamento == request.IdDepartamento, cancellationToken);
        if (departamento is null)
        {
            return null;
        }
        string nombre = request.Nombre.Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new InvalidOperationException("El nombre del departamento es obligatorio.");
        }
        bool nombreDuplicado = await _context.Departamentos.AnyAsync(d => d.IdDepartamento != request.IdDepartamento && d.Nombre.ToLower() == nombre.ToLower(), cancellationToken);
        if (nombreDuplicado)
        {
            throw new InvalidOperationException("Ya existe otro departamento con ese nombre.");
        }
        departamento.Nombre = nombre;
        departamento.Descripcion = string.IsNullOrWhiteSpace(request.Descripcion) ? null : request.Descripcion.Trim();
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
