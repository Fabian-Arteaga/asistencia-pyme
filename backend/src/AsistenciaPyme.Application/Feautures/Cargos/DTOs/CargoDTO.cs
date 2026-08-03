using System;
using System.Collections.Generic;
using System.Text;

namespace AsistenciaPyme.Application.Feautures.Cargos.DTOs
{
    public class CargoDTO
    { 
        public int IdCargo { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public string Funciones { get; set; } = string.Empty;

        public string Responsabilidades { get; set; } = string.Empty;

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }
    }
}

