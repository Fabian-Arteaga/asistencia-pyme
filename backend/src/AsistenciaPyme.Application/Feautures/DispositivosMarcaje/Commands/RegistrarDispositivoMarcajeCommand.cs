using MediatR;

namespace AsistenciaPyme.Application.Feautures.DispositivosMarcaje.Commands;

public class RegistrarDispositivoMarcajeCommand : IRequest<int>
{
    public string Nombre { get; set; } = string.Empty;
    public int Tipo { get; set; }
    public int? IdDepartamento { get; set; }
    public string Identificador { get; set; } = string.Empty;
}
