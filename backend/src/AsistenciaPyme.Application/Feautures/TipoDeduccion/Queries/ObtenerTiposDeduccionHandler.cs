using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.TiposDeduccion.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.TiposDeduccion.Queries;

public class ObtenerTiposDeduccionHandler
    : IRequestHandler<
        ObtenerTiposDeduccionQuery,
        List<TipoDeduccionDto>>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerTiposDeduccionHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<List<TipoDeduccionDto>> Handle(
        ObtenerTiposDeduccionQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.TiposDeduccion
            .AsNoTracking()
            .OrderBy(tipo => tipo.Nombre)
            .Select(tipo => new TipoDeduccionDto
            {
                IdTipoDeduccion =
                    tipo.IdTipoDeduccion,

                Nombre = tipo.Nombre,
                Descripcion = tipo.Descripcion,
                TipoCalculo = tipo.TipoCalculo,

                ValorPredeterminado =
                    tipo.ValorPredeterminado,

                Activo = tipo.Activo,
                FechaCreacion = tipo.FechaCreacion,
                FechaActualizacion =
                    tipo.FechaActualizacion
            })
            .ToListAsync(cancellationToken);
    }
}