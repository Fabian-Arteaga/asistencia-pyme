using AsistenciaPyme.Application.Features.Vacaciones.Commands;
using AsistenciaPyme.Application.Features.Vacaciones.DTOs;
using AsistenciaPyme.Application.Features.Vacaciones.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AsistenciaPyme.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class VacacionesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VacacionesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<VacacionDto>> Crear(
        [FromBody] CrearVacacionCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            VacacionDto? vacacion =
                await _mediator.Send(
                    command,
                    cancellationToken);

            if (vacacion is null)
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
                    idVacacion = vacacion.IdVacacion
                },
                vacacion);
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
    public async Task<ActionResult<List<VacacionDto>>> ObtenerTodas(
        CancellationToken cancellationToken)
    {
        List<VacacionDto> vacaciones =
            await _mediator.Send(
                new ObtenerVacacionesQuery(),
                cancellationToken);

        return Ok(vacaciones);
    }

    [HttpGet("{idVacacion:int}")]
    public async Task<ActionResult<VacacionDto>> ObtenerPorId(
        int idVacacion,
        CancellationToken cancellationToken)
    {
        VacacionDto? vacacion =
            await _mediator.Send(
                new ObtenerVacacionPorIdQuery
                {
                    IdVacacion = idVacacion
                },
                cancellationToken);

        if (vacacion is null)
        {
            return NotFound(new
            {
                Mensaje =
                    $"No se encontró la vacación con ID {idVacacion}."
            });
        }

        return Ok(vacacion);
    }

    [HttpGet("empleado/{codigoEmpleado}")]
    public async Task<ActionResult<List<VacacionDto>>>
        ObtenerPorCodigoEmpleado(
            string codigoEmpleado,
            CancellationToken cancellationToken)
    {
        List<VacacionDto>? vacaciones =
            await _mediator.Send(
                new ObtenerVacacionesPorCodigoEmpleadoQuery
                {
                    CodigoEmpleado = codigoEmpleado
                },
                cancellationToken);

        if (vacaciones is null)
        {
            return NotFound(new
            {
                Mensaje =
                    $"No se encontró el empleado con código " +
                    $"{codigoEmpleado}."
            });
        }

        return Ok(vacaciones);
    }

    [HttpPut("{idVacacion:int}")]
    public async Task<ActionResult<VacacionDto>> Actualizar(
        int idVacacion,
        [FromBody] ActualizarVacacionCommand command,
        CancellationToken cancellationToken)
    {
        command.IdVacacion = idVacacion;

        try
        {
            VacacionDto? vacacion =
                await _mediator.Send(
                    command,
                    cancellationToken);

            if (vacacion is null)
            {
                return NotFound(new
                {
                    Mensaje =
                        $"No se encontró la vacación con ID " +
                        $"{idVacacion}."
                });
            }

            return Ok(vacacion);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                Mensaje = exception.Message
            });
        }
    }

    [HttpPatch("{idVacacion:int}/cancelar")]
    public async Task<ActionResult<VacacionDto>> Cancelar(
        int idVacacion,
        CancellationToken cancellationToken)
    {
        VacacionDto? vacacion =
            await _mediator.Send(
                new CancelarVacacionCommand
                {
                    IdVacacion = idVacacion
                },
                cancellationToken);

        if (vacacion is null)
        {
            return NotFound(new
            {
                Mensaje =
                    $"No se encontró la vacación con ID {idVacacion}."
            });
        }

        return Ok(vacacion);
    }
}