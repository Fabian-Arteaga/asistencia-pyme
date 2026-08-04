using AsistenciaPyme.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Common.Interfaces
{
    public interface IAsistenciaPymeDbContext
    {
        DbSet<Cargo> Cargos { get; }

        DbSet<Empleado> Empleados { get; }

        DbSet<Asistencia> Asistencias { get; }

        DbSet<Administrador> Administradores { get; }

        DbSet<Vacacion> Vacaciones { get; }

        Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
    }
}
