using System;

namespace AsistenciaPyme.Domain.Entities
{
    public class EmpleadoDepartamentoHistorial
    {
        public int Id { get; set; }

        public int IdEmpleado { get; set; }

        public int IdDepartamento { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public Empleado Empleado { get; set; } = null!;

        public Departamento Departamento { get; set; } = null!;
    }
}
