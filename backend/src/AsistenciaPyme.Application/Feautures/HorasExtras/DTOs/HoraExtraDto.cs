namespace AsistenciaPyme.Application.Feautures.HorasExtras.DTOs;
public class HoraExtraDto
{
    public int IdHoraExtra { get; set; }
    public int IdEmpleado { get; set; }
    public string? NombreEmpleado { get; set; }
    public DateTime Fecha { get; set; }
    public int MinutosDetectados { get; set; }
    public int MinutosAprobados { get; set; }
    public int Estado { get; set; }
    public int? AprobadoPor { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string? Observacion { get; set; }
}
