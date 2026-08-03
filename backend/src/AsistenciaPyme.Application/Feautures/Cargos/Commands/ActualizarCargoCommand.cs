using AsistenciaPyme.Application.Feautures.Cargos.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace AsistenciaPyme.Application.Features.Cargos.Commands;

public class ActualizarCargoCommand : IRequest<CargoDTO?>
{
    [JsonIgnore]
    public int IdCargo { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public string Funciones { get; set; } = string.Empty;

    public string Responsabilidades { get; set; } = string.Empty;
}