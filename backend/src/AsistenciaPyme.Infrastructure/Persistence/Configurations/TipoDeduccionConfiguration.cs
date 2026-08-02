using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace AsistenciaPyme.Infrastructure.Persistence.Configurations
{
    public class TipoDeduccionConfiguration : IEntityTypeConfiguration<TipoDeduccion>
    {
        public void Configure(EntityTypeBuilder<TipoDeduccion> builder)
        {
            builder.ToTable("tipos_deduccion");

            builder.HasKey(t => t.IdTipoDeduccion);

            builder.Property(t => t.IdTipoDeduccion)
                .HasColumnName("id_tipo_deduccion")
                .ValueGeneratedOnAdd();

            builder.Property(t => t.Nombre)
                .HasColumnName("nombre")
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(t => t.Nombre)
                .IsUnique();

            builder.Property(t => t.Descripcion)
                .HasColumnName("descripcion")
                .HasMaxLength(250);

            builder.Property(t => t.TipoCalculo)
                .HasColumnName("tipo_calculo")
                .HasConversion<int>()
                .IsRequired();

            builder.Property(t => t.ValorPredeterminado)
                .HasColumnName("valor_predeterminado")
                .HasPrecision(12, 2);

            builder.Property(t => t.Activo)
                .HasColumnName("activo")
                .HasDefaultValue(true);

            builder.Property(t => t.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(t => t.FechaActualizacion)
                .HasColumnName("fecha_actualizacion");
        }
    }
}
