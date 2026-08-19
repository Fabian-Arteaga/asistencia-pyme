using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AsistenciaPyme.Application.Feautures.Departamentos.Commands;
public class CrearDepartamentoHandler : IRequestHandler<CrearDepartamentoCommand, int>
{
    private readonly IAsistenciaPymeDbContext _context;
    public CrearDepartamentoHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }
    public async Task<int> Handle(CrearDepartamentoCommand request, CancellationToken cancellationToken)
    {
        string nombre = request.Nombre.Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new InvalidOperationException("El nombre del departamento es obligatorio.");
        }
        bool yaExiste = await _context.Departamentos.AnyAsync(d => d.Nombre.ToLower() == nombre.ToLower(), cancellationToken);
        if (yaExiste)
        {
            throw new InvalidOperationException("Ya existe un departamento con ese nombre.");
        }
        var departamento = new Departamento
        {
            Nombre = nombre,
            Descripcion = string.IsNullOrWhiteSpace(request.Descripcion) ? null : request.Descripcion.Trim(),
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };
        await _context.Departamentos.AddAsync(departamento, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return departamento.IdDepartamento;
    }
}
