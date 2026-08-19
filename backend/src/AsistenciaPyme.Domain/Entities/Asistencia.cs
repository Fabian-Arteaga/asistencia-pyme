using System;
using System.Collections.Generic;
using System.Text;

namespace AsistenciaPyme.Domain.Entities
{
    public class Asistencia
    {
        public int IdAsistencia { get; set; }

        public int IdEmpleado { get; set; }

        public DateTime HoraEntrada { get; set; }

        public DateTime? HoraSalida { get; set; }

        // Tardanza detection fields
        public bool EsEntradaTardia { get; set; }

        public int MinutosTardanza { get; set; }

        public DateTime? HoraProgramadaEntrada { get; set; }

        public DateTime? HoraProgramadaSalida { get; set; }

        public string? Observacion { get; set; }

        public bool Corregida { get; set; }

        public string? MotivoCorreccion { get; set; }

        public int? IdAdministrador { get; set; }

        public DateTime? FechaCorreccion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaActualizacion { get; set; }

        public Empleado Empleado { get; set; } = null!;

        public Administrador? AdministradorCorreccion { get; set; }
    }
}
