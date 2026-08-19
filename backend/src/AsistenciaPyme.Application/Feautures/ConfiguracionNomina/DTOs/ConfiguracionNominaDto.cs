namespace AsistenciaPyme.Application.Feautures.ConfiguracionNomina.DTOs;
public class ConfiguracionNominaDto
{
    public int IdConfiguracionNomina { get; set; }
    public int MinutosToleranciaEntrada { get; set; }
    public decimal MultiplicadorHoraExtra { get; set; }
    public decimal DiasVacacionesPorMes { get; set; }
    public int DiasBaseProrrateoVacaciones { get; set; }
    public int PoliticaDescuentoTardanza { get; set; }
    public int MinutosMaximosVerificacionDepartamento { get; set; }
    public decimal? TasaINSS { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
