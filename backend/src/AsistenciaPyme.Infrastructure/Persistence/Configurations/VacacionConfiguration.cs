using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;


namespace AsistenciaPyme.Infrastructure.Persistence.Configurations
{
    public class VacacionConfiguration : IEntityTypeConfiguration<Vacacion>
    {
        public void Configure(EntityTypeBuilder<Vacacion> builder)
        {
            builder.ToTable("vacaciones");

            builder.HasKey(v => v.IdVacacion);

            builder.Property(v => v.IdVacacion)
                .HasColumnName("id_vacacion")
                .ValueGeneratedOnAdd();

            builder.Property(v => v.IdEmpleado)
                .HasColumnName("id_empleado");

            builder.Property(v => v.IdAdministrador)
                .HasColumnName("id_administrador");

            builder.Property(v => v.FechaInicio)
                .HasColumnName("fecha_inicio")
                .HasColumnType("date")
                .IsRequired();

            builder.Property(v => v.FechaFin)
                .HasColumnName("fecha_fin")
                .HasColumnType("date")
                .IsRequired();

            builder.Property(v => v.Motivo)
                .HasColumnName("motivo")
                .HasMaxLength(250);

            builder.Property(v => v.Observacion)
                .HasColumnName("observacion")
                .HasMaxLength(300);

            builder.Property(v => v.Cancelada)
                .HasColumnName("cancelada")
                .HasDefaultValue(false);

            builder.Property(v => v.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(v => v.FechaActualizacion)
                .HasColumnName("fecha_actualizacion");

            builder.HasIndex(v => new
            {
                v.IdEmpleado,
                v.FechaInicio,
                v.FechaFin
            });

            builder.HasOne(v => v.Empleado)
                .WithMany(e => e.Vacaciones)
                .HasForeignKey(v => v.IdEmpleado)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Administrador)
                .WithMany(a => a.VacacionesRegistradas)
                .HasForeignKey(v => v.IdAdministrador)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
