using AsistenciaPyme.Application.Feautures.HorariosLaborales.Commands;
using AsistenciaPyme.Application.Feautures.HorariosLaborales.DTOs;
using AsistenciaPyme.Application.Feautures.HorariosLaborales.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AsistenciaPyme.WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class HorariosLaboralesController : ControllerBase
{
    private readonly IMediator _mediator;
    public HorariosLaboralesController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearHorarioLaboralCommand command, CancellationToken cancellationToken)
    {
        try
        {
            int idHorario = await _mediator.Send(command, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, new { IdHorarioLaboral = idHorario, Mensaje = "Horario laboral creado correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Mensaje = ex.Message });
        }
    }
    [HttpGet]
    public async Task<ActionResult<List<HorarioLaboralDto>>> ObtenerTodos(CancellationToken cancellationToken)
    {
        var horarios = await _mediator.Send(new ObtenerHorariosLaboralesQuery(), cancellationToken);
        return Ok(horarios);
    }
    [HttpGet("{idHorarioLaboral:int}")]
    public async Task<ActionResult<HorarioLaboralDto>> ObtenerPorId(int idHorarioLaboral, CancellationToken cancellationToken)
    {
        var horario = await _mediator.Send(new ObtenerHorarioLaboralPorIdQuery { IdHorarioLaboral = idHorarioLaboral }, cancellationToken);
        if (horario is null) return NotFound(new { Mensaje = $"No se encontró el horario laboral con ID {idHorarioLaboral}." });
        return Ok(horario);
    }
    [HttpPut("{idHorarioLaboral:int}")]
    public async Task<ActionResult<HorarioLaboralDto>> Actualizar(int idHorarioLaboral, [FromBody] ActualizarHorarioLaboralCommand command, CancellationToken cancellationToken)
    {
        command.IdHorarioLaboral = idHorarioLaboral;
        try
        {
            var horario = await _mediator.Send(command, cancellationToken);
            if (horario is null) return NotFound(new { Mensaje = $"No se encontró el horario laboral con ID {idHorarioLaboral}." });
            return Ok(horario);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Mensaje = ex.Message });
        }
    }
    [HttpPatch("{idHorarioLaboral:int}/estado")]
    public async Task<ActionResult<HorarioLaboralDto>> CambiarEstado(int idHorarioLaboral, [FromBody] CambiarEstadoHorarioLaboralCommand command, CancellationToken cancellationToken)
    {
        command.IdHorarioLaboral = idHorarioLaboral;
        var horario = await _mediator.Send(command, cancellationToken);
        if (horario is null) return NotFound(new { Mensaje = $"No se encontró el horario laboral con ID {idHorarioLaboral}." });
        return Ok(horario);
    }
}
