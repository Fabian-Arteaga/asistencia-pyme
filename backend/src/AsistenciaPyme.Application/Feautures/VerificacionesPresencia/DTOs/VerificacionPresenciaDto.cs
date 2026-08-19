namespace AsistenciaPyme.Application.Feautures.VerificacionesPresencia.DTOs;

public class VerificacionPresenciaDto
{
    public int IdVerificacionPresencia { get; set; }
    public int IdEmpleado { get; set; }
    public int? IdAsistencia { get; set; }
    public int? IdDispositivoMarcaje { get; set; }
    public int? IdDepartamento { get; set; }
    public DateTime FechaHora { get; set; }
    public int Estado { get; set; }
    public string? EstadoNombre { get; set; }
    public string? Mensaje { get; set; }
}
