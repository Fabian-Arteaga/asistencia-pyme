using AsistenciaPyme.Application.Feautures.HorasExtras.Commands;
using AsistenciaPyme.Application.Feautures.HorasExtras.DTOs;
using AsistenciaPyme.Application.Feautures.HorasExtras.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AsistenciaPyme.WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class HorasExtrasController : ControllerBase
{
    private readonly IMediator _mediator;
    public HorasExtrasController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet("pendientes")]
    public async Task<ActionResult<List<HoraExtraDto>>> ObtenerPendientes(CancellationToken cancellationToken)
    {
        var horasExtras = await _mediator.Send(new ObtenerHorasExtrasPendientesQuery(), cancellationToken);
        return Ok(horasExtras);
    }
    [HttpPatch("{idHoraExtra:int}/aprobar")]
    public async Task<ActionResult<HoraExtraDto>> Aprobar(int idHoraExtra, [FromBody] AprobarHoraExtraCommand command, CancellationToken cancellationToken)
    {
        command.IdHoraExtra = idHoraExtra;
        try
        {
            var horaExtra = await _mediator.Send(command, cancellationToken);
            if (horaExtra is null) return NotFound(new { Mensaje = $"No se encontró la hora extra con ID {idHoraExtra}." });
            return Ok(horaExtra);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Mensaje = ex.Message });
        }
    }
    [HttpPatch("{idHoraExtra:int}/rechazar")]
    public async Task<ActionResult<HoraExtraDto>> Rechazar(int idHoraExtra, [FromBody] RechazarHoraExtraCommand command, CancellationToken cancellationToken)
    {
        command.IdHoraExtra = idHoraExtra;
        var horaExtra = await _mediator.Send(command, cancellationToken);
        if (horaExtra is null) return NotFound(new { Mensaje = $"No se encontró la hora extra con ID {idHoraExtra}." });
        return Ok(horaExtra);
    }
}
