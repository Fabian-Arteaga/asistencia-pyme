namespace AsistenciaPyme.Application.Features.Vacaciones.DTOs;

public class VacacionDto
{
    public int IdVacacion { get; set; }

    public int IdEmpleado { get; set; }

    public string CodigoEmpleado { get; set; } = string.Empty;

    public string NombreEmpleado { get; set; } = string.Empty;

    public int IdAdministrador { get; set; }

    public string NombreAdministrador { get; set; } = string.Empty;

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public string? Motivo { get; set; }

    public string? Observacion { get; set; }

    public bool Cancelada { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }
}