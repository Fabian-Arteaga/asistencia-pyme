using System;
using System.Collections.Generic;
using System.Text;

namespace AsistenciaPyme.Application.Feautures.Asistencias.DTOs
{
    public class ResultadoMarcacionDto
    {
        public int IdAsistencia { get; set; }

        public int IdEmpleado { get; set; }

        public string CodigoEmpleado { get; set; } = string.Empty;

        public string NombreEmpleado { get; set; } = string.Empty;

        public string TipoMarcacion { get; set; } = string.Empty;

        public DateTime FechaHoraMarcacion { get; set; }

        public DateTime HoraEntrada { get; set; }

        public DateTime? HoraSalida { get; set; }

        public DateTime? HoraProgramadaEntrada { get; set; }

        public DateTime? HoraProgramadaSalida { get; set; }

        public bool EsEntradaTardia { get; set; }

        public int MinutosTardanza { get; set; }

        public string Mensaje { get; set; } = string.Empty;
    }
}
