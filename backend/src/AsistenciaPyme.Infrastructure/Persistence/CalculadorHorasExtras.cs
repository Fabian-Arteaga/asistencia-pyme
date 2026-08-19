using AsistenciaPyme.Application.Common.Interfaces;
using System;

namespace AsistenciaPyme.Infrastructure.Persistence
{
    public class CalculadorHorasExtras : ICalculadorHorasExtras
    {
        private const int HorasPorDia = 8;
        private const int DiasBase = 30;

        public decimal CalcularMontoHorasExtras(decimal salarioBase, int minutosAprobados, decimal multiplicadorHoraExtra)
        {
            if (minutosAprobados <= 0 || salarioBase <= 0 || multiplicadorHoraExtra <= 0)
            {
                return 0m;
            }

            decimal horasAprobadas = minutosAprobados / 60m;
            decimal valorHoraOrdinaria = salarioBase / (DiasBase * HorasPorDia);
            decimal monto = horasAprobadas * valorHoraOrdinaria * multiplicadorHoraExtra;
            return Math.Round(monto, 2, MidpointRounding.AwayFromZero);
        }
    }
}
