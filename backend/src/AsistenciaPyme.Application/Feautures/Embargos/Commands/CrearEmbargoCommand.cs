using MediatR;
namespace AsistenciaPyme.Application.Feautures.Embargos.Commands;
public class CrearEmbargoCommand : IRequest<int>
{
    public int IdEmpleado { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int TipoCalculo { get; set; }
    public decimal? Monto { get; set; }
    public decimal? Porcentaje { get; set; }
    public decimal SaldoPendiente { get; set; }
    public string? Referencia { get; set; }
    public string? Observacion { get; set; }
}
