using AsistenciaPyme.Application.Feautures.Embargos.Commands;
using AsistenciaPyme.Application.Feautures.Embargos.DTOs;
using AsistenciaPyme.Application.Feautures.Embargos.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AsistenciaPyme.WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class EmbargosController : ControllerBase
{
    private readonly IMediator _mediator;
    public EmbargosController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearEmbargoCommand command, CancellationToken cancellationToken)
    {
        try
        {
            int idEmbargo = await _mediator.Send(command, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, new { IdEmbargo = idEmbargo, Mensaje = "Embargo registrado correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Mensaje = ex.Message });
        }
    }
    [HttpGet]
    public async Task<ActionResult<List<EmbargoDto>>> ObtenerTodos(CancellationToken cancellationToken)
    {
        var embargos = await _mediator.Send(new ObtenerEmbargosQuery(), cancellationToken);
        return Ok(embargos);
    }
    [HttpGet("{idEmbargo:int}")]
    public async Task<ActionResult<EmbargoDto>> ObtenerPorId(int idEmbargo, CancellationToken cancellationToken)
    {
        var embargo = await _mediator.Send(new ObtenerEmbargoPorIdQuery { IdEmbargo = idEmbargo }, cancellationToken);
        if (embargo is null) return NotFound(new { Mensaje = $"No se encontró el embargo con ID {idEmbargo}." });
        return Ok(embargo);
    }
    [HttpPut("{idEmbargo:int}")]
    public async Task<ActionResult<EmbargoDto>> Actualizar(int idEmbargo, [FromBody] ActualizarEmbargoCommand command, CancellationToken cancellationToken)
    {
        command.IdEmbargo = idEmbargo;
        try
        {
            var embargo = await _mediator.Send(command, cancellationToken);
            if (embargo is null) return NotFound(new { Mensaje = $"No se encontró el embargo con ID {idEmbargo}." });
            return Ok(embargo);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Mensaje = ex.Message });
        }
    }
    [HttpPatch("{idEmbargo:int}/estado")]
    public async Task<ActionResult<EmbargoDto>> CambiarEstado(int idEmbargo, [FromBody] CambiarEstadoEmbargoCommand command, CancellationToken cancellationToken)
    {
        command.IdEmbargo = idEmbargo;
        var embargo = await _mediator.Send(command, cancellationToken);
        if (embargo is null) return NotFound(new { Mensaje = $"No se encontró el embargo con ID {idEmbargo}." });
        return Ok(embargo);
    }
}
