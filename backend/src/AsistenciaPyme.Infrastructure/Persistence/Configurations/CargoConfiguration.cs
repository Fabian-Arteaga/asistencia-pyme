using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsistenciaPyme.Infrastructure.Persistence.Configurations
{
    public class CargoConfiguration : IEntityTypeConfiguration<Cargo>
    {
        public void Configure(EntityTypeBuilder<Cargo> builder)
        {
            builder.ToTable("cargos");

            builder.HasKey(c => c.IdCargo);

            builder.Property(c => c.IdCargo)
                .HasColumnName("id_cargo")
                .ValueGeneratedOnAdd();

            builder.Property(c => c.Nombre)
                .HasColumnName("nombre")
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(c => c.Nombre)
                .IsUnique();

            builder.Property(c => c.Descripcion)
                .HasColumnName("descripcion")
                .HasMaxLength(250);

            builder.Property(c => c.Funciones)
                .HasColumnName("funciones")
                .IsRequired();

            builder.Property(c => c.Responsabilidades)
                .HasColumnName("responsabilidades")
                .IsRequired();

            builder.Property(c => c.Activo)
                .HasColumnName("activo")
                .HasDefaultValue(true);

            builder.Property(c => c.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(c => c.FechaActualizacion)
                .HasColumnName("fecha_actualizacion");
        }
    }
}
