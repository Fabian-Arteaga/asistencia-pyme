using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AsistenciaPyme.Infrastructure.Persistence.Configurations
{
    public class DetalleConceptoPlanillaConfiguration : IEntityTypeConfiguration<DetalleConceptoPlanilla>
    {
        public void Configure(EntityTypeBuilder<DetalleConceptoPlanilla> builder)
        {
            builder.ToTable("DetallesConceptoPlanilla");

            builder.HasKey(d => d.IdDetalleConceptoPlanilla);

            builder.Property(d => d.IdDetalleConceptoPlanilla)
                .HasColumnName("IdDetalleConceptoPlanilla")
                .ValueGeneratedOnAdd();

            builder.Property(d => d.IdDetallePlanilla)
                .HasColumnName("IdDetallePlanilla");

            builder.Property(d => d.IdConceptoPlanilla)
                .HasColumnName("IdConceptoPlanilla");

            builder.Property(d => d.Monto)
                .HasColumnType("numeric(18,2)")
                .HasColumnName("Monto");

            builder.Property(d => d.Descripcion)
                .HasColumnName("Descripcion");

            builder.HasOne(d => d.DetallePlanilla)
                .WithMany(d => d.Conceptos)
                .HasForeignKey(d => d.IdDetallePlanilla)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(d => d.IdDetallePlanilla);
            builder.HasIndex(d => d.IdConceptoPlanilla);

            builder.HasOne(d => d.ConceptoPlanilla)
                .WithMany(c => c.Detalles)
                .HasForeignKey(d => d.IdConceptoPlanilla)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
