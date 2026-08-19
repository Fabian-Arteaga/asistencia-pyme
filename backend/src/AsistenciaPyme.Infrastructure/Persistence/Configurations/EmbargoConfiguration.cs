using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AsistenciaPyme.Infrastructure.Persistence.Configurations
{
    public class EmbargoConfiguration : IEntityTypeConfiguration<Embargo>
    {
        public void Configure(EntityTypeBuilder<Embargo> builder)
        {
            builder.ToTable("Embargos");

            builder.HasKey(e => e.IdEmbargo);

            builder.Property(e => e.IdEmbargo)
                .HasColumnName("IdEmbargo")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.IdEmpleado)
                .HasColumnName("IdEmpleado");

            builder.Property(e => e.FechaInicio)
                .HasColumnName("FechaInicio");

            builder.Property(e => e.FechaFin)
                .HasColumnName("FechaFin");

            builder.Property(e => e.TipoCalculo)
                .HasColumnName("TipoCalculo")
                .HasConversion<int>();

            builder.Property(e => e.Monto)
                .HasPrecision(18, 2)
                .HasColumnName("Monto");

            builder.Property(e => e.Porcentaje)
                .HasPrecision(5, 2)
                .HasColumnName("Porcentaje");

            builder.Property(e => e.SaldoPendiente)
                .HasPrecision(18, 2)
                .HasColumnName("SaldoPendiente");

            builder.Property(e => e.Activo)
                .HasColumnName("Activo");

            builder.Property(e => e.Referencia)
                .HasColumnName("Referencia")
                .HasMaxLength(120);

            builder.Property(e => e.Observacion)
                .HasColumnName("Observacion")
                .HasMaxLength(300);

            builder.HasIndex(e => new { e.IdEmpleado, e.Activo });

            builder.HasOne(e => e.Empleado)
                .WithMany(e => e.Embargos)
                .HasForeignKey(e => e.IdEmpleado)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
