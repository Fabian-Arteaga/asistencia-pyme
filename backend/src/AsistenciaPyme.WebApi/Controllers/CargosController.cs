using AsistenciaPyme.Application.Features.Cargos.Commands;
using AsistenciaPyme.Application.Features.Cargos.Queries;
using AsistenciaPyme.Application.Feautures.Cargos.Commands;
using AsistenciaPyme.Application.Feautures.Cargos.DTOs;
using AsistenciaPyme.Application.Feautures.Cargos.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AsistenciaPyme.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CargosController : ControllerBase
{
    private readonly IMediator _mediator;

    public CargosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        CrearCargoCommand command,
        CancellationToken cancellationToken)
    {
        int idCargo = await _mediator.Send(
            command,
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            new
            {
                IdCargo = idCargo,
                Mensaje = "Cargo creado correctamente."
            });
    }
    [HttpGet]
    public async Task<ActionResult<List<CargoDTO>>> ObtenerTodos(
    CancellationToken cancellationToken)
    {
        var query = new ObtenerCargosQuery();

        List<CargoDTO> cargos = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(cargos);
    }
    [HttpGet("{idCargo:int}")]
    public async Task<ActionResult<CargoDTO>> ObtenerPorId(
    int idCargo,
    CancellationToken cancellationToken)
    {
        if (idCargo <= 0)
        {
            return BadRequest(new
            {
                Mensaje = "El identificador del cargo debe ser mayor que cero."
            });
        }

        var query = new ObtenerCargoPorIdQuery
        {
            IdCargo = idCargo
        };

        CargoDTO? cargo = await _mediator.Send(
            query,
            cancellationToken);

        if (cargo is null)
        {
            return NotFound(new
            {
                Mensaje = $"No se encontró el cargo con ID {idCargo}."
            });
        }

        return Ok(cargo);
    }
    [HttpPut("{idCargo:int}")]
    public async Task<ActionResult<CargoDTO>> Actualizar(
    int idCargo,
    [FromBody] ActualizarCargoCommand command,
    CancellationToken cancellationToken)
    {
        if (idCargo <= 0)
        {
            return BadRequest(new
            {
                Mensaje = "El identificador del cargo debe ser mayor que cero."
            });
        }

        command.IdCargo = idCargo;

        try
        {
            CargoDTO? cargoActualizado = await _mediator.Send(
                command,
                cancellationToken);

            if (cargoActualizado is null)
            {
                return NotFound(new
                {
                    Mensaje = $"No se encontró el cargo con ID {idCargo}."
                });
            }

            return Ok(cargoActualizado);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                Mensaje = exception.Message
            });
        }
    }
    [HttpPatch("{idCargo:int}/estado")]
    public async Task<ActionResult<CargoDTO>> CambiarEstado(
    int idCargo,
    [FromBody] CambiarEstadoCargoCommand command,
    CancellationToken cancellationToken)
    {
        command.IdCargo = idCargo;

        CargoDTO? cargoActualizado = await _mediator.Send(
            command,
            cancellationToken);

        if (cargoActualizado is null)
        {
            return NotFound(new
            {
                Mensaje = $"No se encontró el cargo con ID {idCargo}."
            });
        }

        return Ok(cargoActualizado);
    }
}