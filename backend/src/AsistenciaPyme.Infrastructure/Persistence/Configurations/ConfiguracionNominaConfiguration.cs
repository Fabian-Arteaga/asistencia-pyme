using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AsistenciaPyme.Infrastructure.Persistence.Configurations
{
    public class ConfiguracionNominaConfiguration : IEntityTypeConfiguration<ConfiguracionNomina>
    {
        public void Configure(EntityTypeBuilder<ConfiguracionNomina> builder)
        {
            builder.ToTable("ConfiguracionesNomina");

            builder.HasKey(c => c.IdConfiguracionNomina);

            builder.Property(c => c.IdConfiguracionNomina)
                .HasColumnName("IdConfiguracionNomina")
                .ValueGeneratedOnAdd();

            builder.Property(c => c.MinutosToleranciaEntrada)
                .HasColumnName("MinutosToleranciaEntrada")
                .IsRequired();

            builder.Property(c => c.MultiplicadorHoraExtra)
                .HasPrecision(18, 2)
                .HasColumnName("MultiplicadorHoraExtra")
                .IsRequired();

            builder.Property(c => c.DiasVacacionesPorMes)
                .HasPrecision(18, 2)
                .HasColumnName("DiasVacacionesPorMes")
                .IsRequired();

            builder.Property(c => c.DiasBaseProrrateoVacaciones)
                .HasColumnName("DiasBaseProrrateoVacaciones")
                .IsRequired();

            builder.Property(c => c.PoliticaDescuentoTardanza)
                .HasColumnName("PoliticaDescuentoTardanza")
                .HasConversion<int>()
                .IsRequired();

            builder.Property(c => c.MinutosMaximosVerificacionDepartamento)
                .HasColumnName("MinutosMaximosVerificacionDepartamento")
                .IsRequired();

            builder.Property(c => c.TasaINSS)
                .HasPrecision(18, 2)
                .HasColumnName("TasaINSS")
                .IsRequired(false);

            builder.Property(c => c.FechaCreacion)
                .HasColumnName("FechaCreacion")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasData(new ConfiguracionNomina
            {
                IdConfiguracionNomina = 1,
                MinutosToleranciaEntrada = 10,
                MultiplicadorHoraExtra = 2.0m,
                DiasVacacionesPorMes = 2.5m,
                DiasBaseProrrateoVacaciones = 30,
                PoliticaDescuentoTardanza = PoliticaDescuentoTardanza.SinDeduccion,
                MinutosMaximosVerificacionDepartamento = 10,
                TasaINSS = null,
                FechaCreacion = new DateTime(2026, 8, 19, 0, 0, 0, DateTimeKind.Utc)
            });
        }
    }
}
