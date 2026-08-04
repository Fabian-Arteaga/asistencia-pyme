using AsistenciaPyme.Application
    .Features.Autenticacion.Commands;
using AsistenciaPyme.Application
    .Features.Autenticacion.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AsistenciaPyme.WebApi.Controllers;

[ApiController]
[Route("api/autenticacion")]
[AllowAnonymous]
public class AutenticacionController
    : ControllerBase
{
    private readonly IMediator _mediator;

    public AutenticacionController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<
        ActionResult<LoginResponseDto>> Login(
            [FromBody]
            IniciarSesionCommand command,
            CancellationToken cancellationToken)
    {
        LoginResponseDto? resultado =
            await _mediator.Send(
                command,
                cancellationToken);

        if (resultado is null)
        {
            return Unauthorized(new
            {
                Mensaje =
                    "Correo o contraseña incorrectos."
            });
        }

        return Ok(resultado);
    }
}