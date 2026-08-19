using AsistenciaPyme.Application.Features.Planillas.Commands;
using AsistenciaPyme.Application.Features.Planillas.DTOs;
using AsistenciaPyme.Application.Features.Empleados.Queries;
using AsistenciaPyme.Domain.Entities;
using AsistenciaPyme.Domain.Enums;
using AsistenciaPyme.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AsistenciaPyme.Tests;

public class PlanillaDepartamentoSeleccionTests
{
    private AsistenciaPymeDbContext CrearContextoEnMemoria(string nombreBd)
    {
        var options = new DbContextOptionsBuilder<AsistenciaPymeDbContext>()
            .UseInMemoryDatabase(databaseName: nombreBd)
            .Options;

        return new AsistenciaPymeDbContext(options);
    }

    [Fact]
    public async Task ObtenerEmpleados_FiltradoPorDepartamentoYSoloActivos_RetornaSoloCorrespondientes()
    {
        var context = CrearContextoEnMemoria(Guid.NewGuid().ToString());

        var depto1 = new Departamento { IdDepartamento = 1, Nombre = "Tecnología", Activo = true };
        var depto2 = new Departamento { IdDepartamento = 2, Nombre = "Ventas", Activo = true };
        var cargo = new Cargo { IdCargo = 1, Nombre = "Desarrollador", Activo = true };
        var horario = new HorarioLaboral { IdHorarioLaboral = 1, Nombre = "Diurno", Activo = true };

        context.Departamentos.AddRange(depto1, depto2);
        context.Cargos.Add(cargo);
        context.HorariosLaborales.Add(horario);

        var emp1 = new Empleado
        {
            IdEmpleado = 1,
            IdDepartamento = 1,
            IdCargo = 1,
            IdHorarioLaboral = 1,
            CodigoEmpleado = "EMP-001",
            Identificacion = "001-010190-0001A",
            Nombres = "Juan",
            Apellidos = "Pérez",
            SalarioBase = 25000,
            Estado = EstadoEmpleado.Activo,
            FechaContratacion = new DateOnly(2025, 1, 1)
        };

        var emp2 = new Empleado
        {
            IdEmpleado = 2,
            IdDepartamento = 1,
            IdCargo = 1,
            IdHorarioLaboral = 1,
            CodigoEmpleado = "EMP-002",
            Identificacion = "001-010190-0002B",
            Nombres = "María",
            Apellidos = "López",
            SalarioBase = 28000,
            Estado = EstadoEmpleado.Inactivo,
            FechaContratacion = new DateOnly(2025, 1, 1)
        };

        var emp3 = new Empleado
        {
            IdEmpleado = 3,
            IdDepartamento = 2,
            IdCargo = 1,
            IdHorarioLaboral = 1,
            CodigoEmpleado = "EMP-003",
            Identificacion = "001-010190-0003C",
            Nombres = "Carlos",
            Apellidos = "Gómez",
            SalarioBase = 30000,
            Estado = EstadoEmpleado.Activo,
            FechaContratacion = new DateOnly(2025, 1, 1)
        };

        context.Empleados.AddRange(emp1, emp2, emp3);
        await context.SaveChangesAsync();

        var handler = new ObtenerEmpleadosHandler(context);

        // Consulta departamento 1 solo activos
        var resultado = await handler.Handle(
            new ObtenerEmpleadosQuery { IdDepartamento = 1, SoloActivos = true },
            CancellationToken.None);

        Assert.Single(resultado);
        Assert.Equal("Juan", resultado[0].Nombres);
        Assert.Equal("EMP-001", resultado[0].CodigoEmpleado);
    }

