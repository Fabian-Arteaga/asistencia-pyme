using AsistenciaPyme.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.Commands;

public class PonderacionItemDto
{
    public int IdCategoriaEvaluacion { get; set; }
    public decimal Ponderacion { get; set; }
}

public class ConfigurarPonderacionesCommand : IRequest<bool>
{
    public List<PonderacionItemDto> Ponderaciones { get; set; } = new();
}

public class ConfigurarPonderacionesHandler : IRequestHandler<ConfigurarPonderacionesCommand, bool>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ConfigurarPonderacionesHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ConfigurarPonderacionesCommand request, CancellationToken cancellationToken)
    {
        if (request.Ponderaciones == null || !request.Ponderaciones.Any())
        {
            throw new InvalidOperationException("Debe ingresar las ponderaciones para las categorías.");
        }

        // Validar que ninguna sea negativa o cero
        if (request.Ponderaciones.Any(p => p.Ponderacion <= 0))
        {
            throw new InvalidOperationException("Cada categoría debe tener una ponderación mayor a cero.");
        }

        // Validar suma exacta de 100%
        decimal suma = request.Ponderaciones.Sum(p => p.Ponderacion);
        if (Math.Round(suma, 2) != 100.00m)
        {
            throw new InvalidOperationException($"La suma total de las ponderaciones debe ser exactamente 100%. Actualmente suma {suma}%.");
        }

        var categorias = await _context.CategoriasEvaluacion
            .Where(c => c.Activo)
            .ToListAsync(cancellationToken);

        foreach (var item in request.Ponderaciones)
        {
            var categoria = categorias.FirstOrDefault(c => c.IdCategoriaEvaluacion == item.IdCategoriaEvaluacion);
            if (categoria is null)
            {
                throw new InvalidOperationException($"No se encontró la categoría con ID {item.IdCategoriaEvaluacion}.");
            }

            categoria.Ponderacion = Math.Round(item.Ponderacion, 2);
            categoria.FechaActualizacion = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
