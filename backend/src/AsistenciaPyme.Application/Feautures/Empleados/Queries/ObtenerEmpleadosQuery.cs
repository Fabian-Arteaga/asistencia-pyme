using AsistenciaPyme.Application.Features.Empleados.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Empleados.Queries;

public class ObtenerEmpleadosQuery
    : IRequest<List<EmpleadoDto>>
{
    public int? IdDepartamento { get; set; }

    public bool? SoloActivos { get; set; }
}