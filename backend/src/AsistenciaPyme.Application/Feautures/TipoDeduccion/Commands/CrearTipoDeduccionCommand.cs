using AsistenciaPyme.Application.Features.TiposDeduccion.DTOs;
using AsistenciaPyme.Domain.Enums;
using MediatR;

namespace AsistenciaPyme.Application.Features.TiposDeduccion.Commands;

public class CrearTipoDeduccionCommand
    : IRequest<TipoDeduccionDto>
{
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public TipoCalculoDeduccion TipoCalculo { get; set; }

    public decimal? ValorPredeterminado { get; set; }
}