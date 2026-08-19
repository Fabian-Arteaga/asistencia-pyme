using System;

namespace AsistenciaPyme.Application.Common.Interfaces
{
    public interface ICalculadorHorasExtras
    {
        decimal CalcularMontoHorasExtras(decimal salarioBase, int minutosAprobados, decimal multiplicadorHoraExtra);
    }
}
