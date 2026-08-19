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
            .Include(e => e.Departamento)
            .Include(e => e.HorarioLaboral)
            .Where(e => e.IdEmpleado == request.IdEmpleado)
            .Select(e => new EmpleadoDto
            {
                IdEmpleado = e.IdEmpleado,
                IdCargo = e.IdCargo,
                IdDepartamento = e.IdDepartamento,
                IdHorarioLaboral = e.IdHorarioLaboral,
                NombreCargo = e.Cargo.Nombre,
                NombreDepartamento = e.Departamento != null ? e.Departamento.Nombre : null,
                NombreHorarioLaboral = e.HorarioLaboral != null ? e.HorarioLaboral.Nombre : null,
                CodigoEmpleado = e.CodigoEmpleado,
                Identificacion = e.Identificacion,
                NumeroINSS = e.NumeroINSS,
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