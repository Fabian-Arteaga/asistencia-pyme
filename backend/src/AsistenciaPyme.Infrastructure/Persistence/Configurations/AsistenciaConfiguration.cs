using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsistenciaPyme.Infrastructure.Persistence.Configurations
{
    public class AsistenciaConfiguration : IEntityTypeConfiguration<Asistencia>
    {
        public void Configure(EntityTypeBuilder<Asistencia> builder)
        {
            builder.ToTable("asistencias");

            builder.HasKey(a => a.IdAsistencia);

            builder.Property(a => a.IdAsistencia)
                .HasColumnName("id_asistencia")
                .ValueGeneratedOnAdd();

            builder.Property(a => a.IdEmpleado)
                .HasColumnName("id_empleado");

            builder.Property(a => a.HoraEntrada)
                .HasColumnName("hora_entrada")
                .IsRequired();

            builder.Property(a => a.HoraSalida)
                .HasColumnName("hora_salida");

            builder.Property(a => a.Observacion)
                .HasColumnName("observacion")
                .HasMaxLength(300);

            builder.Property(a => a.Corregida)
                .HasColumnName("corregida")
                .HasDefaultValue(false);

            builder.Property(a => a.MotivoCorreccion)
                .HasColumnName("motivo_correccion")
                .HasMaxLength(300);

            builder.Property(a => a.IdAdministrador)
                .HasColumnName("id_administrador");

            builder.Property(a => a.FechaCorreccion)
                .HasColumnName("fecha_correccion");

            builder.Property(a => a.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(a => a.FechaActualizacion)
                .HasColumnName("fecha_actualizacion");

            builder.HasIndex(a => new { a.IdEmpleado, a.HoraEntrada });

            builder.HasOne(a => a.Empleado)
                .WithMany(e => e.Asistencias)
                .HasForeignKey(a => a.IdEmpleado)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.AdministradorCorreccion)
                .WithMany(a => a.AsistenciasCorregidas)
                .HasForeignKey(a => a.IdAdministrador)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
