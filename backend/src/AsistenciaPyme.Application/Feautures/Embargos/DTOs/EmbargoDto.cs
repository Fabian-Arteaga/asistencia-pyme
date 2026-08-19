namespace AsistenciaPyme.Application.Feautures.Embargos.DTOs;
public class EmbargoDto
{
    public int IdEmbargo { get; set; }
    public int IdEmpleado { get; set; }
    public string? NombreEmpleado { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int TipoCalculo { get; set; }
    public decimal? Monto { get; set; }
    public decimal? Porcentaje { get; set; }
    public decimal SaldoPendiente { get; set; }
    public bool Activo { get; set; }
    public string? Referencia { get; set; }
    public string? Observacion { get; set; }
}
