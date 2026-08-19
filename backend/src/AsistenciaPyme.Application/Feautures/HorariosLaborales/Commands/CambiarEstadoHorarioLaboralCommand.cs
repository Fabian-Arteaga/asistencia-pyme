using AsistenciaPyme.Application.Feautures.HorariosLaborales.DTOs;
using MediatR;
using System.Text.Json.Serialization;
namespace AsistenciaPyme.Application.Feautures.HorariosLaborales.Commands;
public class CambiarEstadoHorarioLaboralCommand : IRequest<HorarioLaboralDto?>
{
    [JsonIgnore]
    public int IdHorarioLaboral { get; set; }
    public bool Activo { get; set; }
}
