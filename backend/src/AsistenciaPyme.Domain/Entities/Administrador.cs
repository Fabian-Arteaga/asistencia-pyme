using System;
using System.Collections.Generic;
using System.Text;

namespace AsistenciaPyme.Domain.Entities
{
    public class Administrador
    {
        public int IdAdministrador { get; set; }

        public string Nombres { get; set; } = string.Empty;

        public string Apellidos { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string ContrasenaHash { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaActualizacion { get; set; }

        public ICollection<Asistencia> AsistenciasCorregidas { get; set; }
            = new List<Asistencia>();

        public ICollection<Vacacion> VacacionesRegistradas { get; set; }
            = new List<Vacacion>();

        public ICollection<Planilla> PlanillasGeneradas { get; set; }
            = new List<Planilla>();
    }
}
