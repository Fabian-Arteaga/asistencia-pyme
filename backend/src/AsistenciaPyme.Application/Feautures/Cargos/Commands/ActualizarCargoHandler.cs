using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.Cargos.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Cargos.Commands;

public class ActualizarCargoHandler
    : IRequestHandler<ActualizarCargoCommand, CargoDTO?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ActualizarCargoHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<CargoDTO?> Handle(
        ActualizarCargoCommand request,
        CancellationToken cancellationToken)
    {
        var cargo = await _context.Cargos
            .FirstOrDefaultAsync(
                c => c.IdCargo == request.IdCargo,
                cancellationToken);

        if (cargo is null)
        {
            return null;
        }

        string nombre = request.Nombre.Trim();

        bool nombreDuplicado = await _context.Cargos
            .AnyAsync(
                c => c.IdCargo != request.IdCargo
                     && c.Nombre.ToLower() == nombre.ToLower(),
                cancellationToken);

        if (nombreDuplicado)
        {
            throw new InvalidOperationException(
                "Ya existe otro cargo con ese nombre.");
        }

        cargo.Nombre = nombre;
        cargo.Descripcion = request.Descripcion?.Trim();
        cargo.Funciones = request.Funciones.Trim();
        cargo.Responsabilidades = request.Responsabilidades.Trim();
        cargo.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new CargoDTO
        {
            IdCargo = cargo.IdCargo,
            Nombre = cargo.Nombre,
            Descripcion = cargo.Descripcion,
            Funciones = cargo.Funciones,
            Responsabilidades = cargo.Responsabilidades,
            Activo = cargo.Activo,
            FechaCreacion = cargo.FechaCreacion,
            FechaActualizacion = cargo.FechaActualizacion
        };
    }
}