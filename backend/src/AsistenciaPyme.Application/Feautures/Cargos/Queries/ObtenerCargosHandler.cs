using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.Cargos.DTOs;
using AsistenciaPyme.Application.Feautures.Cargos.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Cargos.Queries;

public class ObtenerCargosHandler
    : IRequestHandler<ObtenerCargosQuery, List<CargoDTO>>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerCargosHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<List<CargoDTO>> Handle(
        ObtenerCargosQuery request,
        CancellationToken cancellationToken)
    {
        List<CargoDTO> cargos = await _context.Cargos
            .AsNoTracking()
            .OrderBy(cargo => cargo.Nombre)
            .Select(cargo => new CargoDTO   
            {
                IdCargo = cargo.IdCargo,
                Nombre = cargo.Nombre,
                Descripcion = cargo.Descripcion,
                Funciones = cargo.Funciones,
                Responsabilidades = cargo.Responsabilidades,
                Activo = cargo.Activo,
                FechaCreacion = cargo.FechaCreacion,
                FechaActualizacion = cargo.FechaActualizacion
            })
            .ToListAsync(cancellationToken);

        return cargos;
    }
}