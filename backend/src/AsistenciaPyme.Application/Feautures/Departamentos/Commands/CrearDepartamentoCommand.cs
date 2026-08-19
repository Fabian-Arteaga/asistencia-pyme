using MediatR;
namespace AsistenciaPyme.Application.Feautures.Departamentos.Commands;
public class CrearDepartamentoCommand : IRequest<int>
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
