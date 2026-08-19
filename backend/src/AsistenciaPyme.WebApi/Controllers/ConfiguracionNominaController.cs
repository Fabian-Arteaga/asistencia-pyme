using AsistenciaPyme.Application.Feautures.ConfiguracionNomina.Commands;
using AsistenciaPyme.Application.Feautures.ConfiguracionNomina.DTOs;
using AsistenciaPyme.Application.Feautures.ConfiguracionNomina.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AsistenciaPyme.WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class ConfiguracionNominaController : ControllerBase
{
    private readonly IMediator _mediator;
    public ConfiguracionNominaController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet]
    public async Task<ActionResult<ConfiguracionNominaDto>> Obtener(CancellationToken cancellationToken)
    {
        var configuracion = await _mediator.Send(new ObtenerConfiguracionNominaQuery(), cancellationToken);
        return Ok(configuracion);
    }
    [HttpPut]
    public async Task<ActionResult<ConfiguracionNominaDto>> Actualizar([FromBody] ActualizarConfiguracionNominaCommand command, CancellationToken cancellationToken)
    {
        var configuracion = await _mediator.Send(command, cancellationToken);
        return Ok(configuracion);
    }
}
