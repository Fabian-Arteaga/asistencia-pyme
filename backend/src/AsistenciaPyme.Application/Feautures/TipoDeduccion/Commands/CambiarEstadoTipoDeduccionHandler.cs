using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.TiposDeduccion.DTOs;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.TiposDeduccion.Commands;

public class CambiarEstadoTipoDeduccionHandler
    : IRequestHandler<
        CambiarEstadoTipoDeduccionCommand,
        TipoDeduccionDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public CambiarEstadoTipoDeduccionHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<TipoDeduccionDto?> Handle(
        CambiarEstadoTipoDeduccionCommand request,
        CancellationToken cancellationToken)
    {
        TipoDeduccion? tipoDeduccion =
            await _context.TiposDeduccion
                .FirstOrDefaultAsync(
                    tipo =>
                        tipo.IdTipoDeduccion ==
                        request.IdTipoDeduccion,
                    cancellationToken);

        if (tipoDeduccion is null)
        {
            return null;
        }

        tipoDeduccion.Activo = request.Activo;
        tipoDeduccion.FechaActualizacion =
            DateTime.UtcNow;

        await _context.SaveChangesAsync(
            cancellationToken);

        return new TipoDeduccionDto
        {
            IdTipoDeduccion =
                tipoDeduccion.IdTipoDeduccion,

            Nombre = tipoDeduccion.Nombre,
            Descripcion = tipoDeduccion.Descripcion,
            TipoCalculo = tipoDeduccion.TipoCalculo,

            ValorPredeterminado =
                tipoDeduccion.ValorPredeterminado,

            Activo = tipoDeduccion.Activo,
            FechaCreacion =
                tipoDeduccion.FechaCreacion,

            FechaActualizacion =
                tipoDeduccion.FechaActualizacion
        };
    }
}