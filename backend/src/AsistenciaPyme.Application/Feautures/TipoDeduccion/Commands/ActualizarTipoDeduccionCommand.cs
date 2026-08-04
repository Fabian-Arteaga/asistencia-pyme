using AsistenciaPyme.Application.Features.TiposDeduccion.DTOs;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using System.Text.Json.Serialization;

namespace AsistenciaPyme.Application.Features.TiposDeduccion.Commands;

public class ActualizarTipoDeduccionCommand
    : IRequest<TipoDeduccionDto?>
{
    [JsonIgnore]
    public int IdTipoDeduccion { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public TipoCalculoDeduccion TipoCalculo { get; set; }

    public decimal? ValorPredeterminado { get; set; }
}