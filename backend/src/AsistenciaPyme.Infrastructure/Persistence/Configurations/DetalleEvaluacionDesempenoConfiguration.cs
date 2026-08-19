using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AsistenciaPyme.Infrastructure.Persistence.Configurations;

public class DetalleEvaluacionDesempenoConfiguration : IEntityTypeConfiguration<DetalleEvaluacionDesempeno>
{
    public void Configure(EntityTypeBuilder<DetalleEvaluacionDesempeno> builder)
    {
        builder.ToTable("detalles_evaluacion_desempeno");

        builder.HasKey(d => d.IdDetalleEvaluacionDesempeno);

        builder.Property(d => d.IdDetalleEvaluacionDesempeno)
            .HasColumnName("id_detalle_evaluacion_desempeno")
            .ValueGeneratedOnAdd();

        builder.Property(d => d.IdEvaluacionDesempeno)
            .HasColumnName("id_evaluacion_desempeno")
            .IsRequired();

        builder.Property(d => d.IdCriterioEvaluacion)
            .HasColumnName("id_criterio_evaluacion")
            .IsRequired();

        builder.Property(d => d.Puntuacion)
            .HasColumnName("puntuacion")
            .IsRequired();

        builder.Property(d => d.Comentario)
            .HasColumnName("comentario")
            .HasMaxLength(250);

        builder.Property(d => d.PonderacionCategoriaHistorica)
            .HasColumnName("ponderacion_categoria_historica")
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(d => d.NombreCategoriaHistorica)
            .HasColumnName("nombre_categoria_historica")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(d => d.TextoCriterioHistorico)
            .HasColumnName("texto_criterio_historico")
            .HasMaxLength(250)
            .IsRequired();

        builder.HasIndex(d => new { d.IdEvaluacionDesempeno, d.IdCriterioEvaluacion })
            .IsUnique();

        builder.HasOne(d => d.Evaluacion)
            .WithMany(e => e.Detalles)
            .HasForeignKey(d => d.IdEvaluacionDesempeno)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Criterio)
            .WithMany()
            .HasForeignKey(d => d.IdCriterioEvaluacion)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
