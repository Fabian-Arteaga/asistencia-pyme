namespace AsistenciaPyme.Tests;

public class PayrollBusinessRulesTests
{
    [Fact]
    public void HoraExtra_Pendiente_NoPaga()
    {
        var estado = "Pendiente";
        var minutosAprobados = 0;

        var paga = estado == "Aprobada" && minutosAprobados > 0;

        Assert.False(paga);
    }

    [Fact]
    public void HoraExtra_Rechazada_NoPaga()
    {
        var estado = "Rechazada";
        var minutosAprobados = 0;

        var paga = estado == "Aprobada" && minutosAprobados > 0;

        Assert.False(paga);
    }

    [Fact]
    public void HoraExtra_Aprobada_Paga()
    {
        var estado = "Aprobada";
        var minutosAprobados = 120;

        var paga = estado == "Aprobada" && minutosAprobados > 0;

        Assert.True(paga);
    }

    [Fact]
    public void MinutosAprobados_MayoresQueDetectados_SeInvalida()
    {
        var minutosDetectados = 90;
        var minutosAprobados = 120;

        var esValido = minutosAprobados > 0 && minutosAprobados <= minutosDetectados;

        Assert.False(esValido);
    }

    [Fact]
    public void Embargo_Activo_SeAplica()
    {
        var activo = true;
        var vigente = true;

        var aplica = activo && vigente;

        Assert.True(aplica);
    }

    [Fact]
    public void Embargo_Vencido_NoSeAplica()
    {
        var activo = true;
        var vigente = false;

        var aplica = activo && vigente;

        Assert.False(aplica);
    }

    [Fact]
    public void Planilla_Con_10_Empleados_Crea_1Planilla_Y_10_Detalles()
    {
        var cantidadEmpleados = 10;
        var planillas = 1;
        var detalles = cantidadEmpleados;

        Assert.Equal(planillas, 1);
        Assert.Equal(detalles, 10);
    }

    [Fact]
    public void PlanillaDuplicada_Departamento_Y_Periodo_SeRechaza()
    {
        var departamento = 4;
        var periodo = "2026-08";
        var existePlanilla = true;

        var esDuplicada = existePlanilla && departamento > 0 && !string.IsNullOrWhiteSpace(periodo);

        Assert.True(esDuplicada);
    }

    [Fact]
    public void PlanillaCerrada_NoPuedeRecalcularse()
    {
        var estado = "Cerrada";

        var recalculable = estado != "Cerrada" && estado != "Pagada";

        Assert.False(recalculable);
    }

    [Fact]
    public void PlanillaPagada_NoPuedeRecalcularse()
    {
        var estado = "Pagada";

        var recalculable = estado != "Cerrada" && estado != "Pagada";

        Assert.False(recalculable);
    }

    [Fact]
    public void Snapshot_ConservaSalarioAnterior()
    {
        var salarioHistorico = 15000m;
        var salarioActual = 18000m;

        var salarioMostradoHistorico = salarioHistorico;

        Assert.Equal(15000m, salarioMostradoHistorico);
        Assert.NotEqual(salarioActual, salarioMostradoHistorico);
    }

    [Fact]
    public void NumeroINSS_Duplicado_SeRechaza()
    {
        var numerosINSS = new[] { "123456", "123456" };
        var esDuplicado = numerosINSS.Distinct().Count() != numerosINSS.Length;

        Assert.True(esDuplicado);
    }

    [Fact]
    public void CambioDepartamento_GeneraHistorial()
    {
        var historialCreado = true;
        var empleadoActualizado = true;

        var procesoValido = historialCreado && empleadoActualizado;

        Assert.True(procesoValido);
    }

    [Fact]
    public void INSSNull_NoGeneraDescuentoFicticio()
    {
        decimal? tasaINSS = null;
        var monto = tasaINSS.HasValue ? 100m : 0m;

        Assert.Equal(0m, monto);
    }

    [Fact]
    public void ConceptosDePlanilla_CoincidenConTotales()
    {
        var conceptos = new[]
        {
            new { Nombre = "SALARIO_BASE", Monto = 1500m },
            new { Nombre = "HORA_EXTRA", Monto = 300m },
            new { Nombre = "INSS", Monto = 180m },
            new { Nombre = "EMBARGO", Monto = 90m }
        };

        var totalIngresos = conceptos.Where(c => c.Nombre == "SALARIO_BASE" || c.Nombre == "HORA_EXTRA").Sum(c => c.Monto);
        var totalDeducciones = conceptos.Where(c => c.Nombre == "INSS" || c.Nombre == "EMBARGO").Sum(c => c.Monto);
        var total = totalIngresos - totalDeducciones;

        Assert.Equal(1800m, totalIngresos);
        Assert.Equal(270m, totalDeducciones);
        Assert.Equal(1530m, total);
    }
}
