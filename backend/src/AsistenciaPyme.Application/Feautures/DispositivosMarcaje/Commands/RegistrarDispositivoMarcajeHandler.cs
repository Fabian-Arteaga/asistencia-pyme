using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Feautures.DispositivosMarcaje.Commands;

public class RegistrarDispositivoMarcajeHandler : IRequestHandler<RegistrarDispositivoMarcajeCommand, int>
{
    private readonly IAsistenciaPymeDbContext _context;

    public RegistrarDispositivoMarcajeHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(RegistrarDispositivoMarcajeCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            throw new InvalidOperationException("El nombre del dispositivo es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(request.Identificador))
        {
            throw new InvalidOperationException("El identificador del dispositivo es obligatorio.");
        }

        if (request.Tipo is not (int)TipoDispositivoMarcaje.KioscoPrincipal and not (int)TipoDispositivoMarcaje.TerminalDepartamento)
        {
            throw new InvalidOperationException("El tipo de dispositivo no es válido.");
        }

        if (request.Tipo == (int)TipoDispositivoMarcaje.TerminalDepartamento && request.IdDepartamento is null)
        {
            throw new InvalidOperationException("Un terminal de departamento debe indicar su departamento.");
        }

        if (request.IdDepartamento is not null)
        {
            var departamento = await _context.Departamentos
                .AnyAsync(d => d.IdDepartamento == request.IdDepartamento && d.Activo, cancellationToken);

            if (!departamento)
            {
                throw new InvalidOperationException("El departamento indicado no existe o está inactivo.");
            }
        }

        var dispositivo = new DispositivoMarcaje
        {
            Nombre = request.Nombre,
            Tipo = (TipoDispositivoMarcaje)request.Tipo,
            IdDepartamento = request.IdDepartamento,
            Identificador = request.Identificador,
            Activo = true
        };

        await _context.DispositivosMarcaje.AddAsync(dispositivo, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return dispositivo.IdDispositivoMarcaje;
    }
}
