using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.TiposDeduccion.DTOs;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.TiposDeduccion.Commands;

public class CrearTipoDeduccionHandler
    : IRequestHandler<
        CrearTipoDeduccionCommand,
        TipoDeduccionDto>
{
    private readonly IAsistenciaPymeDbContext _context;

    public CrearTipoDeduccionHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<TipoDeduccionDto> Handle(
        CrearTipoDeduccionCommand request,
        CancellationToken cancellationToken)
    {
        string nombre = request.Nombre.Trim();

        bool nombreDuplicado = await _context.TiposDeduccion
            .AnyAsync(
                tipo =>
                    tipo.Nombre.ToLower() ==
                    nombre.ToLower(),
                cancellationToken);

        if (nombreDuplicado)
        {
            throw new InvalidOperationException(
                "Ya existe un tipo de deducción con ese nombre.");
        }

        var tipoDeduccion = new TipoDeduccion
        {
            Nombre = nombre,

            Descripcion =
                string.IsNullOrWhiteSpace(request.Descripcion)
                    ? null
                    : request.Descripcion.Trim(),

            TipoCalculo = request.TipoCalculo,
            ValorPredeterminado = request.ValorPredeterminado,
            Activo = true,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = null
        };

        await _context.TiposDeduccion.AddAsync(
            tipoDeduccion,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

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
            FechaCreacion = tipoDeduccion.FechaCreacion,
            FechaActualizacion =
                tipoDeduccion.FechaActualizacion
        };
    }
}