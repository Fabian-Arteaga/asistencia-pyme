using AsistenciaPyme.Application.Feautures.Embargos.DTOs;
using MediatR;
using System.Text.Json.Serialization;
namespace AsistenciaPyme.Application.Feautures.Embargos.Commands;
public class CambiarEstadoEmbargoCommand : IRequest<EmbargoDto?>
{
    [JsonIgnore]
    public int IdEmbargo { get; set; }
    public bool Activo { get; set; }
}
