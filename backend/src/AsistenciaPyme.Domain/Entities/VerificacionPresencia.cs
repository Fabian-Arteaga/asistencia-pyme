using System;

namespace AsistenciaPyme.Domain.Entities
{
    public enum EstadoVerificacionPresencia
    {
        Confirmada = 1,
        FueraDeTiempo = 2,
        NoConfirmada = 3
    }

    public class VerificacionPresencia
    {
        public int IdVerificacionPresencia { get; set; }

        public int IdEmpleado { get; set; }

        public int? IdAsistencia { get; set; }

        public int? IdDepartamento { get; set; }

        public int? IdDispositivoMarcaje { get; set; }

        public DateTime FechaHora { get; set; }

        public EstadoVerificacionPresencia Estado { get; set; }

        public Empleado Empleado { get; set; } = null!;

        public Asistencia? Asistencia { get; set; }

        public Departamento? Departamento { get; set; }

        public DispositivoMarcaje? Dispositivo { get; set; }
    }
}
