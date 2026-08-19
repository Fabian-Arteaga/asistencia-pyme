namespace AsistenciaPyme.Application.Features.Empleados.DTOs;

public class EmpleadoDto
{
    public int IdEmpleado { get; set; }

    public int IdCargo { get; set; }

    public int? IdDepartamento { get; set; }

    public int? IdHorarioLaboral { get; set; }

    public string? NombreDepartamento { get; set; }

    public string? NombreHorarioLaboral { get; set; }

    public string NombreCargo { get; set; } = string.Empty;

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

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }
}