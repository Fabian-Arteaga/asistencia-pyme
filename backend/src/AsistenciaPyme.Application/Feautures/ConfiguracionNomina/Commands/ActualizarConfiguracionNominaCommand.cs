using AsistenciaPyme.Application.Feautures.ConfiguracionNomina.DTOs;
using MediatR;
using System.Text.Json.Serialization;
namespace AsistenciaPyme.Application.Feautures.ConfiguracionNomina.Commands;
public class ActualizarConfiguracionNominaCommand : IRequest<ConfiguracionNominaDto>
{
    [JsonIgnore]
    public int IdConfiguracionNomina { get; set; }
    public int MinutosToleranciaEntrada { get; set; } = 10;
    public decimal MultiplicadorHoraExtra { get; set; } = 2.0m;
    public decimal DiasVacacionesPorMes { get; set; } = 2.5m;
    public int DiasBaseProrrateoVacaciones { get; set; } = 30;
    public int PoliticaDescuentoTardanza { get; set; } = 1;
    public int MinutosMaximosVerificacionDepartamento { get; set; } = 10;
    public decimal? TasaINSS { get; set; }
}
