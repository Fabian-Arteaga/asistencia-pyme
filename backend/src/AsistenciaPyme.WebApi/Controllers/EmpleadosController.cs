using AsistenciaPyme.Application.Features.Empleados.Commands;
using AsistenciaPyme.Application.Features.Empleados.DTOs;
using AsistenciaPyme.Application.Features.Empleados.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AsistenciaPyme.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpleadosController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmpleadosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] CrearEmpleadoCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            int idEmpleado = await _mediator.Send(
                command,
                cancellationToken);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { idEmpleado },
                new
                {
                    IdEmpleado = idEmpleado,
                    Mensaje = "Empleado creado correctamente."
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

    [HttpGet]
    public async Task<ActionResult<List<EmpleadoDto>>> ObtenerTodos(
        CancellationToken cancellationToken)
    {
        List<EmpleadoDto> empleados = await _mediator.Send(
            new ObtenerEmpleadosQuery(),
            cancellationToken);

        return Ok(empleados);
    }

    [HttpGet("{idEmpleado:int}")]
    public async Task<ActionResult<EmpleadoDto>> ObtenerPorId(
        int idEmpleado,
        CancellationToken cancellationToken)
    {
        EmpleadoDto? empleado = await _mediator.Send(
            new ObtenerEmpleadoPorIdQuery
            {
                IdEmpleado = idEmpleado
            },
            cancellationToken);

        if (empleado is null)
        {
            return NotFound(new
            {
                Mensaje =
                    $"No se encontró el empleado con ID {idEmpleado}."
            });
        }

        return Ok(empleado);
    }

    [HttpPut("{idEmpleado:int}")]
    public async Task<ActionResult<EmpleadoDto>> Actualizar(
        int idEmpleado,
        [FromBody] ActualizarEmpleadoCommand command,
        CancellationToken cancellationToken)
    {
        command.IdEmpleado = idEmpleado;

        try
        {
            EmpleadoDto? empleado = await _mediator.Send(
                command,
                cancellationToken);

            if (empleado is null)
            {
                return NotFound(new
                {
                    Mensaje =
                        $"No se encontró el empleado con ID {idEmpleado}."
                });
            }

            return Ok(empleado);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                Mensaje = exception.Message
            });
        }
    }

    [HttpPatch("{idEmpleado:int}/estado")]
    public async Task<ActionResult<EmpleadoDto>> CambiarEstado(
        int idEmpleado,
        [FromBody] CambiarEstadoEmpleadoCommand command,
        CancellationToken cancellationToken)
    {
        command.IdEmpleado = idEmpleado;

        EmpleadoDto? empleado = await _mediator.Send(
            command,
            cancellationToken);

        if (empleado is null)
        {
            return NotFound(new
            {
                Mensaje =
                    $"No se encontró el empleado con ID {idEmpleado}."
            });
        }

        return Ok(empleado);
    }
}