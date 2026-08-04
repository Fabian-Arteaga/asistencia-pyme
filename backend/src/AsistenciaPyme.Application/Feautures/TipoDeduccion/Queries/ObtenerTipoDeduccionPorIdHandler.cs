using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.TiposDeduccion.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.TiposDeduccion.Queries;

public class ObtenerTipoDeduccionPorIdHandler
    : IRequestHandler<
        ObtenerTipoDeduccionPorIdQuery,
        TipoDeduccionDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerTipoDeduccionPorIdHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<TipoDeduccionDto?> Handle(
        ObtenerTipoDeduccionPorIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.TiposDeduccion
            .AsNoTracking()
            .Where(tipo =>
                tipo.IdTipoDeduccion ==
                request.IdTipoDeduccion)
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}