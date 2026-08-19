using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsistenciaPyme.Infrastructure.Persistence.Configurations
{
    public class DeduccionPlanillaConfiguration : IEntityTypeConfiguration<DeduccionPlanilla>
    {
        public void Configure(EntityTypeBuilder<DeduccionPlanilla> builder)
        {
            builder.ToTable("deducciones_planilla");

            builder.HasKey(d => d.IdDeduccionPlanilla);

            builder.Property(d => d.IdDeduccionPlanilla)
                .HasColumnName("id_deduccion_planilla")
                .ValueGeneratedOnAdd();

            builder.Property(d => d.IdPlanilla)
                .HasColumnName("id_planilla");

            builder.Property(d => d.IdDetallePlanilla)
                .HasColumnName("id_detalle_planilla");

            builder.Property(d => d.IdTipoDeduccion)
                .HasColumnName("id_tipo_deduccion");

            builder.Property(d => d.ValorAplicado)
                .HasColumnName("valor_aplicado")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(d => d.MontoCalculado)
                .HasColumnName("monto_calculado")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(d => d.Observacion)
                .HasColumnName("observacion")
                .HasMaxLength(250);

            builder.Property(d => d.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasIndex(d => new
            {
                d.IdPlanilla,
                d.IdTipoDeduccion
            }).IsUnique();

            builder.HasOne(d => d.Planilla)
                .WithMany(p => p.Deducciones)
                .HasForeignKey(d => d.IdPlanilla)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.DetallePlanilla)
                .WithMany()
                .HasForeignKey(d => d.IdDetallePlanilla)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.TipoDeduccion)
                .WithMany(t => t.DeduccionesPlanilla)
                .HasForeignKey(d => d.IdTipoDeduccion)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
