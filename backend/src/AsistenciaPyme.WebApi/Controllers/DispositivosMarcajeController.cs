using AsistenciaPyme.Application.Feautures.DispositivosMarcaje.Commands;
using AsistenciaPyme.Application.Feautures.DispositivosMarcaje.DTOs;
using AsistenciaPyme.Application.Feautures.DispositivosMarcaje.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AsistenciaPyme.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class DispositivosMarcajeController : ControllerBase
{
    private readonly IMediator _mediator;

    public DispositivosMarcajeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<DispositivoMarcajeDto>>> ObtenerTodos(CancellationToken cancellationToken)
    {
        var dispositivos = await _mediator.Send(new ObtenerDispositivosMarcajeQuery(), cancellationToken);
        return Ok(dispositivos);
    }

    [HttpPost]
    public async Task<ActionResult<object>> Registrar([FromBody] RegistrarDispositivoMarcajeCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var id = await _mediator.Send(command, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, new { IdDispositivoMarcaje = id, Mensaje = "Dispositivo registrado correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Mensaje = ex.Message });
        }
    }
}
