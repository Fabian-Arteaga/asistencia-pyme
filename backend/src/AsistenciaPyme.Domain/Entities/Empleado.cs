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

        // New: Departamento relationship
        public int? IdDepartamento { get; set; }

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

        // New: Numero INSS (nullable, unique when not null)
        public string? NumeroINSS { get; set; }

        // New: Horario laboral reference
        public int? IdHorarioLaboral { get; set; }

        public EstadoEmpleado Estado { get; set; } = EstadoEmpleado.Activo;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaActualizacion { get; set; }

        // Jerarquía organizacional (Jefe directo y subordinados)
        public int? IdJefeDirecto { get; set; }

        public Cargo Cargo { get; set; } = null!;

        public Empleado? JefeDirecto { get; set; }

        public ICollection<Empleado> Subordinados { get; set; }
            = new List<Empleado>();

        // New relations
        public Departamento? Departamento { get; set; }

        public HorarioLaboral? HorarioLaboral { get; set; }

        public ICollection<HoraExtra> HorasExtras { get; set; } = new List<HoraExtra>();

        public ICollection<Embargo> Embargos { get; set; } = new List<Embargo>();

        public ICollection<EmpleadoDepartamentoHistorial> DepartamentoHistorial { get; set; }
            = new List<EmpleadoDepartamentoHistorial>();

        public ICollection<Asistencia> Asistencias { get; set; }
            = new List<Asistencia>();

        public ICollection<Vacacion> Vacaciones { get; set; }
            = new List<Vacacion>();

        public ICollection<Planilla> Planillas { get; set; }
            = new List<Planilla>();

        public ICollection<EvaluacionDesempeno> EvaluacionesRecibidas { get; set; }
            = new List<EvaluacionDesempeno>();

        public ICollection<EvaluacionDesempeno> EvaluacionesRealizadas { get; set; }
            = new List<EvaluacionDesempeno>();
    }
}
