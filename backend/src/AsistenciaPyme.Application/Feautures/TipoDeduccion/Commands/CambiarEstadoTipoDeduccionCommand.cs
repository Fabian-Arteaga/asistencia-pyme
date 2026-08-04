using AsistenciaPyme.Application.Features.TiposDeduccion.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace AsistenciaPyme.Application.Features.TiposDeduccion.Commands;

public class CambiarEstadoTipoDeduccionCommand
    : IRequest<TipoDeduccionDto?>
{
    [JsonIgnore]
    public int IdTipoDeduccion { get; set; }

    public bool Activo { get; set; }
}