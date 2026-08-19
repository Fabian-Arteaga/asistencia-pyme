using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AsistenciaPyme.Infrastructure.Persistence.Configurations;

public class EvaluacionDesempenoConfiguration : IEntityTypeConfiguration<EvaluacionDesempeno>
{
    public void Configure(EntityTypeBuilder<EvaluacionDesempeno> builder)
    {
        builder.ToTable("evaluaciones_desempeno");

        builder.HasKey(e => e.IdEvaluacionDesempeno);

        builder.Property(e => e.IdEvaluacionDesempeno)
            .HasColumnName("id_evaluacion_desempeno")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.IdPeriodoEvaluacion)
            .HasColumnName("id_periodo_evaluacion")
            .IsRequired();

        builder.Property(e => e.IdEmpleadoEvaluado)
            .HasColumnName("id_empleado_evaluado")
            .IsRequired();

        builder.Property(e => e.IdEvaluador)
            .HasColumnName("id_evaluador")
            .IsRequired();

        builder.Property(e => e.TipoEvaluador)
            .HasColumnName("tipo_evaluador")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(e => e.Estado)
            .HasColumnName("estado")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(e => e.PuntajeFinal)
            .HasColumnName("puntaje_final")
            .HasPrecision(5, 2);

        builder.Property(e => e.ObservacionesGenerales)
            .HasColumnName("observaciones_generales")
            .HasMaxLength(500);

        builder.Property(e => e.FechaAsignacion)
            .HasColumnName("fecha_asignacion")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.FechaCompletada)
            .HasColumnName("fecha_completada");

        builder.Property(e => e.FechaActualizacion)
            .HasColumnName("fecha_actualizacion");

        builder.HasIndex(e => new { e.IdPeriodoEvaluacion, e.IdEmpleadoEvaluado, e.IdEvaluador, e.TipoEvaluador })
            .IsUnique();

        builder.HasOne(e => e.Periodo)
            .WithMany(p => p.Evaluaciones)
            .HasForeignKey(e => e.IdPeriodoEvaluacion)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.EmpleadoEvaluado)
            .WithMany(emp => emp.EvaluacionesRecibidas)
            .HasForeignKey(e => e.IdEmpleadoEvaluado)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Evaluador)
            .WithMany(emp => emp.EvaluacionesRealizadas)
            .HasForeignKey(e => e.IdEvaluador)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
