using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AsistenciaPyme.Infrastructure.Persistence.Configurations
{
    public class HoraExtraConfiguration : IEntityTypeConfiguration<HoraExtra>
    {
        public void Configure(EntityTypeBuilder<HoraExtra> builder)
        {
            builder.ToTable("HorasExtras");

            builder.HasKey(h => h.IdHoraExtra);

            builder.Property(h => h.IdHoraExtra)
                .HasColumnName("IdHoraExtra")
                .ValueGeneratedOnAdd();

            builder.Property(h => h.IdEmpleado)
                .HasColumnName("IdEmpleado");

            builder.Property(h => h.IdAsistencia)
                .HasColumnName("IdAsistencia");

            builder.Property(h => h.Fecha)
                .HasColumnName("Fecha")
                .HasColumnType("date");

            builder.Property(h => h.MinutosDetectados)
                .HasColumnName("MinutosDetectados");

            builder.Property(h => h.MinutosAprobados)
                .HasColumnName("MinutosAprobados");

            builder.Property(h => h.Estado)
                .HasColumnName("Estado")
                .HasConversion<int>();

            builder.Property(h => h.AprobadoPor)
                .HasColumnName("AprobadoPor");

            builder.Property(h => h.FechaAprobacion)
                .HasColumnName("FechaAprobacion");

            builder.Property(h => h.Observacion)
                .HasColumnName("Observacion")
                .HasMaxLength(300);

            builder.HasIndex(h => new { h.IdEmpleado, h.Fecha });

            builder.HasOne(h => h.Empleado)
                .WithMany(e => e.HorasExtras)
                .HasForeignKey(h => h.IdEmpleado)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(h => h.Asistencia)
                .WithMany()
                .HasForeignKey(h => h.IdAsistencia)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
