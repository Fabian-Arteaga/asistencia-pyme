using System;

namespace AsistenciaPyme.Domain.Entities
{
    public enum TipoDispositivoMarcaje
    {
        KioscoPrincipal = 1,
        TerminalDepartamento = 2
    }

    public class DispositivoMarcaje
    {
        public int IdDispositivoMarcaje { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public TipoDispositivoMarcaje Tipo { get; set; }

        public int? IdDepartamento { get; set; }

        public string Identificador { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public Departamento? Departamento { get; set; }
    }
}
