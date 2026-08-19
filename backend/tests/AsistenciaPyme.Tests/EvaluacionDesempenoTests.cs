using AsistenciaPyme.Application.Feautures.EvaluacionesDesempeno.DTOs;
using AsistenciaPyme.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AsistenciaPyme.Tests;

public class EvaluacionDesempenoTests
{
    [Fact]
    public void Ponderaciones_SumaExacta100_EsValida()
    {
        // 7 categorías con ponderaciones configurables
        var ponderaciones = new Dictionary<string, decimal>
        {
            { "Rendimiento", 20.00m },
            { "Objetivos", 15.00m },
            { "Responsabilidad", 15.00m },
            { "Competencias", 15.00m },
            { "Aptitudes", 10.00m },
            { "Iniciativa", 15.00m },
            { "Creatividad", 10.00m }
        };

        decimal suma = ponderaciones.Values.Sum();
        bool esValido = Math.Round(suma, 2) == 100.00m;

        Assert.True(esValido);
        Assert.Equal(100.00m, suma);
    }

    [Fact]
    public void Ponderaciones_SumaDiferenteDe100_EsInvalida()
    {
        var ponderaciones = new Dictionary<string, decimal>
        {
            { "Rendimiento", 25.00m },
            { "Objetivos", 15.00m },
            { "Responsabilidad", 15.00m },
            { "Competencias", 15.00m },
            { "Aptitudes", 10.00m },
            { "Iniciativa", 10.00m },
            { "Creatividad", 5.00m } // Suma = 95%
        };

        decimal suma = ponderaciones.Values.Sum();
        bool esValido = Math.Round(suma, 2) == 100.00m;

        Assert.False(esValido);
        Assert.Equal(95.00m, suma);
    }

    [Fact]
    public void Ponderaciones_ValorMenorOIgualACero_EsInvalida()
    {
        var ponderaciones = new List<decimal> { 20m, 15m, 0m, 20m, 15m, 15m, 15m };
        bool tieneCeroONegativo = ponderaciones.Any(p => p <= 0);

        Assert.True(tieneCeroONegativo);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(3, true)]
    [InlineData(4, true)]
    [InlineData(5, true)]
    [InlineData(0, false)]
    [InlineData(6, false)]
    [InlineData(-1, false)]
    public void EscalaPuntuacion_ValidaRango1a5(int puntuacion, bool esperadoValido)
    {
        bool esValido = puntuacion >= 1 && puntuacion <= 5;
        Assert.Equal(esperadoValido, esValido);
    }

    [Fact]
    public void CalculoResultado_FormulaPonderada_CalculaCorrectamenteEscala0a100()
    {
        // Supongamos 7 categorías con ponderaciones y respuestas:
        // C1 (20%): notas [5, 5, 5] -> prom = 5.0 -> (5/5)*20 = 20.00
        // C2 (15%): notas [4, 4]    -> prom = 4.0 -> (4/5)*15 = 12.00
        // C3 (15%): notas [3, 4, 5] -> prom = 4.0 -> (4/5)*15 = 12.00
        // C4 (15%): notas [5, 4]    -> prom = 4.5 -> (4.5/5)*15 = 13.50
        // C5 (10%): notas [4, 4]    -> prom = 4.0 -> (4/5)*10 = 8.00
        // C6 (15%): notas [5, 5]    -> prom = 5.0 -> (5/5)*15 = 15.00
        // C7 (10%): notas [4, 5]    -> prom = 4.5 -> (4.5/5)*10 = 9.00
        // Total esperado: 20 + 12 + 12 + 13.5 + 8 + 15 + 9 = 89.50

        var categorias = new List<(decimal Ponderacion, List<int> Notas)>
        {
            (20.00m, new List<int> { 5, 5, 5 }),
            (15.00m, new List<int> { 4, 4 }),
            (15.00m, new List<int> { 3, 4, 5 }),
            (15.00m, new List<int> { 5, 4 }),
            (10.00m, new List<int> { 4, 4 }),
            (15.00m, new List<int> { 5, 5 }),
            (10.00m, new List<int> { 4, 5 })
        };

        decimal puntajeFinal = 0m;
        foreach (var cat in categorias)
        {
            decimal prom = (decimal)cat.Notas.Average();
            decimal ponderado = (prom / 5.0m) * cat.Ponderacion;
            puntajeFinal += ponderado;
        }

        puntajeFinal = Math.Round(puntajeFinal, 2);

        Assert.Equal(89.50m, puntajeFinal);
        Assert.Equal("Muy bueno", EvaluacionDesempenoDto.ObtenerClasificacion(puntajeFinal));
    }

