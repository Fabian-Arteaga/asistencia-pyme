namespace AsistenciaPyme.Application.Features.Planillas.DTOs;

public class DeduccionAplicarDto
{
    public int IdTipoDeduccion { get; set; }

    public decimal? ValorAplicado { get; set; }

    public string? Observacion { get; set; }
}