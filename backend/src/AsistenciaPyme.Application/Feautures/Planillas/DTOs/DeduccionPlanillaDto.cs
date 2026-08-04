using AsistenciaPyme.Domain.Enums;

namespace AsistenciaPyme.Application.Features.Planillas.DTOs;

public class DeduccionPlanillaDto
{
    public int IdDeduccionPlanilla { get; set; }

    public int IdTipoDeduccion { get; set; }

    public string NombreTipoDeduccion { get; set; } =
        string.Empty;

    public TipoCalculoDeduccion TipoCalculo { get; set; }

    public decimal ValorAplicado { get; set; }

    public decimal MontoCalculado { get; set; }

    public string? Observacion { get; set; }
}