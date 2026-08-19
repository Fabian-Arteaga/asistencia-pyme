using AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.Commands;
using AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.DTOs;
using AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.Queries;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AsistenciaPyme.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class EvaluacionesDesempenoController : ControllerBase
{
    private readonly IMediator _mediator;

    public EvaluacionesDesempenoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #region Evaluaciones

    [HttpGet]
    public async Task<ActionResult<List<EvaluacionDesempenoDto>>> ObtenerTodos(
        [FromQuery] int? idPeriodo,
        [FromQuery] int? idEmpleadoEvaluado,
        [FromQuery] int? idEvaluador,
        [FromQuery] TipoEvaluador? tipoEvaluador,
        [FromQuery] EstadoEvaluacion? estado,
        [FromQuery] int? idDepartamento,
        CancellationToken cancellationToken)
    {
        var query = new ObtenerEvaluacionesQuery
        {
            IdPeriodo = idPeriodo,
            IdEmpleadoEvaluado = idEmpleadoEvaluado,
            IdEvaluador = idEvaluador,
            TipoEvaluador = tipoEvaluador,
            Estado = estado,
            IdDepartamento = idDepartamento
        };

        var resultados = await _mediator.Send(query, cancellationToken);
        return Ok(resultados);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EvaluacionDesempenoDto>> ObtenerPorId(int id, CancellationToken cancellationToken)
    {
        var evaluacion = await _mediator.Send(new ObtenerEvaluacionPorIdQuery { IdEvaluacionDesempeno = id }, cancellationToken);
        if (evaluacion is null)
        {
            return NotFound(new { Mensaje = $"No se encontró la evaluación con ID {id}." });
        }
        return Ok(evaluacion);
    }

    [HttpPost("asignar")]
    public async Task<IActionResult> Asignar([FromBody] AsignarEvaluacionCommand command, CancellationToken cancellationToken)
    {
        try
        {
            int idEvaluacion = await _mediator.Send(command, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, new
            {
                IdEvaluacionDesempeno = idEvaluacion,
                Mensaje = "Evaluación asignada correctamente."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Mensaje = ex.Message });
        }
    }

    [HttpPut("{id:int}/guardar-respuestas")]
    public async Task<IActionResult> GuardarRespuestas(int id, [FromBody] GuardarRespuestasEvaluacionCommand command, CancellationToken cancellationToken)
    {
        command.IdEvaluacionDesempeno = id;
        try
        {
            await _mediator.Send(command, cancellationToken);
            return Ok(new { Mensaje = "Respuestas guardadas en borrador correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Mensaje = ex.Message });
        }
    }

    [HttpPost("{id:int}/completar")]
    public async Task<IActionResult> Completar(int id, [FromBody] CompletarEvaluacionCommand command, CancellationToken cancellationToken)
    {
        command.IdEvaluacionDesempeno = id;
        try
        {
            decimal puntajeFinal = await _mediator.Send(command, cancellationToken);
            return Ok(new
            {
                IdEvaluacionDesempeno = id,
                PuntajeFinal = puntajeFinal,
                Clasificacion = EvaluacionDesempenoDto.ObtenerClasificacion(puntajeFinal),
                Mensaje = "Evaluación completada y puntaje calculado exitosamente."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Mensaje = ex.Message });
        }
    }

    [HttpGet("empleado/{idEmpleado:int}/consolidado-360")]
    public async Task<ActionResult<ResultadoConsolidado360Dto>> ObtenerConsolidado360(int idEmpleado, [FromQuery] int idPeriodo, CancellationToken cancellationToken)
    {
        if (idPeriodo <= 0)
        {
            return BadRequest(new { Mensaje = "Debe especificar un ID de período válido." });
        }

        var consolidado = await _mediator.Send(new ObtenerResultadoConsolidado360Query
        {
            IdEmpleado = idEmpleado,
            IdPeriodoEvaluacion = idPeriodo
        }, cancellationToken);

        if (consolidado is null)
        {
            return NotFound(new { Mensaje = "No se encontraron datos para el empleado y período especificados." });
        }

        return Ok(consolidado);
    }

    [HttpGet("empleado/{idEmpleado:int}/historial")]
    public async Task<ActionResult<HistorialEvaluacionEmpleadoDto>> ObtenerHistorial(int idEmpleado, CancellationToken cancellationToken)
    {
        var historial = await _mediator.Send(new ObtenerHistorialEmpleadoQuery { IdEmpleado = idEmpleado }, cancellationToken);
        if (historial is null)
        {
            return NotFound(new { Mensaje = $"No se encontró el empleado con ID {idEmpleado}." });
        }
        return Ok(historial);
    }

    #endregion

    #region Periodos

    [HttpGet("periodos")]
    public async Task<ActionResult<List<PeriodoEvaluacionDto>>> ObtenerPeriodos(CancellationToken cancellationToken)
    {
        var periodos = await _mediator.Send(new ObtenerPeriodosEvaluacionQuery(), cancellationToken);
        return Ok(periodos);
    }

    [HttpGet("periodos/activo")]
    public async Task<ActionResult<PeriodoEvaluacionDto>> ObtenerPeriodoActivo(CancellationToken cancellationToken)
    {
        var periodo = await _mediator.Send(new ObtenerPeriodoActivoQuery(), cancellationToken);
        if (periodo is null)
        {
            return NotFound(new { Mensaje = "No hay ningún período de evaluación activo actualmente." });
        }
        return Ok(periodo);
    }

    [HttpPost("periodos")]
    public async Task<IActionResult> CrearPeriodo([FromBody] CrearPeriodoEvaluacionCommand command, CancellationToken cancellationToken)
    {
        try
        {
            int id = await _mediator.Send(command, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, new
            {
                IdPeriodoEvaluacion = id,
                Mensaje = "Período de evaluación creado correctamente."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Mensaje = ex.Message });
        }
    }

    [HttpPut("periodos/{id:int}")]
    public async Task<IActionResult> ActualizarPeriodo(int id, [FromBody] ActualizarPeriodoEvaluacionCommand command, CancellationToken cancellationToken)
    {
        command.IdPeriodoEvaluacion = id;
        try
        {
            await _mediator.Send(command, cancellationToken);
            return Ok(new { Mensaje = "Período de evaluación actualizado correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Mensaje = ex.Message });
        }
    }

    [HttpPatch("periodos/{id:int}/estado")]
    public async Task<IActionResult> CambiarEstadoPeriodo(int id, [FromBody] CambiarEstadoPeriodoEvaluacionCommand command, CancellationToken cancellationToken)
    {
        command.IdPeriodoEvaluacion = id;
        try
        {
            await _mediator.Send(command, cancellationToken);
            return Ok(new { Mensaje = "Estado del período actualizado correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Mensaje = ex.Message });
        }
    }

    #endregion

    #region Checklist y Categorias

    [HttpGet("checklist")]
    public async Task<ActionResult<List<CategoriaEvaluacionDto>>> ObtenerChecklist(CancellationToken cancellationToken)
    {
        var checklist = await _mediator.Send(new ObtenerChecklistEvaluacionQuery(), cancellationToken);
        return Ok(checklist);
    }

    [HttpPut("categorias/ponderaciones")]
    public async Task<IActionResult> ConfigurarPonderaciones([FromBody] ConfigurarPonderacionesCommand command, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(command, cancellationToken);
            return Ok(new { Mensaje = "Ponderaciones de categorías actualizadas correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Mensaje = ex.Message });
        }
    }

    [HttpPost("criterios")]
    public async Task<IActionResult> CrearCriterio([FromBody] CrearCriterioCommand command, CancellationToken cancellationToken)
    {
        try
        {
            int id = await _mediator.Send(command, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, new
            {
                IdCriterioEvaluacion = id,
                Mensaje = "Criterio de evaluación creado correctamente."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Mensaje = ex.Message });
        }
    }

    [HttpPut("criterios/{id:int}")]
    public async Task<IActionResult> ActualizarCriterio(int id, [FromBody] ActualizarCriterioCommand command, CancellationToken cancellationToken)
    {
        command.IdCriterioEvaluacion = id;
        try
        {
            await _mediator.Send(command, cancellationToken);
            return Ok(new { Mensaje = "Criterio de evaluación actualizado correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Mensaje = ex.Message });
        }
    }

    [HttpPatch("criterios/{id:int}/estado")]
    public async Task<IActionResult> CambiarEstadoCriterio(int id, [FromBody] CambiarEstadoCriterioCommand command, CancellationToken cancellationToken)
    {
        command.IdCriterioEvaluacion = id;
        try
        {
            await _mediator.Send(command, cancellationToken);
            return Ok(new { Mensaje = "Estado del criterio actualizado correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Mensaje = ex.Message });
        }
    }

    #endregion
}
