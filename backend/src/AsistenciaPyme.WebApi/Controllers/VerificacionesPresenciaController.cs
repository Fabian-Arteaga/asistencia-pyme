using AsistenciaPyme.Application.Feautures.VerificacionesPresencia.Commands;
using AsistenciaPyme.Application.Feautures.VerificacionesPresencia.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AsistenciaPyme.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class VerificacionesPresenciaController : ControllerBase
{
    private readonly IMediator _mediator;

    public VerificacionesPresenciaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<VerificacionPresenciaDto>> Registrar([FromBody] RegistrarVerificacionPresenciaCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _mediator.Send(command, cancellationToken);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Mensaje = ex.Message });
        }
    }
}
