using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AsistenciaPyme.Infrastructure.Persistence.Configurations;

public class PeriodoEvaluacionConfiguration : IEntityTypeConfiguration<PeriodoEvaluacion>
{
    public void Configure(EntityTypeBuilder<PeriodoEvaluacion> builder)
    {
        builder.ToTable("periodos_evaluacion");

        builder.HasKey(p => p.IdPeriodoEvaluacion);

        builder.Property(p => p.IdPeriodoEvaluacion)
            .HasColumnName("id_periodo_evaluacion")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(p => p.Nombre)
            .IsUnique();

        builder.Property(p => p.FechaInicio)
            .HasColumnName("fecha_inicio")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(p => p.FechaFin)
            .HasColumnName("fecha_fin")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(p => p.Estado)
            .HasColumnName("estado")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.FechaCreacion)
            .HasColumnName("fecha_creacion")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(p => p.FechaActualizacion)
            .HasColumnName("fecha_actualizacion");
    }
}
