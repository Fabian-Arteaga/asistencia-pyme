using AsistenciaPyme.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Common.Interfaces
{
    public interface IAsistenciaPymeDbContext
    {
        DbSet<Cargo> Cargos { get; }

        DbSet<Empleado> Empleados { get; }

        DbSet<Asistencia> Asistencias { get; }

        DbSet<Administrador> Administradores { get; }

        DbSet<Vacacion> Vacaciones { get; }

        DbSet<TipoDeduccion> TiposDeduccion { get; }

        DbSet<Planilla> Planillas { get; }

        DbSet<DeduccionPlanilla> DeduccionesPlanilla { get; }

        DbSet<Departamento> Departamentos { get; }

        DbSet<HorarioLaboral> HorariosLaborales { get; }

        DbSet<DetallePlanilla> DetallesPlanilla { get; }

        DbSet<ConceptoPlanilla> ConceptosPlanilla { get; }

        DbSet<DetalleConceptoPlanilla> DetallesConceptoPlanilla { get; }

        DbSet<HoraExtra> HorasExtras { get; }

        DbSet<Embargo> Embargos { get; }

        DbSet<ConfiguracionNomina> ConfiguracionesNomina { get; }

        DbSet<EmpleadoDepartamentoHistorial> EmpleadoDepartamentoHistorials { get; }

        DbSet<DispositivoMarcaje> DispositivosMarcaje { get; }

        DbSet<VerificacionPresencia> VerificacionesPresencia { get; }

        DbSet<PeriodoEvaluacion> PeriodosEvaluacion { get; }

        DbSet<CategoriaEvaluacion> CategoriasEvaluacion { get; }

        DbSet<CriterioEvaluacion> CriteriosEvaluacion { get; }

        DbSet<EvaluacionDesempeno> EvaluacionesDesempeno { get; }

        DbSet<DetalleEvaluacionDesempeno> DetallesEvaluacionDesempeno { get; }

        Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
    }
}
