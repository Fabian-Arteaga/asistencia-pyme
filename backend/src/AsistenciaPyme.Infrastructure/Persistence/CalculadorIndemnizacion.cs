using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.Indemnizacion.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Infrastructure.Persistence;

public class CalculadorIndemnizacion : ICalculadorIndemnizacion
{
    private readonly AsistenciaPymeDbContext _context;

    public CalculadorIndemnizacion(AsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<IndemnizacionEmpleadoDto> CalcularAsync(int idEmpleado, CancellationToken cancellationToken = default)
    {
        var empleado = await _context.Empleados
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.IdEmpleado == idEmpleado, cancellationToken);

        if (empleado is null)
        {
            throw new InvalidOperationException("El empleado no existe.");
        }

        var configuracion = await _context.ConfiguracionesNomina
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (configuracion is null)
        {
            return new IndemnizacionEmpleadoDto
            {
                IdEmpleado = empleado.IdEmpleado,
                NombreEmpleado = $"{empleado.Nombres} {empleado.Apellidos}".Trim(),
                PuedeCalcularse = false,
                Mensaje = "Todavía no existe una política de indemnización configurada para calcular una proyección."
            };
        }

        return new IndemnizacionEmpleadoDto
        {
            IdEmpleado = empleado.IdEmpleado,
            NombreEmpleado = $"{empleado.Nombres} {empleado.Apellidos}".Trim(),
            PuedeCalcularse = false,
            Mensaje = "La política de indemnización no está configurada en esta implementación."
        };
    }
}
