using AsistenciaPyme.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsistenciaPyme.Domain.Entities
{
    public class Empleado
    {
        public int IdEmpleado { get; set; }

        public int IdCargo { get; set; }

        public string CodigoEmpleado { get; set; } = string.Empty;

        public string PinHash { get; set; } = string.Empty;

        public string Identificacion { get; set; } = string.Empty;

        public string Nombres { get; set; } = string.Empty;

        public string Apellidos { get; set; } = string.Empty;

        public string? Telefono { get; set; }

        public string? Correo { get; set; }

        public string? Direccion { get; set; }

        public DateOnly FechaContratacion { get; set; }

        public decimal SalarioBase { get; set; }

        public EstadoEmpleado Estado { get; set; } = EstadoEmpleado.Activo;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaActualizacion { get; set; }

        public Cargo Cargo { get; set; } = null!;

        public ICollection<Asistencia> Asistencias { get; set; }
            = new List<Asistencia>();

        public ICollection<Vacacion> Vacaciones { get; set; }
            = new List<Vacacion>();

        public ICollection<Planilla> Planillas { get; set; }
            = new List<Planilla>();
    }
}
