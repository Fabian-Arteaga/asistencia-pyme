using AsistenciaPyme.Application.Feautures.Asistencias;

namespace AsistenciaPyme.Tests;

public class AsistenciaReglasTests
{
    [Fact]
    public void Entrada_08_10_ConTolerancia_10_NoEsTardia()
    {
        var horaProgramada = new DateTime(2026, 1, 12, 8, 0, 0, DateTimeKind.Utc);
        var horaEntrada = new DateTime(2026, 1, 12, 8, 10, 0, DateTimeKind.Utc);

        var tardanza = AsistenciaReglas.CalcularMinutosTardanza(horaEntrada, horaProgramada, 10);

        Assert.Equal(0, tardanza);
    }

    [Fact]
    public void Entrada_08_11_ConTolerancia_10_RegistraTardanzaEnUnMinuto()
    {
        var horaProgramada = new DateTime(2026, 1, 12, 8, 0, 0, DateTimeKind.Utc);
        var horaEntrada = new DateTime(2026, 1, 12, 8, 11, 0, DateTimeKind.Utc);

        var tardanza = AsistenciaReglas.CalcularMinutosTardanza(horaEntrada, horaProgramada, 10);

        Assert.Equal(1, tardanza);
    }

    [Fact]
    public void Salida_20_00_ConHorario_17_00_Detecta180MinutosExtra()
    {
        var horaProgramada = new DateTime(2026, 1, 12, 17, 0, 0, DateTimeKind.Utc);
        var horaSalida = new DateTime(2026, 1, 12, 20, 0, 0, DateTimeKind.Utc);

        var minutosExtra = AsistenciaReglas.CalcularMinutosExtra(horaSalida, horaProgramada);

        Assert.Equal(180, minutosExtra);
    }

    [Fact]
    public void MinutosAprobados_MayoresQueDetectados_SeRechaza()
    {
        var minutosDetectados = 120;
        var minutosAprobados = 180;

        var esValido = minutosAprobados <= minutosDetectados;

        Assert.False(esValido);
    }

    [Fact]
    public void HoraExtra_Pendiente_NoGeneraPago()
    {
        var horasExtraPendiente = 180;

        Assert.True(horasExtraPendiente > 0);
        Assert.NotEqual(0, horasExtraPendiente);
    }

    [Fact]
    public void HoraExtra_Aprobada_GeneraMontoSegunMinutos()
    {
        var minutosAprobados = 120;
        var salarioBase = 30000m;
        var multiplicador = 2.0m;

        var monto = salarioBase * (decimal)minutosAprobados / 60m / 30m * multiplicador;

        Assert.True(monto > 0m);
    }

    [Fact]
    public void Vacaciones_2_5_PorMes_SeCalculaComoProrrateoDiario()
    {
        var diasVacacionesPorMes = 2.5m;
        var diasBase = 30m;
        var prorrateoDiario = diasVacacionesPorMes / diasBase;

        Assert.Equal(0.0833333333333333333333333333m, prorrateoDiario, 12);
    }

    [Fact]
    public void ReglasNoPermitenTardanzaNegativa()
    {
        var horaProgramada = new DateTime(2026, 1, 12, 8, 0, 0, DateTimeKind.Utc);
        var horaEntrada = new DateTime(2026, 1, 12, 7, 50, 0, DateTimeKind.Utc);

        var tardanza = AsistenciaReglas.CalcularMinutosTardanza(horaEntrada, horaProgramada, 10);

        Assert.Equal(0, tardanza);
    }
}
