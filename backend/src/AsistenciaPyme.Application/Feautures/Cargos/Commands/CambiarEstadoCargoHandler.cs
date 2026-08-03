using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.Cargos.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Cargos.Commands;

public class CambiarEstadoCargoHandler
    : IRequestHandler<CambiarEstadoCargoCommand, CargoDTO?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public CambiarEstadoCargoHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<CargoDTO?> Handle(
        CambiarEstadoCargoCommand request,
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

        cargo.Activo = request.Activo;
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