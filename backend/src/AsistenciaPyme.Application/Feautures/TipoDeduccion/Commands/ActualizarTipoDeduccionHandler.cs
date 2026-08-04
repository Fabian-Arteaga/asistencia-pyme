using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.TiposDeduccion.DTOs;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.TiposDeduccion.Commands;

public class ActualizarTipoDeduccionHandler
    : IRequestHandler<
        ActualizarTipoDeduccionCommand,
        TipoDeduccionDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ActualizarTipoDeduccionHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<TipoDeduccionDto?> Handle(
        ActualizarTipoDeduccionCommand request,
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

        string nombre = request.Nombre.Trim();

        bool nombreDuplicado = await _context.TiposDeduccion
            .AnyAsync(
                tipo =>
                    tipo.IdTipoDeduccion !=
                        request.IdTipoDeduccion &&
                    tipo.Nombre.ToLower() ==
                        nombre.ToLower(),
                cancellationToken);

        if (nombreDuplicado)
        {
            throw new InvalidOperationException(
                "Ya existe otro tipo de deducción con ese nombre.");
        }

        tipoDeduccion.Nombre = nombre;

        tipoDeduccion.Descripcion =
            string.IsNullOrWhiteSpace(request.Descripcion)
                ? null
                : request.Descripcion.Trim();

        tipoDeduccion.TipoCalculo =
            request.TipoCalculo;

        tipoDeduccion.ValorPredeterminado =
            request.ValorPredeterminado;

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