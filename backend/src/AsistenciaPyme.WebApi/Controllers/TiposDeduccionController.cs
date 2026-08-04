using AsistenciaPyme.Application.Features.TiposDeduccion.Commands;
using AsistenciaPyme.Application.Features.TiposDeduccion.DTOs;
using AsistenciaPyme.Application.Features.TiposDeduccion.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AsistenciaPyme.WebApi.Controllers;

[ApiController]
[Route("api/tipos-deduccion")]
[Authorize(Roles = "Administrador")]
public class TiposDeduccionController : ControllerBase
{
    private readonly IMediator _mediator;

    public TiposDeduccionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<TipoDeduccionDto>> Crear(
        [FromBody] CrearTipoDeduccionCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            TipoDeduccionDto tipoDeduccion =
                await _mediator.Send(
                    command,
                    cancellationToken);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new
                {
                    idTipoDeduccion =
                        tipoDeduccion.IdTipoDeduccion
                },
                tipoDeduccion);
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
    public async Task<ActionResult<List<TipoDeduccionDto>>>
        ObtenerTodos(
            CancellationToken cancellationToken)
    {
        List<TipoDeduccionDto> tipos =
            await _mediator.Send(
                new ObtenerTiposDeduccionQuery(),
                cancellationToken);

        return Ok(tipos);
    }

    [HttpGet("{idTipoDeduccion:int}")]
    public async Task<ActionResult<TipoDeduccionDto>> ObtenerPorId(
        int idTipoDeduccion,
        CancellationToken cancellationToken)
    {
        TipoDeduccionDto? tipo =
            await _mediator.Send(
                new ObtenerTipoDeduccionPorIdQuery
                {
                    IdTipoDeduccion =
                        idTipoDeduccion
                },
                cancellationToken);

        if (tipo is null)
        {
            return NotFound(new
            {
                Mensaje =
                    $"No se encontró el tipo de deducción con ID " +
                    $"{idTipoDeduccion}."
            });
        }

        return Ok(tipo);
    }

    [HttpPut("{idTipoDeduccion:int}")]
    public async Task<ActionResult<TipoDeduccionDto>> Actualizar(
        int idTipoDeduccion,
        [FromBody] ActualizarTipoDeduccionCommand command,
        CancellationToken cancellationToken)
    {
        command.IdTipoDeduccion = idTipoDeduccion;

        try
        {
            TipoDeduccionDto? tipo =
                await _mediator.Send(
                    command,
                    cancellationToken);

            if (tipo is null)
            {
                return NotFound(new
                {
                    Mensaje =
                        $"No se encontró el tipo de deducción con ID " +
                        $"{idTipoDeduccion}."
                });
            }

            return Ok(tipo);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                Mensaje = exception.Message
            });
        }
    }

    [HttpPatch("{idTipoDeduccion:int}/estado")]
    public async Task<ActionResult<TipoDeduccionDto>>
        CambiarEstado(
            int idTipoDeduccion,
            [FromBody] CambiarEstadoTipoDeduccionCommand command,
            CancellationToken cancellationToken)
    {
        command.IdTipoDeduccion = idTipoDeduccion;

        TipoDeduccionDto? tipo =
            await _mediator.Send(
                command,
                cancellationToken);

        if (tipo is null)
        {
            return NotFound(new
            {
                Mensaje =
                    $"No se encontró el tipo de deducción con ID " +
                    $"{idTipoDeduccion}."
            });
        }

        return Ok(tipo);
    }
}