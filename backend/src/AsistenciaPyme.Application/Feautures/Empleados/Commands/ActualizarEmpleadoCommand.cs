using AsistenciaPyme.Application.Features.Empleados.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace AsistenciaPyme.Application.Features.Empleados.Commands;

public class ActualizarEmpleadoCommand
    : IRequest<EmpleadoDto?>
{
    [JsonIgnore]
    public int IdEmpleado { get; set; }

    public int IdCargo { get; set; }

    public int? IdDepartamento { get; set; }

    public int? IdHorarioLaboral { get; set; }

    public string CodigoEmpleado { get; set; } = string.Empty;

    public string Identificacion { get; set; } = string.Empty;

    public string? NumeroINSS { get; set; }

    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public string? Direccion { get; set; }

    public DateOnly FechaContratacion { get; set; }

    public decimal SalarioBase { get; set; }
}