using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.DTOs;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.Queries;

public class ObtenerEvaluacionesQuery : IRequest<List<EvaluacionDesempenoDto>>
{
    public int? IdPeriodo { get; set; }
    public int? IdEmpleadoEvaluado { get; set; }
    public int? IdEvaluador { get; set; }
    public TipoEvaluador? TipoEvaluador { get; set; }
    public EstadoEvaluacion? Estado { get; set; }
    public int? IdDepartamento { get; set; }
}

public class ObtenerEvaluacionesHandler : IRequestHandler<ObtenerEvaluacionesQuery, List<EvaluacionDesempenoDto>>
{
    private readonly IAsistenciaPymeDbContext _context;

    public ObtenerEvaluacionesHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<List<EvaluacionDesempenoDto>> Handle(ObtenerEvaluacionesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.EvaluacionesDesempeno
            .Include(e => e.Periodo)
            .Include(e => e.EmpleadoEvaluado).ThenInclude(emp => emp.Cargo)
            .Include(e => e.EmpleadoEvaluado).ThenInclude(emp => emp.Departamento)
            .Include(e => e.Evaluador).ThenInclude(eval => eval.Cargo)
            .AsQueryable();

        if (request.IdPeriodo.HasValue && request.IdPeriodo.Value > 0)
        {
            query = query.Where(e => e.IdPeriodoEvaluacion == request.IdPeriodo.Value);
        }

        if (request.IdEmpleadoEvaluado.HasValue && request.IdEmpleadoEvaluado.Value > 0)
        {
            query = query.Where(e => e.IdEmpleadoEvaluado == request.IdEmpleadoEvaluado.Value);
        }

        if (request.IdEvaluador.HasValue && request.IdEvaluador.Value > 0)
        {
            query = query.Where(e => e.IdEvaluador == request.IdEvaluador.Value);
        }

        if (request.TipoEvaluador.HasValue)
        {
            query = query.Where(e => e.TipoEvaluador == request.TipoEvaluador.Value);
        }

        if (request.Estado.HasValue)
        {
            query = query.Where(e => e.Estado == request.Estado.Value);
        }

        if (request.IdDepartamento.HasValue && request.IdDepartamento.Value > 0)
        {
            query = query.Where(e => e.EmpleadoEvaluado.IdDepartamento == request.IdDepartamento.Value);
        }

        var evaluaciones = await query
            .OrderByDescending(e => e.FechaAsignacion)
            .Select(e => new EvaluacionDesempenoDto
            {
                IdEvaluacionDesempeno = e.IdEvaluacionDesempeno,
                IdPeriodoEvaluacion = e.IdPeriodoEvaluacion,
                NombrePeriodo = e.Periodo.Nombre,
                IdEmpleadoEvaluado = e.IdEmpleadoEvaluado,
                CodigoEmpleadoEvaluado = e.EmpleadoEvaluado.CodigoEmpleado,
                NombreCompletoEvaluado = $"{e.EmpleadoEvaluado.Nombres} {e.EmpleadoEvaluado.Apellidos}".Trim(),
                CargoEvaluado = e.EmpleadoEvaluado.Cargo.Nombre,
                DepartamentoEvaluado = e.EmpleadoEvaluado.Departamento != null ? e.EmpleadoEvaluado.Departamento.Nombre : "Sin departamento",
                IdEvaluador = e.IdEvaluador,
                CodigoEvaluador = e.Evaluador.CodigoEmpleado,
                NombreCompletoEvaluador = $"{e.Evaluador.Nombres} {e.Evaluador.Apellidos}".Trim(),
                CargoEvaluador = e.Evaluador.Cargo.Nombre,
                TipoEvaluador = e.TipoEvaluador,
                Estado = e.Estado,
                PuntajeFinal = e.PuntajeFinal,
                ObservacionesGenerales = e.ObservacionesGenerales,
                FechaAsignacion = e.FechaAsignacion,
                FechaCompletada = e.FechaCompletada
            })
            .ToListAsync(cancellationToken);

        return evaluaciones;
    }
}
