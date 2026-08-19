using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace AsistenciaPyme.Infrastructure.Persistence.Configurations;

public class CategoriaEvaluacionConfiguration : IEntityTypeConfiguration<CategoriaEvaluacion>
{
    public void Configure(EntityTypeBuilder<CategoriaEvaluacion> builder)
    {
        builder.ToTable("categorias_evaluacion");

        builder.HasKey(c => c.IdCategoriaEvaluacion);

        builder.Property(c => c.IdCategoriaEvaluacion)
            .HasColumnName("id_categoria_evaluacion")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(c => c.Nombre)
            .IsUnique();

        builder.Property(c => c.Descripcion)
            .HasColumnName("descripcion")
            .HasMaxLength(250);

        builder.Property(c => c.Ponderacion)
            .HasColumnName("ponderacion")
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(c => c.Orden)
            .HasColumnName("orden")
            .IsRequired();

        builder.Property(c => c.Activo)
            .HasColumnName("activo")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(c => c.FechaCreacion)
            .HasColumnName("fecha_creacion")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(c => c.FechaActualizacion)
            .HasColumnName("fecha_actualizacion");

        // Seed de las 7 categorías obligatorias con configuración inicial editable que suma 100%
        builder.HasData(
            new CategoriaEvaluacion
            {
                IdCategoriaEvaluacion = 1,
                Nombre = "Rendimiento",
                Descripcion = "Evaluación de la productividad, calidad del trabajo y cumplimiento en tiempos establecidos.",
                Ponderacion = 20.00m,
                Orden = 1,
                Activo = true,
                FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new CategoriaEvaluacion
            {
                IdCategoriaEvaluacion = 2,
                Nombre = "Objetivos",
                Descripcion = "Grado de consecución de metas individuales y alineación con objetivos del negocio.",
                Ponderacion = 15.00m,
                Orden = 2,
                Activo = true,
                FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new CategoriaEvaluacion
            {
                IdCategoriaEvaluacion = 3,
                Nombre = "Responsabilidad",
                Descripcion = "Compromiso con las normas, puntualidad, asistencia y cuidado de recursos de la empresa.",
                Ponderacion = 15.00m,
                Orden = 3,
                Activo = true,
                FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new CategoriaEvaluacion
            {
                IdCategoriaEvaluacion = 4,
                Nombre = "Competencias",
                Descripcion = "Conocimientos técnicos, habilidades específicas del puesto y resolución de problemas.",
                Ponderacion = 15.00m,
                Orden = 4,
                Activo = true,
                FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new CategoriaEvaluacion
            {
                IdCategoriaEvaluacion = 5,
                Nombre = "Aptitudes",
                Descripcion = "Trabajo en equipo, comunicación asertiva, empatía y adaptabilidad a cambios.",
                Ponderacion = 10.00m,
                Orden = 5,
                Activo = true,
                FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new CategoriaEvaluacion
            {
                IdCategoriaEvaluacion = 6,
                Nombre = "Iniciativa",
                Descripcion = "Proactividad, autonomía para proponer mejoras y anticipación a requerimientos.",
                Ponderacion = 15.00m,
                Orden = 6,
                Activo = true,
                FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new CategoriaEvaluacion
            {
                IdCategoriaEvaluacion = 7,
                Nombre = "Creatividad",
                Descripcion = "Aporte de ideas innovadoras y alternativas eficaces para optimizar procesos.",
                Ponderacion = 10.00m,
                Orden = 7,
                Activo = true,
                FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
