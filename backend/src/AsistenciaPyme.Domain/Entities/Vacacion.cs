using System;
using System.Collections.Generic;
using System.Text;

namespace AsistenciaPyme.Domain.Entities
{
    public class Vacacion
    {
        public int IdVacacion { get; set; }

        public int IdEmpleado { get; set; }

        public int IdAdministrador { get; set; }

        public DateOnly FechaInicio { get; set; }

        public DateOnly FechaFin { get; set; }

        public string? Motivo { get; set; }

        public string? Observacion { get; set; }

        public bool Cancelada { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaActualizacion { get; set; }

        public Empleado Empleado { get; set; } = null!;

        public Administrador Administrador { get; set; } = null!;
    }
}
