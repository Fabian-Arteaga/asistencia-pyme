using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AsistenciaPyme.Infrastructure.Persistence.Configurations
{
    public class EmpleadoDepartamentoHistorialConfiguration : IEntityTypeConfiguration<EmpleadoDepartamentoHistorial>
    {
        public void Configure(EntityTypeBuilder<EmpleadoDepartamentoHistorial> builder)
        {
            builder.ToTable("EmpleadoDepartamentoHistorials");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.Id)
                .HasColumnName("IdEmpleadoDepartamentoHistorial")
                .ValueGeneratedOnAdd();

            builder.Property(h => h.IdEmpleado)
                .HasColumnName("IdEmpleado");

            builder.Property(h => h.IdDepartamento)
                .HasColumnName("IdDepartamento");

            builder.Property(h => h.FechaInicio)
                .HasColumnName("FechaInicio");

            builder.Property(h => h.FechaFin)
                .HasColumnName("FechaFin");

            builder.HasIndex(h => h.IdEmpleado);
            builder.HasIndex(h => h.IdDepartamento);

            builder.HasOne(h => h.Empleado)
                .WithMany(e => e.DepartamentoHistorial)
                .HasForeignKey(h => h.IdEmpleado)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(h => h.Departamento)
                .WithMany()
                .HasForeignKey(h => h.IdDepartamento)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
