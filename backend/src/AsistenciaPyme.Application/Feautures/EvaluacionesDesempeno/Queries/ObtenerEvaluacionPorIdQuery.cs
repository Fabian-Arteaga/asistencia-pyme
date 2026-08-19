using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.Queries;

public class ObtenerEvaluacionPorIdQuery : IRequest<EvaluacionDesempenoDto?>
{
    public int IdEvaluacionDesempeno { get; set; }
}

public class ObtenerEvaluacionPorIdHandler : IRequestHandler<ObtenerEvaluacionPorIdQuery, EvaluacionDesempenoDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerEvaluacionPorIdHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<EvaluacionDesempenoDto?> Handle(ObtenerEvaluacionPorIdQuery request, CancellationToken cancellationToken)
    {
        var evaluacion = await _context.EvaluacionesDesempeno
            .Include(e => e.Periodo)
            .Include(e => e.EmpleadoEvaluado).ThenInclude(emp => emp.Cargo)
            .Include(e => e.EmpleadoEvaluado).ThenInclude(emp => emp.Departamento)
            .Include(e => e.Evaluador).ThenInclude(eval => eval.Cargo)
            .Include(e => e.Detalles).ThenInclude(d => d.Criterio).ThenInclude(c => c.Categoria)
            .FirstOrDefaultAsync(e => e.IdEvaluacionDesempeno == request.IdEvaluacionDesempeno, cancellationToken);

        if (evaluacion is null)
        {
            return null;
        }

        var dto = new EvaluacionDesempenoDto
        {
            IdEvaluacionDesempeno = evaluacion.IdEvaluacionDesempeno,
            IdPeriodoEvaluacion = evaluacion.IdPeriodoEvaluacion,
            NombrePeriodo = evaluacion.Periodo.Nombre,
            IdEmpleadoEvaluado = evaluacion.IdEmpleadoEvaluado,
            CodigoEmpleadoEvaluado = evaluacion.EmpleadoEvaluado.CodigoEmpleado,
            NombreCompletoEvaluado = $"{evaluacion.EmpleadoEvaluado.Nombres} {evaluacion.EmpleadoEvaluado.Apellidos}".Trim(),
            CargoEvaluado = evaluacion.EmpleadoEvaluado.Cargo.Nombre,
            DepartamentoEvaluado = evaluacion.EmpleadoEvaluado.Departamento?.Nombre ?? "Sin departamento",
            IdEvaluador = evaluacion.IdEvaluador,
            CodigoEvaluador = evaluacion.Evaluador.CodigoEmpleado,
            NombreCompletoEvaluador = $"{evaluacion.Evaluador.Nombres} {evaluacion.Evaluador.Apellidos}".Trim(),
            CargoEvaluador = evaluacion.Evaluador.Cargo.Nombre,
            TipoEvaluador = evaluacion.TipoEvaluador,
            Estado = evaluacion.Estado,
            PuntajeFinal = evaluacion.PuntajeFinal,
            ObservacionesGenerales = evaluacion.ObservacionesGenerales,
            FechaAsignacion = evaluacion.FechaAsignacion,
            FechaCompletada = evaluacion.FechaCompletada,
            Detalles = evaluacion.Detalles.Select(d => new DetalleEvaluacionDto
            {
                IdDetalleEvaluacionDesempeno = d.IdDetalleEvaluacionDesempeno,
                IdEvaluacionDesempeno = d.IdEvaluacionDesempeno,
                IdCriterioEvaluacion = d.IdCriterioEvaluacion,
                IdCategoriaEvaluacion = d.Criterio?.IdCategoriaEvaluacion ?? 0,
                NombreCategoria = !string.IsNullOrEmpty(d.NombreCategoriaHistorica) ? d.NombreCategoriaHistorica : (d.Criterio?.Categoria?.Nombre ?? ""),
                TextoCriterio = !string.IsNullOrEmpty(d.TextoCriterioHistorico) ? d.TextoCriterioHistorico : (d.Criterio?.Texto ?? ""),
                PonderacionCategoria = d.PonderacionCategoriaHistorica > 0 ? d.PonderacionCategoriaHistorica : (d.Criterio?.Categoria?.Ponderacion ?? 0),
                Puntuacion = d.Puntuacion,
                Comentario = d.Comentario
            }).ToList()
        };

        return dto;
    }
}
