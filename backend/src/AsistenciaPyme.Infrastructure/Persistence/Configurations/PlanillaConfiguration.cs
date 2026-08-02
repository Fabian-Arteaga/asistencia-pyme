using System;
using System.Collections.Generic;
using System.Text;
using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AsistenciaPyme.Infrastructure.Persistence.Configurations
{
    public class PlanillaConfiguration : IEntityTypeConfiguration<Planilla>
    {
        public void Configure(EntityTypeBuilder<Planilla> builder)
        {
            builder.ToTable("planillas");

            builder.HasKey(p => p.IdPlanilla);

            builder.Property(p => p.IdPlanilla)
                .HasColumnName("id_planilla")
                .ValueGeneratedOnAdd();

            builder.Property(p => p.IdEmpleado)
                .HasColumnName("id_empleado");

            builder.Property(p => p.IdAdministrador)
                .HasColumnName("id_administrador");

            builder.Property(p => p.FechaInicioPeriodo)
                .HasColumnName("fecha_inicio_periodo")
                .HasColumnType("date")
                .IsRequired();

            builder.Property(p => p.FechaFinPeriodo)
                .HasColumnName("fecha_fin_periodo")
                .HasColumnType("date")
                .IsRequired();

            builder.Property(p => p.SalarioBasePeriodo)
                .HasColumnName("salario_base_periodo")
                .HasPrecision(12, 2)
                .IsRequired();

            builder.Property(p => p.IngresosAdicionales)
                .HasColumnName("ingresos_adicionales")
                .HasPrecision(12, 2)
                .HasDefaultValue(0m);

            builder.Property(p => p.TotalDeducciones)
                .HasColumnName("total_deducciones")
                .HasPrecision(12, 2)
                .HasDefaultValue(0m);

            builder.Property(p => p.SalarioNeto)
                .HasColumnName("salario_neto")
                .HasPrecision(12, 2)
                .HasDefaultValue(0m);

            builder.Property(p => p.Estado)
                .HasColumnName("estado")
                .HasConversion<int>()
                .IsRequired();

            builder.Property(p => p.FechaGeneracion)
                .HasColumnName("fecha_generacion");

            builder.Property(p => p.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(p => p.FechaActualizacion)
                .HasColumnName("fecha_actualizacion");

            builder.HasIndex(p => new
            {
                p.IdEmpleado,
                p.FechaInicioPeriodo,
                p.FechaFinPeriodo
            }).IsUnique();

            builder.HasOne(p => p.Empleado)
                .WithMany(e => e.Planillas)
                .HasForeignKey(p => p.IdEmpleado)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Administrador)
                .WithMany(a => a.PlanillasGeneradas)
                .HasForeignKey(p => p.IdAdministrador)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
