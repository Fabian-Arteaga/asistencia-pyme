using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
namespace AsistenciaPyme.Infrastructure.Persistence.Configurations
{
    public class EmpleadoConfiguration : IEntityTypeConfiguration<Empleado>
    {
        public void Configure(EntityTypeBuilder<Empleado> builder)
        {
            builder.ToTable("empleados");

            builder.HasKey(e => e.IdEmpleado);

            builder.Property(e => e.IdEmpleado)
                .HasColumnName("id_empleado")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.IdCargo)
                .HasColumnName("id_cargo");

            builder.Property(e => e.IdDepartamento)
                .HasColumnName("id_departamento");

            builder.Property(e => e.IdHorarioLaboral)
                .HasColumnName("id_horario_laboral");

            builder.Property(e => e.IdJefeDirecto)
                .HasColumnName("id_jefe_directo");

            builder.Property(e => e.CodigoEmpleado)
                .HasColumnName("codigo_empleado")
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(e => e.CodigoEmpleado)
                .IsUnique();

            builder.Property(e => e.PinHash)
                .HasColumnName("pin_hash")
                .IsRequired();

            builder.Property(e => e.Identificacion)
                .HasColumnName("identificacion")
                .HasMaxLength(30)
                .IsRequired();

            builder.HasIndex(e => e.Identificacion)
                .IsUnique();

            builder.Property(e => e.Nombres)
                .HasColumnName("nombres")
                .HasMaxLength(80)
                .IsRequired();

            builder.Property(e => e.Apellidos)
                .HasColumnName("apellidos")
                .HasMaxLength(80)
                .IsRequired();

            builder.Property(e => e.Telefono)
                .HasColumnName("telefono")
                .HasMaxLength(20);

            builder.Property(e => e.Correo)
                .HasColumnName("correo")
                .HasMaxLength(150);

            builder.HasIndex(e => e.Correo)
                .IsUnique();

            builder.Property(e => e.Direccion)
                .HasColumnName("direccion")
                .HasMaxLength(250);

            builder.Property(e => e.FechaContratacion)
                .HasColumnName("fecha_contratacion")
                .HasColumnType("date")
                .IsRequired();

            builder.Property(e => e.SalarioBase)
                .HasColumnName("salario_base")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(e => e.Estado)
                .HasColumnName("estado")
                .HasConversion<int>()
                .IsRequired();

            builder.Property(e => e.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(e => e.FechaActualizacion)
                .HasColumnName("fecha_actualizacion");

            builder.HasOne(e => e.Cargo)
                .WithMany(c => c.Empleados)
                .HasForeignKey(e => e.IdCargo)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Departamento)
                .WithMany(d => d.Empleados)
                .HasForeignKey(e => e.IdDepartamento)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.HorarioLaboral)
                .WithMany(h => h.Empleados)
                .HasForeignKey(e => e.IdHorarioLaboral)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.JefeDirecto)
                .WithMany(j => j.Subordinados)
                .HasForeignKey(e => e.IdJefeDirecto)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.NumeroINSS)
                .IsUnique()
                .HasFilter("\"NumeroINSS\" IS NOT NULL");
        }
    }
}
