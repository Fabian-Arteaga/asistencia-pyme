using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace AsistenciaPyme.Infrastructure.Persistence.Configurations
{
    public class AdministradorConfiguration : IEntityTypeConfiguration<Administrador>
    {
        public void Configure(EntityTypeBuilder<Administrador> builder)
        {
            builder.ToTable("administradores");

            builder.HasKey(a => a.IdAdministrador);

            builder.Property(a => a.IdAdministrador)
                .HasColumnName("id_administrador")
                .ValueGeneratedOnAdd();

            builder.Property(a => a.Nombres)
                .HasColumnName("nombres")
                .HasMaxLength(80)
                .IsRequired();

            builder.Property(a => a.Apellidos)
                .HasColumnName("apellidos")
                .HasMaxLength(80)
                .IsRequired();

            builder.Property(a => a.Correo)
                .HasColumnName("correo")
                .HasMaxLength(150)
                .IsRequired();

            builder.HasIndex(a => a.Correo)
                .IsUnique();

            builder.Property(a => a.ContrasenaHash)
                .HasColumnName("contrasena_hash")
                .IsRequired();

            builder.Property(a => a.Activo)
                .HasColumnName("activo")
                .HasDefaultValue(true);

            builder.Property(a => a.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(a => a.FechaActualizacion)
                .HasColumnName("fecha_actualizacion");
        }
    }
}
