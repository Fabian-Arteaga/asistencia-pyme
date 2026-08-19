using AsistenciaPyme.Application.Feautures.HorariosLaborales.DTOs;
using MediatR;
using System.Text.Json.Serialization;
namespace AsistenciaPyme.Application.Feautures.HorariosLaborales.Commands;
public class ActualizarHorarioLaboralCommand : IRequest<HorarioLaboralDto?>
{
    [JsonIgnore]
    public int IdHorarioLaboral { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public TimeOnly HoraEntrada { get; set; }
    public TimeOnly HoraSalida { get; set; }
    public string DiasLaborales { get; set; } = string.Empty;
}
