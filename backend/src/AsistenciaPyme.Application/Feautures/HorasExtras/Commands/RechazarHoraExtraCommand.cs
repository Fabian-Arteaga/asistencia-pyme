using AsistenciaPyme.Application.Feautures.HorasExtras.DTOs;
using MediatR;
using System.Text.Json.Serialization;
namespace AsistenciaPyme.Application.Feautures.HorasExtras.Commands;
public class RechazarHoraExtraCommand : IRequest<HoraExtraDto?>
{
    [JsonIgnore]
    public int IdHoraExtra { get; set; }
    public string? Observacion { get; set; }
}
