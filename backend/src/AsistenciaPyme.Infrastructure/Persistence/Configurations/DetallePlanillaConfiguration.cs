using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AsistenciaPyme.Infrastructure.Persistence.Configurations
{
    public class DetallePlanillaConfiguration : IEntityTypeConfiguration<DetallePlanilla>
    {
        public void Configure(EntityTypeBuilder<DetallePlanilla> builder)
        {
            builder.ToTable("DetallesPlanilla");

            builder.HasKey(d => d.IdDetallePlanilla);

            builder.Property(d => d.IdDetallePlanilla)
                .HasColumnName("IdDetallePlanilla")
                .ValueGeneratedOnAdd();

            builder.Property(d => d.IdPlanilla)
                .HasColumnName("IdPlanilla");

            builder.Property(d => d.IdEmpleado)
                .HasColumnName("IdEmpleado");

            builder.Property(d => d.CodigoEmpleado)
                .HasColumnName("CodigoEmpleado")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(d => d.NombreEmpleado)
                .HasColumnName("NombreEmpleado")
                .HasMaxLength(120)
                .IsRequired();

            builder.Property(d => d.NumeroINSS)
                .HasColumnName("NumeroINSS")
                .HasMaxLength(20);

            builder.Property(d => d.Cargo)
                .HasColumnName("Cargo")
                .HasMaxLength(120)
                .IsRequired();

            builder.Property(d => d.Departamento)
                .HasColumnName("Departamento")
                .HasMaxLength(120)
                .IsRequired();

            builder.Property(d => d.SalarioBase)
                .HasPrecision(18, 2)
                .HasColumnName("SalarioBase");

            builder.Property(d => d.DiasLaborados)
                .HasColumnName("DiasLaborados");

            builder.Property(d => d.MinutosLaborados)
                .HasColumnName("MinutosLaborados");

            builder.Property(d => d.CantidadTardanzas)
                .HasColumnName("CantidadTardanzas");

            builder.Property(d => d.MinutosTardanza)
                .HasColumnName("MinutosTardanza");

            builder.Property(d => d.DescuentoTardanza)
                .HasPrecision(18, 2)
                .HasColumnName("DescuentoTardanza");

            builder.Property(d => d.MinutosExtrasDetectados)
                .HasColumnName("MinutosExtrasDetectados");

            builder.Property(d => d.MinutosExtrasAprobados)
                .HasColumnName("MinutosExtrasAprobados");

            builder.Property(d => d.MontoHorasExtras)
                .HasPrecision(18, 2)
                .HasColumnName("MontoHorasExtras");

            builder.Property(d => d.VacacionesAcumuladasPeriodo)
                .HasPrecision(18, 2)
                .HasColumnName("VacacionesAcumuladasPeriodo");

            builder.Property(d => d.SaldoVacaciones)
                .HasPrecision(18, 2)
                .HasColumnName("SaldoVacaciones");

            builder.Property(d => d.TotalIngresos)
                .HasPrecision(18, 2)
                .HasColumnName("TotalIngresos");

            builder.Property(d => d.TotalDeducciones)
                .HasPrecision(18, 2)
                .HasColumnName("TotalDeducciones");

            builder.Property(d => d.SalarioNeto)
                .HasPrecision(18, 2)
                .HasColumnName("SalarioNeto");

            builder.Property(d => d.IndemnizacionProyectada)
                .HasPrecision(18, 2)
                .IsRequired(false)
                .HasColumnName("IndemnizacionProyectada");

            builder.Property(d => d.FechaCreacion)
                .HasColumnName("FechaCreacion")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasIndex(d => d.IdPlanilla);
            builder.HasIndex(d => d.IdEmpleado);
            builder.HasIndex(d => new
            {
                d.IdPlanilla,
                d.IdEmpleado
            }).IsUnique();

            builder.HasOne(d => d.Planilla)
                .WithMany(p => p.Detalles)
                .HasForeignKey(d => d.IdPlanilla)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Empleado)
                .WithMany()
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(d => d.Conceptos)
                .WithOne(c => c.DetallePlanilla)
                .HasForeignKey(c => c.IdDetallePlanilla)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
