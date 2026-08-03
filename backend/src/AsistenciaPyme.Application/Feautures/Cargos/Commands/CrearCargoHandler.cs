using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Feautures.Cargos.Commands
{
    public class CrearCargoHandler : IRequestHandler<CrearCargoCommand, int>
    {
        private readonly IAsistenciaPymeDbContext _context;

        public CrearCargoHandler(IAsistenciaPymeDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(
            CrearCargoCommand request,
            CancellationToken cancellationToken)
        {
            string nombre = request.Nombre.Trim();

            bool existeCargo = await _context.Cargos
                .AnyAsync(
                    cargo => cargo.Nombre.ToLower() == nombre.ToLower(),
                    cancellationToken);

            if (existeCargo)
            {
                throw new InvalidOperationException(
                    "Ya existe un cargo con ese nombre.");
            }

            var cargo = new Cargo
            {
                Nombre = nombre,
                Descripcion = request.Descripcion?.Trim(),
                Funciones = request.Funciones.Trim(),
                Responsabilidades = request.Responsabilidades.Trim(),
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            await _context.Cargos.AddAsync(cargo, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return cargo.IdCargo;
        }
    }
}

