using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.Indemnizacion.DTOs;
using AsistenciaPyme.Application.Feautures.Indemnizacion.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AsistenciaPyme.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class IndemnizacionesController : ControllerBase
{
    private readonly IMediator _mediator;

    public IndemnizacionesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{idEmpleado:int}")]
    public async Task<ActionResult<IndemnizacionEmpleadoDto>> Obtener(int idEmpleado, CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new ObtenerIndemnizacionEmpleadoQuery { IdEmpleado = idEmpleado }, cancellationToken);
        return Ok(dto);
    }
}
