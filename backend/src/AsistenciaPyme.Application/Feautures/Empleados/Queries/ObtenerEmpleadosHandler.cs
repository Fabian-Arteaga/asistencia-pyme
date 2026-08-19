using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Empleados.DTOs;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Empleados.Queries;

public class ObtenerEmpleadosHandler
    : IRequestHandler<ObtenerEmpleadosQuery, List<EmpleadoDto>>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerEmpleadosHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmpleadoDto>> Handle(
        ObtenerEmpleadosQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Empleados
            .AsNoTracking()
            .Include(e => e.Departamento)
            .Include(e => e.HorarioLaboral)
            .Include(e => e.JefeDirecto)
            .AsQueryable();

        if (request.IdDepartamento.HasValue)
        {
            query = query.Where(e => e.IdDepartamento == request.IdDepartamento.Value);
        }

        if (request.SoloActivos.HasValue && request.SoloActivos.Value)
        {
            query = query.Where(e => e.Estado == EstadoEmpleado.Activo);
        }

        return await query
            .OrderBy(e => e.Nombres)
            .ThenBy(e => e.Apellidos)
            .Select(e => new EmpleadoDto
            {
                IdEmpleado = e.IdEmpleado,
                IdCargo = e.IdCargo,
                IdDepartamento = e.IdDepartamento,
                IdHorarioLaboral = e.IdHorarioLaboral,
                IdJefeDirecto = e.IdJefeDirecto,
                NombreCargo = e.Cargo.Nombre,
                NombreDepartamento = e.Departamento != null ? e.Departamento.Nombre : null,
                NombreHorarioLaboral = e.HorarioLaboral != null ? e.HorarioLaboral.Nombre : null,
                NombreJefeDirecto = e.JefeDirecto != null ? (e.JefeDirecto.Nombres + " " + e.JefeDirecto.Apellidos).Trim() : null,
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
            .ToListAsync(cancellationToken);
    }
}