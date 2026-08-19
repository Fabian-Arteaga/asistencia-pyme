using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AsistenciaPyme.Infrastructure.Persistence.Configurations
{
    public class VerificacionPresenciaConfiguration : IEntityTypeConfiguration<VerificacionPresencia>
    {
        public void Configure(EntityTypeBuilder<VerificacionPresencia> builder)
        {
            builder.ToTable("VerificacionesPresencia");

            builder.HasKey(v => v.IdVerificacionPresencia);

            builder.Property(v => v.IdVerificacionPresencia)
                .HasColumnName("IdVerificacionPresencia")
                .ValueGeneratedOnAdd();

            builder.Property(v => v.IdEmpleado)
                .HasColumnName("IdEmpleado");

            builder.Property(v => v.IdAsistencia)
                .HasColumnName("IdAsistencia");

            builder.Property(v => v.IdDepartamento)
                .HasColumnName("IdDepartamento");

            builder.Property(v => v.IdDispositivoMarcaje)
                .HasColumnName("IdDispositivoMarcaje");

            builder.Property(v => v.FechaHora)
                .HasColumnName("FechaHora");

            builder.Property(v => v.Estado)
                .HasColumnName("Estado")
                .HasConversion<int>();

            builder.HasIndex(v => v.IdDispositivoMarcaje);

            builder.HasOne(v => v.Empleado)
                .WithMany()
                .HasForeignKey(v => v.IdEmpleado)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Asistencia)
                .WithMany()
                .HasForeignKey(v => v.IdAsistencia)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(v => v.Departamento)
                .WithMany()
                .HasForeignKey(v => v.IdDepartamento)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(v => v.Dispositivo)
                .WithMany()
                .HasForeignKey(v => v.IdDispositivoMarcaje)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
