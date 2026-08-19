namespace AsistenciaPyme.Application.Feautures.Indemnizacion.DTOs;

public class IndemnizacionEmpleadoDto
{
    public int IdEmpleado { get; set; }
    public string? NombreEmpleado { get; set; }
    public bool PuedeCalcularse { get; set; }
    public decimal? Monto { get; set; }
    public string? Mensaje { get; set; }
}