    [Fact]
    public async Task GenerarPlanillaDepartamento_ConSeleccionEspecifica_ProcesaSoloEmpleadosSeleccionados()
    {
        var context = CrearContextoEnMemoria(Guid.NewGuid().ToString());

        var admin = new Administrador { IdAdministrador = 1, Nombres = "Admin", Apellidos = "General", Correo = "admin@test.com", Activo = true };
        var depto = new Departamento { IdDepartamento = 1, Nombre = "Operaciones", Activo = true };
        var cargo = new Cargo { IdCargo = 1, Nombre = "Operador", Activo = true };
        var horario = new HorarioLaboral { IdHorarioLaboral = 1, Nombre = "Diurno", Activo = true };
        var config = new ConfiguracionNomina { IdConfiguracionNomina = 1, MultiplicadorHoraExtra = 2.0m };

        context.Administradores.Add(admin);
        context.Departamentos.Add(depto);
        context.Cargos.Add(cargo);
        context.HorariosLaborales.Add(horario);
        context.ConfiguracionesNomina.Add(config);

        var emp1 = new Empleado { IdEmpleado = 10, IdDepartamento = 1, IdCargo = 1, IdHorarioLaboral = 1, CodigoEmpleado = "OP-01", Identificacion = "001-111111-0001A", Nombres = "Ana", Apellidos = "Sosa", SalarioBase = 20000, Estado = EstadoEmpleado.Activo, FechaContratacion = new DateOnly(2025, 1, 1) };
        var emp2 = new Empleado { IdEmpleado = 11, IdDepartamento = 1, IdCargo = 1, IdHorarioLaboral = 1, CodigoEmpleado = "OP-02", Identificacion = "001-111111-0002B", Nombres = "Luis", Apellidos = "Mora", SalarioBase = 22000, Estado = EstadoEmpleado.Activo, FechaContratacion = new DateOnly(2025, 1, 1) };
        var emp3 = new Empleado { IdEmpleado = 12, IdDepartamento = 1, IdCargo = 1, IdHorarioLaboral = 1, CodigoEmpleado = "OP-03", Identificacion = "001-111111-0003C", Nombres = "Sara", Apellidos = "Vega", SalarioBase = 24000, Estado = EstadoEmpleado.Activo, FechaContratacion = new DateOnly(2025, 1, 1) };

        context.Empleados.AddRange(emp1, emp2, emp3);
        await context.SaveChangesAsync();

        var calculadorHorasExtras = new CalculadorHorasExtras();
        var handler = new GenerarPlanillaDepartamentoHandler(context, calculadorHorasExtras);

        // Generar seleccionando únicamente emp1 y emp3 (excluyendo emp2)
        var command = new GenerarPlanillaDepartamentoCommand
        {
            IdDepartamento = 1,
            IdAdministrador = 1,
            FechaInicioPeriodo = new DateOnly(2026, 1, 1),
            FechaFinPeriodo = new DateOnly(2026, 1, 15),
            IdsEmpleadosSeleccionados = new List<int> { 10, 12 }
        };

        var resultado = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Detalles.Count);
        Assert.Contains(resultado.Detalles, d => d.IdEmpleado == 10);
        Assert.Contains(resultado.Detalles, d => d.IdEmpleado == 12);
        Assert.DoesNotContain(resultado.Detalles, d => d.IdEmpleado == 11);
    }

    [Fact]
    public async Task GenerarPlanillaDepartamento_EmpleadoDeOtroDepartamento_LanzaInvalidOperationException()
    {
        var context = CrearContextoEnMemoria(Guid.NewGuid().ToString());

        var admin = new Administrador { IdAdministrador = 1, Nombres = "Admin", Apellidos = "General", Correo = "admin@test.com", Activo = true };
        var depto1 = new Departamento { IdDepartamento = 1, Nombre = "TI", Activo = true };
        var depto2 = new Departamento { IdDepartamento = 2, Nombre = "RRHH", Activo = true };
        var cargo = new Cargo { IdCargo = 1, Nombre = "Especialista", Activo = true };
        var horario = new HorarioLaboral { IdHorarioLaboral = 1, Nombre = "Diurno", Activo = true };

        context.Administradores.Add(admin);
        context.Departamentos.AddRange(depto1, depto2);
        context.Cargos.Add(cargo);
        context.HorariosLaborales.Add(horario);

        var empTI = new Empleado { IdEmpleado = 1, IdDepartamento = 1, IdCargo = 1, IdHorarioLaboral = 1, CodigoEmpleado = "TI-01", Identificacion = "001-222222-0001A", Nombres = "Alex", Apellidos = "Rios", SalarioBase = 30000, Estado = EstadoEmpleado.Activo, FechaContratacion = new DateOnly(2025, 1, 1) };
        var empRRHH = new Empleado { IdEmpleado = 2, IdDepartamento = 2, IdCargo = 1, IdHorarioLaboral = 1, CodigoEmpleado = "RH-01", Identificacion = "001-222222-0002B", Nombres = "Elena", Apellidos = "Paz", SalarioBase = 28000, Estado = EstadoEmpleado.Activo, FechaContratacion = new DateOnly(2025, 1, 1) };

        context.Empleados.AddRange(empTI, empRRHH);
        await context.SaveChangesAsync();

        var calculadorHorasExtras = new CalculadorHorasExtras();
        var handler = new GenerarPlanillaDepartamentoHandler(context, calculadorHorasExtras);

        // Se intenta procesar departamento 1 pero incluyendo al empleado 2 de departamento 2
        var command = new GenerarPlanillaDepartamentoCommand
        {
            IdDepartamento = 1,
            IdAdministrador = 1,
            FechaInicioPeriodo = new DateOnly(2026, 1, 1),
            FechaFinPeriodo = new DateOnly(2026, 1, 15),
            IdsEmpleadosSeleccionados = new List<int> { 1, 2 }
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(command, CancellationToken.None));

        Assert.Contains("no pertenecen al departamento", exception.Message);
    }

    [Fact]
    public async Task GenerarPlanillaDepartamento_ListaVaciaSeleccionados_LanzaInvalidOperationException()
    {
        var context = CrearContextoEnMemoria(Guid.NewGuid().ToString());

        var admin = new Administrador { IdAdministrador = 1, Nombres = "Admin", Apellidos = "General", Correo = "admin@test.com", Activo = true };
        var depto1 = new Departamento { IdDepartamento = 1, Nombre = "TI", Activo = true };
        var cargo = new Cargo { IdCargo = 1, Nombre = "Especialista", Activo = true };
        var horario = new HorarioLaboral { IdHorarioLaboral = 1, Nombre = "Diurno", Activo = true };

        context.Administradores.Add(admin);
        context.Departamentos.Add(depto1);
        context.Cargos.Add(cargo);
        context.HorariosLaborales.Add(horario);

        var empTI = new Empleado { IdEmpleado = 1, IdDepartamento = 1, IdCargo = 1, IdHorarioLaboral = 1, CodigoEmpleado = "TI-01", Identificacion = "001-222222-0001A", Nombres = "Alex", Apellidos = "Rios", SalarioBase = 30000, Estado = EstadoEmpleado.Activo, FechaContratacion = new DateOnly(2025, 1, 1) };
        context.Empleados.Add(empTI);
        await context.SaveChangesAsync();

        var calculadorHorasExtras = new CalculadorHorasExtras();
        var handler = new GenerarPlanillaDepartamentoHandler(context, calculadorHorasExtras);

        var command = new GenerarPlanillaDepartamentoCommand
        {
            IdDepartamento = 1,
            IdAdministrador = 1,
            FechaInicioPeriodo = new DateOnly(2026, 1, 1),
            FechaFinPeriodo = new DateOnly(2026, 1, 15),
            IdsEmpleadosSeleccionados = new List<int>()
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(command, CancellationToken.None));

        Assert.Contains("Debe seleccionar al menos un colaborador", exception.Message);
    }
}
