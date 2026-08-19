using AsistenciaPyme.Application.Feautures.Departamentos.DTOs;
using MediatR;
using System.Text.Json.Serialization;
namespace AsistenciaPyme.Application.Feautures.Departamentos.Commands;
public class CambiarEstadoDepartamentoCommand : IRequest<DepartamentoDto?>
{
    [JsonIgnore]
    public int IdDepartamento { get; set; }
    public bool Activo { get; set; }
}
