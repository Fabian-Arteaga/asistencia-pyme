using AsistenciaPyme.Application.Feautures.Departamentos.DTOs;
using MediatR;
using System.Text.Json.Serialization;
namespace AsistenciaPyme.Application.Feautures.Departamentos.Commands;
public class ActualizarDepartamentoCommand : IRequest<DepartamentoDto?>
{
    [JsonIgnore]
    public int IdDepartamento { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
