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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AsistenciaPymeDbContext).Assembly);
        }
    }
}
