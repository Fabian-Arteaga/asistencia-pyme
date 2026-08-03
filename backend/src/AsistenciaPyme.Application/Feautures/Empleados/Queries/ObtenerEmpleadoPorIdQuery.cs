using AsistenciaPyme.Application.Features.Empleados.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Empleados.Queries;

public class ObtenerEmpleadoPorIdQuery
    : IRequest<EmpleadoDto?>
{
    public int IdEmpleado { get; set; }
}