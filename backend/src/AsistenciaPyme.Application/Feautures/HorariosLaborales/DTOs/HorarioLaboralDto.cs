namespace AsistenciaPyme.Application.Feautures.HorariosLaborales.DTOs;
public class HorarioLaboralDto
{
    public int IdHorarioLaboral { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public TimeOnly HoraEntrada { get; set; }
    public TimeOnly HoraSalida { get; set; }
    public string DiasLaborales { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
