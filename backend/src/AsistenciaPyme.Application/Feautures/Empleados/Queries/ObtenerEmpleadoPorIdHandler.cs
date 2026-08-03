using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Empleados.DTOs;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Empleados.Queries;

public class ObtenerEmpleadoPorIdHandler
    : IRequestHandler<ObtenerEmpleadoPorIdQuery, EmpleadoDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerEmpleadoPorIdHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<EmpleadoDto?> Handle(
        ObtenerEmpleadoPorIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Empleados
            .AsNoTracking()
            .Where(e => e.IdEmpleado == request.IdEmpleado)
            .Select(e => new EmpleadoDto
            {
                IdEmpleado = e.IdEmpleado,
                IdCargo = e.IdCargo,
                NombreCargo = e.Cargo.Nombre,
                CodigoEmpleado = e.CodigoEmpleado,
                Identificacion = e.Identificacion,
                Nombres = e.Nombres,
                Apellidos = e.Apellidos,
                Telefono = e.Telefono,
                Correo = e.Correo,
                Direccion = e.Direccion,
                FechaContratacion = e.FechaContratacion,
                SalarioBase = e.SalarioBase,
                Activo = e.Estado == EstadoEmpleado.Activo,
                FechaCreacion = e.FechaCreacion,
                FechaActualizacion = e.FechaActualizacion
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}