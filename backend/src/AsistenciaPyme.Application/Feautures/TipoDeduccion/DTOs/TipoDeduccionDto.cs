using AsistenciaPyme.Domain.Enums;

namespace AsistenciaPyme.Application.Features.TiposDeduccion.DTOs;

public class TipoDeduccionDto
{
    public int IdTipoDeduccion { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public TipoCalculoDeduccion TipoCalculo { get; set; }

    public decimal? ValorPredeterminado { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }
}