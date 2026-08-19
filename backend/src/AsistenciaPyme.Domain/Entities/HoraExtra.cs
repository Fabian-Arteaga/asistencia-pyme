using System;

namespace AsistenciaPyme.Domain.Entities
{
    public enum EstadoHoraExtra
    {
        Pendiente = 1,
        Aprobada = 2,
        Rechazada = 3
    }

    public class HoraExtra
    {
        public int IdHoraExtra { get; set; }

        public int IdEmpleado { get; set; }

        public int? IdAsistencia { get; set; }

        public DateTime Fecha { get; set; }

        // Stored in minutes
        public int MinutosDetectados { get; set; }

        public int MinutosAprobados { get; set; }

        public EstadoHoraExtra Estado { get; set; } = EstadoHoraExtra.Pendiente;

        public int? AprobadoPor { get; set; }

        public DateTime? FechaAprobacion { get; set; }

        public string? Observacion { get; set; }

        public Empleado Empleado { get; set; } = null!;

        public Asistencia? Asistencia { get; set; }
    }
}
