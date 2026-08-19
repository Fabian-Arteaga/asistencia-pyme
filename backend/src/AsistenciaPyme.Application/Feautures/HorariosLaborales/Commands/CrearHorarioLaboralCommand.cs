using MediatR;
namespace AsistenciaPyme.Application.Feautures.HorariosLaborales.Commands;
public class CrearHorarioLaboralCommand : IRequest<int>
{
    public string Nombre { get; set; } = string.Empty;
    public TimeOnly HoraEntrada { get; set; }
    public TimeOnly HoraSalida { get; set; }
    public string DiasLaborales { get; set; } = string.Empty;
}
