using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using AsistenciaPyme.Application.Common.Interfaces;

namespace AsistenciaPyme.Infrastructure.Persistence
{
    public class AsistenciaPymeDbContext : DbContext, IAsistenciaPymeDbContext
    {
        public AsistenciaPymeDbContext(
       DbContextOptions<AsistenciaPymeDbContext> options)
       : base(options)
        {
        }

        public DbSet<Administrador> Administradores => Set<Administrador>();

        public DbSet<Cargo> Cargos => Set<Cargo>();

        public DbSet<Empleado> Empleados => Set<Empleado>();

        public DbSet<Asistencia> Asistencias => Set<Asistencia>();

        public DbSet<Vacacion> Vacaciones => Set<Vacacion>();

        public DbSet<TipoDeduccion> TiposDeduccion => Set<TipoDeduccion>();

        public DbSet<Planilla> Planillas => Set<Planilla>();

        public DbSet<DeduccionPlanilla> DeduccionesPlanilla =>
            Set<DeduccionPlanilla>();

        // New domain sets
        public DbSet<Departamento> Departamentos => Set<Departamento>();

        public DbSet<HorarioLaboral> HorariosLaborales => Set<HorarioLaboral>();

        public DbSet<DetallePlanilla> DetallesPlanilla => Set<DetallePlanilla>();

        public DbSet<ConceptoPlanilla> ConceptosPlanilla => Set<ConceptoPlanilla>();

        public DbSet<DetalleConceptoPlanilla> DetallesConceptoPlanilla => Set<DetalleConceptoPlanilla>();

        public DbSet<HoraExtra> HorasExtras => Set<HoraExtra>();

        public DbSet<Embargo> Embargos => Set<Embargo>();

        public DbSet<ConfiguracionNomina> ConfiguracionesNomina => Set<ConfiguracionNomina>();

        public DbSet<EmpleadoDepartamentoHistorial> EmpleadoDepartamentoHistorials => Set<EmpleadoDepartamentoHistorial>();

        public DbSet<DispositivoMarcaje> DispositivosMarcaje => Set<DispositivoMarcaje>();

        public DbSet<VerificacionPresencia> VerificacionesPresencia => Set<VerificacionPresencia>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AsistenciaPymeDbContext).Assembly);

            modelBuilder.Entity<Administrador>(b => b.HasKey(a => a.IdAdministrador));
            modelBuilder.Entity<Cargo>(b => b.HasKey(c => c.IdCargo));
            modelBuilder.Entity<Departamento>(b => b.HasKey(d => d.IdDepartamento));
            modelBuilder.Entity<Empleado>(b =>
            {
                b.HasKey(e => e.IdEmpleado);
                b.HasIndex(e => e.IdCargo);
                b.HasIndex(e => e.IdDepartamento);
                b.Property(e => e.SalarioBase).HasColumnType("numeric(18,2)");
                b.HasIndex(e => e.NumeroINSS).IsUnique().HasFilter("\"NumeroINSS\" IS NOT NULL");
            });
            modelBuilder.Entity<Asistencia>(b =>
            {
                b.HasKey(a => a.IdAsistencia);
                b.HasIndex(a => new { a.IdEmpleado, a.HoraEntrada });
            });
            modelBuilder.Entity<Vacacion>(b => b.HasKey(v => v.IdVacacion));
            modelBuilder.Entity<TipoDeduccion>(b => b.HasKey(t => t.IdTipoDeduccion));
            modelBuilder.Entity<Planilla>(b =>
            {
                b.HasKey(p => p.IdPlanilla);
                b.HasIndex(p => new { p.IdDepartamento, p.FechaInicioPeriodo, p.FechaFinPeriodo }).IsUnique();
                b.Property(p => p.TotalSalarioBase).HasColumnType("numeric(18,2)");
                b.Property(p => p.TotalHorasExtras).HasColumnType("numeric(18,2)");
                b.Property(p => p.TotalIngresos).HasColumnType("numeric(18,2)");
                b.Property(p => p.TotalDeducciones).HasColumnType("numeric(18,2)");
                b.Property(p => p.TotalNeto).HasColumnType("numeric(18,2)");
            });
            modelBuilder.Entity<DeduccionPlanilla>(b => b.HasKey(d => d.IdDeduccionPlanilla));
            modelBuilder.Entity<DetallePlanilla>(b =>
            {
                b.HasKey(d => d.IdDetallePlanilla);
                b.HasIndex(d => d.IdPlanilla);
                b.HasIndex(d => d.IdEmpleado);
                b.Property(d => d.SalarioBase).HasColumnType("numeric(18,2)");
                b.Property(d => d.MontoHorasExtras).HasColumnType("numeric(18,2)");
                b.Property(d => d.TotalIngresos).HasColumnType("numeric(18,2)");
                b.Property(d => d.TotalDeducciones).HasColumnType("numeric(18,2)");
                b.Property(d => d.SalarioNeto).HasColumnType("numeric(18,2)");
            });
            modelBuilder.Entity<HorarioLaboral>(b => b.HasKey(h => h.IdHorarioLaboral));
            modelBuilder.Entity<HoraExtra>(b =>
            {
                b.HasKey(h => h.IdHoraExtra);
                b.HasIndex(h => new { h.IdEmpleado, h.Fecha });
                b.Property(h => h.MinutosDetectados).HasColumnType("int");
                b.Property(h => h.MinutosAprobados).HasColumnType("int");
            });
            modelBuilder.Entity<Embargo>(b =>
            {
                b.HasKey(e => e.IdEmbargo);
                b.HasIndex(e => new { e.IdEmpleado, e.Activo });
                b.Property(e => e.Monto).HasColumnType("numeric(18,2)");
                b.Property(e => e.Porcentaje).HasColumnType("numeric(5,2)");
                b.Property(e => e.SaldoPendiente).HasColumnType("numeric(18,2)");
            });
            modelBuilder.Entity<ConfiguracionNomina>(b =>
            {
                b.HasKey(c => c.IdConfiguracionNomina);
            });
            modelBuilder.Entity<EmpleadoDepartamentoHistorial>(b =>
            {
                b.HasKey(h => h.Id);
                b.HasIndex(h => h.IdEmpleado);
                b.HasIndex(h => h.IdDepartamento);
            });
            modelBuilder.Entity<ConceptoPlanilla>(b =>
            {
                b.HasKey(c => c.IdConceptoPlanilla);
                b.Property(c => c.Codigo).HasMaxLength(50);
                b.Property(c => c.Nombre).HasMaxLength(120);
            });
            modelBuilder.Entity<DetalleConceptoPlanilla>(b =>
            {
                b.HasKey(d => d.IdDetalleConceptoPlanilla);
                b.Property(d => d.Monto).HasColumnType("numeric(18,2)");
            });
            modelBuilder.Entity<DispositivoMarcaje>(b => b.HasKey(d => d.IdDispositivoMarcaje));
            modelBuilder.Entity<VerificacionPresencia>(b =>
            {
                b.HasKey(v => v.IdVerificacionPresencia);
                b.HasIndex(v => v.IdDispositivoMarcaje);
            });
        }
    }
}
