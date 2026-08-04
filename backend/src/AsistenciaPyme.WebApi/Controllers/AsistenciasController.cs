using AsistenciaPyme.Application.Features.Asistencias.Commands;
using AsistenciaPyme.Application.Features.Asistencias.DTOs;
using AsistenciaPyme.Application.Features.Asistencias.Queries;
using AsistenciaPyme.Application.Feautures.Asistencias.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AsistenciaPyme.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AsistenciasController : ControllerBase
{
    private readonly IMediator _mediator;

    public AsistenciasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("marcar")]
    public async Task<ActionResult<ResultadoMarcacionDto>> Marcar(
        [FromBody] MarcarAsistenciaCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            ResultadoMarcacionDto resultado =
                await _mediator.Send(
                    command,
                    cancellationToken);

            return Ok(resultado);
        }
        catch (UnauthorizedAccessException exception)
        {
            return Unauthorized(new
            {
                Mensaje = exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                Mensaje = exception.Message
            });
        }
    }
    [HttpPost("manual")]
    public async Task<ActionResult<AsistenciaDto>> RegistrarManual(
    [FromBody] RegistrarAsistenciaManualCommand command,
    CancellationToken cancellationToken)
    {
        try
        {
            AsistenciaDto? asistencia = await _mediator.Send(
                command,
                cancellationToken);

            if (asistencia is null)
            {
                return NotFound(new
                {
                    Mensaje =
                        $"No se encontró el empleado con código " +
                        $"{command.CodigoEmpleado}."
                });
            }

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new
                {
                    idAsistencia = asistencia.IdAsistencia
                },
                asistencia);
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
    public async Task<ActionResult<List<AsistenciaDto>>> ObtenerTodas(
        CancellationToken cancellationToken)
    {
        List<AsistenciaDto> asistencias =
            await _mediator.Send(
                new ObtenerAsistenciasQuery(),
                cancellationToken);

        return Ok(asistencias);
    }

    [HttpGet("{idAsistencia:int}")]
    public async Task<ActionResult<AsistenciaDto>> ObtenerPorId(
        int idAsistencia,
        CancellationToken cancellationToken)
    {
        AsistenciaDto? asistencia =
            await _mediator.Send(
                new ObtenerAsistenciaPorIdQuery
                {
                    IdAsistencia = idAsistencia
                },
                cancellationToken);

        if (asistencia is null)
        {
            return NotFound(new
            {
                Mensaje =
                    $"No se encontró la asistencia con ID {idAsistencia}."
            });
        }

        return Ok(asistencia);
    }
    [HttpGet("empleado/{codigoEmpleado}")]
    public async Task<ActionResult<List<AsistenciaDto>>>
    ObtenerPorCodigoEmpleado(
        string codigoEmpleado,
        CancellationToken cancellationToken)
    {
        List<AsistenciaDto>? asistencias =
            await _mediator.Send(
                new ObtenerAsistenciasPorCodigoQuery
                {
                    CodigoEmpleado = codigoEmpleado
                },
                cancellationToken);

        if (asistencias is null)
        {
            return NotFound(new
            {
                Mensaje =
                    $"No se encontró el empleado con código {codigoEmpleado}."
            });
        }

        return Ok(asistencias);
    }

    [HttpPut("{idAsistencia:int}/corregir")]
    public async Task<ActionResult<AsistenciaDto>> Corregir(
        int idAsistencia,
        [FromBody] CorregirAsistenciaCommand command,
        CancellationToken cancellationToken)
    {
        command.IdAsistencia = idAsistencia;

        try
        {
            AsistenciaDto? asistencia =
                await _mediator.Send(
                    command,
                    cancellationToken);

            if (asistencia is null)
            {
                return NotFound(new
                {
                    Mensaje =
                        $"No se encontró la asistencia con ID {idAsistencia}."
                });
            }

            return Ok(asistencia);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                Mensaje = exception.Message
            });
        }
    }
}