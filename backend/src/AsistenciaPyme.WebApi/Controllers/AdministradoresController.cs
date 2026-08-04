using AsistenciaPyme.Application.Features.Administradores.Commands;
using AsistenciaPyme.Application.Features.Administradores.DTOs;
using AsistenciaPyme.Application.Features.Administradores.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AsistenciaPyme.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class AdministradoresController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdministradoresController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<AdministradorDto>> Crear(
        [FromBody] CrearAdministradorCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            AdministradorDto administrador =
                await _mediator.Send(
                    command,
                    cancellationToken);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new
                {
                    idAdministrador =
                        administrador.IdAdministrador
                },
                administrador);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                Mensaje = exception.Message
            });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<AdministradorDto>>>
        ObtenerTodos(
            CancellationToken cancellationToken)
    {
        List<AdministradorDto> administradores =
            await _mediator.Send(
                new ObtenerAdministradoresQuery(),
                cancellationToken);

        return Ok(administradores);
    }

    [HttpGet("{idAdministrador:int}")]
    public async Task<ActionResult<AdministradorDto>>
        ObtenerPorId(
            int idAdministrador,
            CancellationToken cancellationToken)
    {
        AdministradorDto? administrador =
            await _mediator.Send(
                new ObtenerAdministradorPorIdQuery
                {
                    IdAdministrador =
                        idAdministrador
                },
                cancellationToken);

        if (administrador is null)
        {
            return NotFound(new
            {
                Mensaje =
                    $"No se encontró el administrador con ID " +
                    $"{idAdministrador}."
            });
        }

        return Ok(administrador);
    }

    [HttpPut("{idAdministrador:int}")]
    public async Task<ActionResult<AdministradorDto>>
        Actualizar(
            int idAdministrador,
            [FromBody] ActualizarAdministradorCommand command,
            CancellationToken cancellationToken)
    {
        command.IdAdministrador =
            idAdministrador;

        try
        {
            AdministradorDto? administrador =
                await _mediator.Send(
                    command,
                    cancellationToken);

            if (administrador is null)
            {
                return NotFound(new
                {
                    Mensaje =
                        $"No se encontró el administrador con ID " +
                        $"{idAdministrador}."
                });
            }

            return Ok(administrador);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                Mensaje = exception.Message
            });
        }
    }

    [HttpPatch("{idAdministrador:int}/estado")]
    public async Task<ActionResult<AdministradorDto>>
        CambiarEstado(
            int idAdministrador,
            [FromBody] CambiarEstadoAdministradorCommand command,
            CancellationToken cancellationToken)
    {
        command.IdAdministrador =
            idAdministrador;

        AdministradorDto? administrador =
            await _mediator.Send(
                command,
                cancellationToken);

        if (administrador is null)
        {
            return NotFound(new
            {
                Mensaje =
                    $"No se encontró el administrador con ID " +
                    $"{idAdministrador}."
            });
        }

        return Ok(administrador);
    }
    [HttpPatch("me/contrasena")]
    public async Task<IActionResult> CambiarMiContrasena(
    [FromBody]
    CambiarContrasenaAdministradorCommand command,
    CancellationToken cancellationToken)
    {
        Claim? claimIdAdministrador =
            User.FindFirst("idAdministrador");

        if (claimIdAdministrador is null ||
            !int.TryParse(
                claimIdAdministrador.Value,
                out int idAdministrador))
        {
            return Unauthorized(new
            {
                Mensaje =
                    "No se pudo identificar al administrador autenticado."
            });
        }

        command.IdAdministrador = idAdministrador;

        bool actualizado = await _mediator.Send(
            command,
            cancellationToken);

        if (!actualizado)
        {
            return Unauthorized(new
            {
                Mensaje =
                    "La cuenta del administrador no está disponible."
            });
        }

        return Ok(new
        {
            Mensaje =
                "Contraseña actualizada correctamente.",

            RequiereNuevoInicioSesion = true
        });
    }
}