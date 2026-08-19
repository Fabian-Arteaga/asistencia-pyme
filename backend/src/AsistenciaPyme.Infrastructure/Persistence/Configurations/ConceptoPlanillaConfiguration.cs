using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AsistenciaPyme.Infrastructure.Persistence.Configurations
{
    public class ConceptoPlanillaConfiguration : IEntityTypeConfiguration<ConceptoPlanilla>
    {
        public void Configure(EntityTypeBuilder<ConceptoPlanilla> builder)
        {
            builder.ToTable("ConceptosPlanilla");

            builder.HasKey(c => c.IdConceptoPlanilla);

            builder.Property(c => c.IdConceptoPlanilla)
                .HasColumnName("IdConceptoPlanilla")
                .ValueGeneratedOnAdd();

            builder.Property(c => c.Codigo)
                .HasColumnName("Codigo")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.Nombre)
                .HasColumnName("Nombre")
                .HasMaxLength(120)
                .IsRequired();

            builder.Property(c => c.Tipo)
                .HasColumnName("Tipo")
                .HasConversion<int>();

            builder.Property(c => c.Activo)
                .HasColumnName("Activo");

            builder.HasIndex(c => c.Codigo)
                .IsUnique();

            builder.HasData(
                new ConceptoPlanilla { IdConceptoPlanilla = 1, Codigo = "SALARIO_BASE", Nombre = "Salario Base", Tipo = TipoConceptoPlanilla.Ingreso, Activo = true },
                new ConceptoPlanilla { IdConceptoPlanilla = 2, Codigo = "HORA_EXTRA", Nombre = "Hora Extra", Tipo = TipoConceptoPlanilla.Ingreso, Activo = true },
                new ConceptoPlanilla { IdConceptoPlanilla = 3, Codigo = "INSS", Nombre = "INSS", Tipo = TipoConceptoPlanilla.Deduccion, Activo = true },
                new ConceptoPlanilla { IdConceptoPlanilla = 4, Codigo = "EMBARGO", Nombre = "Embargo", Tipo = TipoConceptoPlanilla.Deduccion, Activo = true },
                new ConceptoPlanilla { IdConceptoPlanilla = 5, Codigo = "TARDANZA", Nombre = "Tardanza", Tipo = TipoConceptoPlanilla.Deduccion, Activo = true },
                new ConceptoPlanilla { IdConceptoPlanilla = 6, Codigo = "BONIFICACION", Nombre = "Bonificación", Tipo = TipoConceptoPlanilla.Ingreso, Activo = true },
                new ConceptoPlanilla { IdConceptoPlanilla = 7, Codigo = "OTRA_DEDUCCION", Nombre = "Otra Deducción", Tipo = TipoConceptoPlanilla.Deduccion, Activo = true }
            );
        }
    }
}
