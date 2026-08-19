using System;

namespace AsistenciaPyme.Domain.Entities
{
    public enum PoliticaDescuentoTardanza
    {
        SinDeduccion = 1,
        PorMinuto = 2,
        Manual = 3
    }

    public class ConfiguracionNomina
    {
        public int IdConfiguracionNomina { get; set; }

        public int MinutosToleranciaEntrada { get; set; } = 10;

        public decimal MultiplicadorHoraExtra { get; set; } = 2.0m;

        public decimal DiasVacacionesPorMes { get; set; } = 2.5m;

        public int DiasBaseProrrateoVacaciones { get; set; } = 30;

        public PoliticaDescuentoTardanza PoliticaDescuentoTardanza { get; set; } = PoliticaDescuentoTardanza.SinDeduccion;

        public int MinutosMaximosVerificacionDepartamento { get; set; } = 10;

        public decimal? TasaINSS { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaActualizacion { get; set; }
    }
}
