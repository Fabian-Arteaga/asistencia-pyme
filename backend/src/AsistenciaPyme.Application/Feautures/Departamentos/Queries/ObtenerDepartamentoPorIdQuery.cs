using AsistenciaPyme.Application.Feautures.Departamentos.DTOs;
using MediatR;
namespace AsistenciaPyme.Application.Feautures.Departamentos.Queries;
public class ObtenerDepartamentoPorIdQuery : IRequest<DepartamentoDto?>
{
    public int IdDepartamento { get; set; }
}
