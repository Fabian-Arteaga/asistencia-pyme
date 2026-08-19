using AsistenciaPyme.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace AsistenciaPyme.Infrastructure.Persistence.Configurations;

public class CriterioEvaluacionConfiguration : IEntityTypeConfiguration<CriterioEvaluacion>
{
    public void Configure(EntityTypeBuilder<CriterioEvaluacion> builder)
    {
        builder.ToTable("criterios_evaluacion");

        builder.HasKey(c => c.IdCriterioEvaluacion);

        builder.Property(c => c.IdCriterioEvaluacion)
            .HasColumnName("id_criterio_evaluacion")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.IdCategoriaEvaluacion)
            .HasColumnName("id_categoria_evaluacion")
            .IsRequired();

        builder.Property(c => c.Texto)
            .HasColumnName("texto")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(c => c.Descripcion)
            .HasColumnName("descripcion")
            .HasMaxLength(250);

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

        builder.HasOne(c => c.Categoria)
            .WithMany(cat => cat.Criterios)
            .HasForeignKey(c => c.IdCategoriaEvaluacion)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed inicial de preguntas/criterios estándar
        builder.HasData(
            // Rendimiento (IdCategoria: 1)
            new CriterioEvaluacion { IdCriterioEvaluacion = 1, IdCategoriaEvaluacion = 1, Texto = "Cumple adecuadamente con sus funciones y tareas asignadas.", Orden = 1, Activo = true, FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new CriterioEvaluacion { IdCriterioEvaluacion = 2, IdCategoriaEvaluacion = 1, Texto = "Mantiene un nivel de productividad y calidad adecuado.", Orden = 2, Activo = true, FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new CriterioEvaluacion { IdCriterioEvaluacion = 3, IdCategoriaEvaluacion = 1, Texto = "Entrega su trabajo en los tiempos establecidos.", Orden = 3, Activo = true, FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

            // Objetivos (IdCategoria: 2)
            new CriterioEvaluacion { IdCriterioEvaluacion = 4, IdCategoriaEvaluacion = 2, Texto = "Alcanza los objetivos y metas planteados para el período.", Orden = 1, Activo = true, FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new CriterioEvaluacion { IdCriterioEvaluacion = 5, IdCategoriaEvaluacion = 2, Texto = "Alinea sus actividades diarias con las prioridades del negocio.", Orden = 2, Activo = true, FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

            // Responsabilidad (IdCategoria: 3)
            new CriterioEvaluacion { IdCriterioEvaluacion = 6, IdCategoriaEvaluacion = 3, Texto = "Demuestra puntualidad, asistencia y cumplimiento de horarios.", Orden = 1, Activo = true, FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new CriterioEvaluacion { IdCriterioEvaluacion = 7, IdCategoriaEvaluacion = 3, Texto = "Cuida adecuadamente las herramientas y recursos de la empresa.", Orden = 2, Activo = true, FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new CriterioEvaluacion { IdCriterioEvaluacion = 8, IdCategoriaEvaluacion = 3, Texto = "Asume la responsabilidad de sus resultados y decisiones.", Orden = 3, Activo = true, FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

            // Competencias (IdCategoria: 4)
            new CriterioEvaluacion { IdCriterioEvaluacion = 9, IdCategoriaEvaluacion = 4, Texto = "Demuestra dominio técnico en los conocimientos de su cargo.", Orden = 1, Activo = true, FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new CriterioEvaluacion { IdCriterioEvaluacion = 10, IdCategoriaEvaluacion = 4, Texto = "Aplica procedimientos y buenas prácticas en su área.", Orden = 2, Activo = true, FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

            // Aptitudes (IdCategoria: 5)
            new CriterioEvaluacion { IdCriterioEvaluacion = 11, IdCategoriaEvaluacion = 5, Texto = "Trabaja en equipo y colabora activamente con sus compañeros.", Orden = 1, Activo = true, FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new CriterioEvaluacion { IdCriterioEvaluacion = 12, IdCategoriaEvaluacion = 5, Texto = "Se comunica de forma asertiva, respetuosa y clara.", Orden = 2, Activo = true, FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

            // Iniciativa (IdCategoria: 6)
            new CriterioEvaluacion { IdCriterioEvaluacion = 13, IdCategoriaEvaluacion = 6, Texto = "Actúa con autonomía y proactividad sin esperar indicaciones constantes.", Orden = 1, Activo = true, FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new CriterioEvaluacion { IdCriterioEvaluacion = 14, IdCategoriaEvaluacion = 6, Texto = "Propone soluciones cuando se presentan problemas o dificultades.", Orden = 2, Activo = true, FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

            // Creatividad (IdCategoria: 7)
            new CriterioEvaluacion { IdCriterioEvaluacion = 15, IdCategoriaEvaluacion = 7, Texto = "Aporta ideas novedosas para simplificar o mejorar procesos.", Orden = 1, Activo = true, FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new CriterioEvaluacion { IdCriterioEvaluacion = 16, IdCategoriaEvaluacion = 7, Texto = "Muestra flexibilidad e ingenio ante nuevos retos de trabajo.", Orden = 2, Activo = true, FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
