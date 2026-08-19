using MediatR;

namespace AsistenciaPyme.Application.Features.Empleados.Commands;

public class CrearEmpleadoCommand : IRequest<int>
{
    public int IdCargo { get; set; }

    public int? IdDepartamento { get; set; }

    public int? IdHorarioLaboral { get; set; }

    public int? IdJefeDirecto { get; set; }

    public string CodigoEmpleado { get; set; } = string.Empty;

    public string Pin { get; set; } = string.Empty;

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