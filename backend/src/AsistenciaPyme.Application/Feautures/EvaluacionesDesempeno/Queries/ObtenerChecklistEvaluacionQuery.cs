using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.Queries;

public class ObtenerChecklistEvaluacionQuery : IRequest<List<CategoriaEvaluacionDto>>
{
}

public class ObtenerChecklistEvaluacionHandler : IRequestHandler<ObtenerChecklistEvaluacionQuery, List<CategoriaEvaluacionDto>>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerChecklistEvaluacionHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoriaEvaluacionDto>> Handle(ObtenerChecklistEvaluacionQuery request, CancellationToken cancellationToken)
    {
        var categorias = await _context.CategoriasEvaluacion
            .Include(c => c.Criterios)
            .Where(c => c.Activo)
            .OrderBy(c => c.Orden)
            .Select(c => new CategoriaEvaluacionDto
            {
                IdCategoriaEvaluacion = c.IdCategoriaEvaluacion,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                Ponderacion = c.Ponderacion,
                Orden = c.Orden,
                Activo = c.Activo,
                Criterios = c.Criterios
                    .Where(cr => cr.Activo)
                    .OrderBy(cr => cr.Orden)
                    .Select(cr => new CriterioEvaluacionDto
                    {
                        IdCriterioEvaluacion = cr.IdCriterioEvaluacion,
                        IdCategoriaEvaluacion = cr.IdCategoriaEvaluacion,
                        Texto = cr.Texto,
                        Descripcion = cr.Descripcion,
                        Orden = cr.Orden,
                        Activo = cr.Activo
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return categorias;
    }
}
