using System;
using System.Collections.Generic;

namespace AsistenciaPyme.Domain.Entities
{
    public class Departamento
    {
        public int IdDepartamento { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaActualizacion { get; set; }

        public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();

        public ICollection<Planilla> Planillas { get; set; } = new List<Planilla>();
    }
}
