using System;
using System.Collections.Generic;

namespace AsistenciaPyme.Domain.Entities
{
    public class HorarioLaboral
    {
        public int IdHorarioLaboral { get; set; }

        public string Nombre { get; set; } = string.Empty;

        // Using TimeOnly to represent times of day
        public TimeOnly HoraEntrada { get; set; }

        public TimeOnly HoraSalida { get; set; }

        // e.g. "Mon,Tue,Wed,Thu,Fri" or other representation
        public string DiasLaborales { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
    }
}
