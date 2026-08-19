namespace AsistenciaPyme.Application.Feautures.Asistencias;

public static class AsistenciaReglas
{
    public static int CalcularMinutosTardanza(DateTime horaEntrada, DateTime horaProgramadaEntrada, int minutosTolerancia)
    {
        if (horaEntrada <= horaProgramadaEntrada)
        {
            return 0;
        }

        var diferencia = (int)Math.Max(0, (horaEntrada - horaProgramadaEntrada).TotalMinutes - minutosTolerancia);
        return diferencia;
    }

    public static int CalcularMinutosExtra(DateTime horaSalida, DateTime horaProgramadaSalida)
    {
        if (horaSalida <= horaProgramadaSalida)
        {
            return 0;
        }

        return (int)Math.Max(0, (horaSalida - horaProgramadaSalida).TotalMinutes);
    }
}