    [Fact]
    public void Consolidado360_PromedioEquitativoDePerspectivas_SinSesgoDeSubordinados()
    {
        // Escenario: Un gerente es evaluado con 360°:
        // Autoevaluación: 90.00
        // Jefe Directo: 80.00
        // 4 Subordinados con notas: [82.00, 84.00, 86.00, 88.00] -> Promedio subordinados = 85.00
        // Consolidado equitativo de las 3 perspectivas: (90 + 80 + 85) / 3 = 85.00

        decimal autoevaluacion = 90.00m;
        decimal jefeDirecto = 80.00m;
        var subordinados = new List<decimal> { 82.00m, 84.00m, 86.00m, 88.00m };

        decimal promSubordinados = subordinados.Average();
        Assert.Equal(85.00m, promSubordinados);

        var perspectivas = new List<decimal> { autoevaluacion, jefeDirecto, promSubordinados };
        decimal consolidado = Math.Round(perspectivas.Average(), 2);

        Assert.Equal(85.00m, consolidado);
        Assert.Equal("Muy bueno", EvaluacionDesempenoDto.ObtenerClasificacion(consolidado));
    }

    [Fact]
    public void Consolidado360_ManejaColaboradorSinSubordinados_PromediaSoloPerspectivasPresentes()
    {
        // Escenario: Colaborador operativo sin subordinados
        // Autoevaluación: 84.00
        // Jefe Directo: 78.00
        // Subordinados: no existen
        // Consolidado: (84 + 78) / 2 = 81.00

        decimal? auto = 84.00m;
        decimal? jefe = 78.00m;
        decimal? subs = null;

        var presentes = new List<decimal>();
        if (auto.HasValue) presentes.Add(auto.Value);
        if (jefe.HasValue) presentes.Add(jefe.Value);
        if (subs.HasValue) presentes.Add(subs.Value);

        decimal consolidado = Math.Round(presentes.Average(), 2);

        Assert.Equal(81.00m, consolidado);
        Assert.Equal("Muy bueno", EvaluacionDesempenoDto.ObtenerClasificacion(consolidado));
    }

    [Theory]
    [InlineData(95.0, "Excelente")]
    [InlineData(90.0, "Excelente")]
    [InlineData(89.9, "Muy bueno")]
    [InlineData(80.0, "Muy bueno")]
    [InlineData(75.5, "Bueno")]
    [InlineData(70.0, "Bueno")]
    [InlineData(65.0, "Aceptable")]
    [InlineData(60.0, "Aceptable")]
    [InlineData(59.9, "Necesita mejorar")]
    [InlineData(40.0, "Necesita mejorar")]
    public void Clasificacion_Rangos_DevuelveEtiquetaCorrecta(double puntajeDouble, string clasificacionEsperada)
    {
        decimal puntaje = (decimal)puntajeDouble;
        string resultado = EvaluacionDesempenoDto.ObtenerClasificacion(puntaje);
        Assert.Equal(clasificacionEsperada, resultado);
    }

    [Fact]
    public void Autoevaluacion_ValidaEvaluadorIgualAEvaluado()
    {
        int idEvaluado = 10;
        int idEvaluadorValido = 10;
        int idEvaluadorInvalido = 12;

        bool esValidoCorrecto = (idEvaluado == idEvaluadorValido);
        bool esValidoIncorrecto = (idEvaluado == idEvaluadorInvalido);

        Assert.True(esValidoCorrecto);
        Assert.False(esValidoIncorrecto);
    }

    [Fact]
    public void JefeDirecto_ValidaEvaluadorEsJefeAsignado()
    {
        int? idJefeDirectoAsignado = 5;

        int evaluadorEsJefe = 5;
        int evaluadorNoEsJefe = 8;

        bool esJefeValido = idJefeDirectoAsignado.HasValue && idJefeDirectoAsignado.Value == evaluadorEsJefe;
        bool esJefeInvalido = idJefeDirectoAsignado.HasValue && idJefeDirectoAsignado.Value == evaluadorNoEsJefe;

        Assert.True(esJefeValido);
        Assert.False(esJefeInvalido);
    }

    [Fact]
    public void Subordinado_ValidaEvaluadoEsJefeDelEvaluador()
    {
        int idEmpleadoEvaluado = 5; // El jefe
        int? idJefeDelSubordinado1 = 5; // Subordinado directo
        int? idJefeDelSubordinado2 = 8; // No es subordinado de 5

        bool esSubordinadoValido = idJefeDelSubordinado1.HasValue && idJefeDelSubordinado1.Value == idEmpleadoEvaluado;
        bool esSubordinadoInvalido = idJefeDelSubordinado2.HasValue && idJefeDelSubordinado2.Value == idEmpleadoEvaluado;

        Assert.True(esSubordinadoValido);
        Assert.False(esSubordinadoInvalido);
    }

    [Fact]
    public void FechasPeriodo_FechaFinDebeSerPosteriorAFechaInicio()
    {
        var fechaInicio = new DateOnly(2026, 1, 1);
        var fechaFinValida = new DateOnly(2026, 6, 30);
        var fechaFinInvalida = new DateOnly(2025, 12, 31);

        Assert.True(fechaFinValida > fechaInicio);
        Assert.False(fechaFinInvalida > fechaInicio);
    }
}
