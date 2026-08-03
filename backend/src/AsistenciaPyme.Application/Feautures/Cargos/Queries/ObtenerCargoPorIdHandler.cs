using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.Cargos.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Feautures.Cargos.Queries
{
    public class ObtenerCargoPorIdHandler
    : IRequestHandler<ObtenerCargoPorIdQuery, CargoDTO?>
    {
        private readonly IAsistenciaPymeDbContext _context;

        public ObtenerCargoPorIdHandler(
            IAsistenciaPymeDbContext context)
        {
            _context = context;
        }

        public async Task<CargoDTO?> Handle(
            ObtenerCargoPorIdQuery request,
            CancellationToken cancellationToken)
        {
            CargoDTO? cargo = await _context.Cargos
                .AsNoTracking()
                .Where(c => c.IdCargo == request.IdCargo)
                .Select(c => new CargoDTO
                {
                    IdCargo = c.IdCargo,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    Funciones = c.Funciones,
                    Responsabilidades = c.Responsabilidades,
                    Activo = c.Activo,
                    FechaCreacion = c.FechaCreacion,
                    FechaActualizacion = c.FechaActualizacion
                })
                .FirstOrDefaultAsync(cancellationToken);

            return cargo;
        }
    }
}
