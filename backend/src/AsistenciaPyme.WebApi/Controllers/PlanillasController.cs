using AsistenciaPyme.Application.Features.Planillas.Commands;
using AsistenciaPyme.Application.Features.Planillas.DTOs;
using AsistenciaPyme.Application.Features.Planillas.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AsistenciaPyme.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlanillasController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlanillasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<PlanillaDto>> Generar(
        [FromBody] GenerarPlanillaCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            PlanillaDto? planilla =
                await _mediator.Send(
                    command,
                    cancellationToken);

            if (planilla is null)
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
                    idPlanilla =
                        planilla.IdPlanilla
                },
                planilla);
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
    public async Task<ActionResult<List<PlanillaDto>>>
        ObtenerTodas(
            CancellationToken cancellationToken)
    {
        List<PlanillaDto> planillas =
            await _mediator.Send(
                new ObtenerPlanillasQuery(),
                cancellationToken);

        return Ok(planillas);
    }

    [HttpGet("{idPlanilla:int}")]
    public async Task<ActionResult<PlanillaDto>> ObtenerPorId(
        int idPlanilla,
        CancellationToken cancellationToken)
    {
        PlanillaDto? planilla =
            await _mediator.Send(
                new ObtenerPlanillaPorIdQuery
                {
                    IdPlanilla = idPlanilla
                },
                cancellationToken);

        if (planilla is null)
        {
            return NotFound(new
            {
                Mensaje =
                    $"No se encontró la planilla con ID " +
                    $"{idPlanilla}."
            });
        }

        return Ok(planilla);
    }

    [HttpGet("empleado/{codigoEmpleado}")]
    public async Task<ActionResult<List<PlanillaDto>>>
        ObtenerPorCodigoEmpleado(
            string codigoEmpleado,
            CancellationToken cancellationToken)
    {
        List<PlanillaDto>? planillas =
            await _mediator.Send(
                new ObtenerPlanillasPorCodigoEmpleadoQuery
                {
                    CodigoEmpleado = codigoEmpleado
                },
                cancellationToken);

        if (planillas is null)
        {
            return NotFound(new
            {
                Mensaje =
                    $"No se encontró el empleado con código " +
                    $"{codigoEmpleado}."
            });
        }

        return Ok(planillas);
    }

    [HttpPatch("{idPlanilla:int}/estado")]
    public async Task<ActionResult<PlanillaDto>> CambiarEstado(
        int idPlanilla,
        [FromBody] CambiarEstadoPlanillaCommand command,
        CancellationToken cancellationToken)
    {
        command.IdPlanilla = idPlanilla;

        try
        {
            PlanillaDto? planilla =
                await _mediator.Send(
                    command,
                    cancellationToken);

            if (planilla is null)
            {
                return NotFound(new
                {
                    Mensaje =
                        $"No se encontró la planilla con ID " +
                        $"{idPlanilla}."
                });
            }

            return Ok(planilla);
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