using AsistenciaPyme.Application.Features.Empleados.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace AsistenciaPyme.Application.Features.Empleados.Commands;

public class CambiarEstadoEmpleadoCommand
    : IRequest<EmpleadoDto?>
{
    [JsonIgnore]
    public int IdEmpleado { get; set; }

    public bool Activo { get; set; }
}