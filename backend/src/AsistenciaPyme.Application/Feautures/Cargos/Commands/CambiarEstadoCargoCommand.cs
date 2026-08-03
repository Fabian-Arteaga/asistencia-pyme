using AsistenciaPyme.Application.Feautures.Cargos.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace AsistenciaPyme.Application.Features.Cargos.Commands;

public class CambiarEstadoCargoCommand : IRequest<CargoDTO?>
{
    [JsonIgnore]
    public int IdCargo { get; set; }

    public bool Activo { get; set; }
}