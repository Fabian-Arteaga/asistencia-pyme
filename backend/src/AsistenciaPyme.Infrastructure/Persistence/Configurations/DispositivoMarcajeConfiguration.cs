using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AsistenciaPyme.Infrastructure.Persistence.Configurations
{
    public class DispositivoMarcajeConfiguration : IEntityTypeConfiguration<DispositivoMarcaje>
    {
        public void Configure(EntityTypeBuilder<DispositivoMarcaje> builder)
        {
            builder.ToTable("DispositivosMarcaje");

            builder.HasKey(d => d.IdDispositivoMarcaje);

            builder.Property(d => d.IdDispositivoMarcaje)
                .HasColumnName("IdDispositivoMarcaje")
                .ValueGeneratedOnAdd();

            builder.Property(d => d.Nombre)
                .HasColumnName("Nombre")
                .HasMaxLength(120)
                .IsRequired();

            builder.Property(d => d.Tipo)
                .HasColumnName("Tipo")
                .HasConversion<int>();

            builder.Property(d => d.IdDepartamento)
                .HasColumnName("IdDepartamento");

            builder.Property(d => d.Identificador)
                .HasColumnName("Identificador")
                .HasMaxLength(120)
                .IsRequired();

            builder.Property(d => d.Activo)
                .HasColumnName("Activo");

            builder.HasOne(d => d.Departamento)
                .WithMany()
                .HasForeignKey(d => d.IdDepartamento)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
