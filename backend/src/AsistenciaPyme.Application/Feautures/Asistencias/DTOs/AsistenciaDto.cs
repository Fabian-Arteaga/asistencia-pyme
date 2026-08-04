namespace AsistenciaPyme.Application.Features.Asistencias.DTOs;

public class AsistenciaDto
{
    public int IdAsistencia { get; set; }

    public int IdEmpleado { get; set; }

    public string CodigoEmpleado { get; set; } = string.Empty;

    public string NombreEmpleado { get; set; } = string.Empty;

    public DateTime HoraEntrada { get; set; }

    public DateTime? HoraSalida { get; set; }

    public string? Observacion { get; set; }

    public bool Corregida { get; set; }

    public string? MotivoCorreccion { get; set; }

    public int? IdAdministrador { get; set; }

    public DateTime? FechaCorreccion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }
}