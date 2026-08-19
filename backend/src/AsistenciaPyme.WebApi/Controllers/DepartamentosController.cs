using AsistenciaPyme.Application.Feautures.Departamentos.Commands;
using AsistenciaPyme.Application.Feautures.Departamentos.DTOs;
using AsistenciaPyme.Application.Feautures.Departamentos.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AsistenciaPyme.WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class DepartamentosController : ControllerBase
{
    private readonly IMediator _mediator;
    public DepartamentosController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearDepartamentoCommand command, CancellationToken cancellationToken)
    {
        try
        {
            int idDepartamento = await _mediator.Send(command, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, new { IdDepartamento = idDepartamento, Mensaje = "Departamento creado correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Mensaje = ex.Message });
        }
    }
    [HttpGet]
    public async Task<ActionResult<List<DepartamentoDto>>> ObtenerTodos(CancellationToken cancellationToken)
    {
        var departamentos = await _mediator.Send(new ObtenerDepartamentosQuery(), cancellationToken);
        return Ok(departamentos);
    }
    [HttpGet("{idDepartamento:int}")]
    public async Task<ActionResult<DepartamentoDto>> ObtenerPorId(int idDepartamento, CancellationToken cancellationToken)
    {
        var departamento = await _mediator.Send(new ObtenerDepartamentoPorIdQuery { IdDepartamento = idDepartamento }, cancellationToken);
        if (departamento is null)
        {
            return NotFound(new { Mensaje = $"No se encontró el departamento con ID {idDepartamento}." });
        }
        return Ok(departamento);
    }

    [HttpGet("{idDepartamento:int}/empleados")]
    public async Task<ActionResult<List<AsistenciaPyme.Application.Features.Empleados.DTOs.EmpleadoDto>>> ObtenerEmpleadosPorDepartamento(
        int idDepartamento,
        [FromQuery] bool soloActivos = true,
        CancellationToken cancellationToken = default)
    {
        var empleados = await _mediator.Send(
            new AsistenciaPyme.Application.Features.Empleados.Queries.ObtenerEmpleadosQuery
            {
                IdDepartamento = idDepartamento,
                SoloActivos = soloActivos
            },
            cancellationToken);

        return Ok(empleados);
    }
    [HttpPut("{idDepartamento:int}")]
    public async Task<ActionResult<DepartamentoDto>> Actualizar(int idDepartamento, [FromBody] ActualizarDepartamentoCommand command, CancellationToken cancellationToken)
    {
        command.IdDepartamento = idDepartamento;
        try
        {
            var departamento = await _mediator.Send(command, cancellationToken);
            if (departamento is null)
            {
                return NotFound(new { Mensaje = $"No se encontró el departamento con ID {idDepartamento}." });
            }
            return Ok(departamento);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Mensaje = ex.Message });
        }
    }
    [HttpPatch("{idDepartamento:int}/estado")]
    public async Task<ActionResult<DepartamentoDto>> CambiarEstado(int idDepartamento, [FromBody] CambiarEstadoDepartamentoCommand command, CancellationToken cancellationToken)
    {
        command.IdDepartamento = idDepartamento;
        var departamento = await _mediator.Send(command, cancellationToken);
        if (departamento is null)
        {
            return NotFound(new { Mensaje = $"No se encontró el departamento con ID {idDepartamento}." });
        }
        return Ok(departamento);
    }
}
