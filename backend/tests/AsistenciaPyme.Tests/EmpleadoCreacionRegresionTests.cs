using AsistenciaPyme.Application.Features.Empleados.Commands;
using AsistenciaPyme.Domain.Entities;
using AsistenciaPyme.Domain.Enums;
using AsistenciaPyme.Infrastructure.Persistence;
using AsistenciaPyme.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AsistenciaPyme.Tests;

public class EmpleadoCreacionRegresionTests
{
    private AsistenciaPymeDbContext CrearDbContextEnMemoria()
    {
        var options = new DbContextOptionsBuilder<AsistenciaPymeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AsistenciaPymeDbContext(options);
    }

    [Fact]
    public async Task CrearEmpleado_ConDepartamentoValido_PersisteEmpleadoYHistorialConFKCorrecta()
    {
        // Arrange
        using var context = CrearDbContextEnMemoria();
        var pinHasher = new PinHasher();

        var cargo = new Cargo
        {
            Nombre = "Desarrollador Senior",
            Funciones = "Desarrollo de software",
            Responsabilidades = "Calidad de código",
            Activo = true
        };
        await context.Cargos.AddAsync(cargo);

        var departamento = new Departamento
        {
            Nombre = "Tecnología",
            Descripcion = "Área de TI",
            Activo = true
        };
        await context.Departamentos.AddAsync(departamento);
        await context.SaveChangesAsync();

        var handler = new CrearEmpleadoHandler(context, pinHasher);
        var command = new CrearEmpleadoCommand
        {
            IdCargo = cargo.IdCargo,
            IdDepartamento = departamento.IdDepartamento,
            CodigoEmpleado = "EMP-001",
            Pin = "1234",
            Identificacion = "001-010190-0001A",
            NumeroINSS = "INSS-1001",
            Nombres = "Carlos",
            Apellidos = "Pérez",
            FechaContratacion = new DateOnly(2026, 1, 15),
            SalarioBase = 35000m
        };

        // Act
        int idEmpleadoCreado = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(idEmpleadoCreado > 0);

        var empleadoEnDb = await context.Empleados
            .Include(e => e.DepartamentoHistorial)
            .FirstOrDefaultAsync(e => e.IdEmpleado == idEmpleadoCreado);

        Assert.NotNull(empleadoEnDb);
        Assert.Equal("Carlos", empleadoEnDb.Nombres);
        Assert.Equal(departamento.IdDepartamento, empleadoEnDb.IdDepartamento);

        // Verificar que el historial departamental se persistió con la FK id_empleado correcta
        Assert.Single(empleadoEnDb.DepartamentoHistorial);
        var historial = empleadoEnDb.DepartamentoHistorial.First();
        Assert.Equal(empleadoEnDb.IdEmpleado, historial.IdEmpleado);
        Assert.Equal(departamento.IdDepartamento, historial.IdDepartamento);
        Assert.Null(historial.FechaFin);
        Assert.True(historial.FechaInicio <= DateTime.UtcNow);
    }

    [Fact]
    public async Task CrearEmpleado_SinDepartamento_PersisteEmpleadoSinHistorial()
    {
        // Arrange
        using var context = CrearDbContextEnMemoria();
        var pinHasher = new PinHasher();

        var cargo = new Cargo
        {
            Nombre = "Consultor Externo",
            Funciones = "Consultoría",
            Responsabilidades = "Asesoría",
            Activo = true
        };
        await context.Cargos.AddAsync(cargo);
        await context.SaveChangesAsync();

        var handler = new CrearEmpleadoHandler(context, pinHasher);
        var command = new CrearEmpleadoCommand
        {
            IdCargo = cargo.IdCargo,
            IdDepartamento = null,
            CodigoEmpleado = "EMP-002",
            Pin = "5678",
            Identificacion = "001-020292-0002B",
            Nombres = "Ana",
            Apellidos = "Gómez",
            FechaContratacion = new DateOnly(2026, 2, 1),
            SalarioBase = 40000m
        };

        // Act
        int idEmpleadoCreado = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(idEmpleadoCreado > 0);
        var empleadoEnDb = await context.Empleados
            .Include(e => e.DepartamentoHistorial)
            .FirstOrDefaultAsync(e => e.IdEmpleado == idEmpleadoCreado);

        Assert.NotNull(empleadoEnDb);
        Assert.Null(empleadoEnDb.IdDepartamento);
        Assert.Empty(empleadoEnDb.DepartamentoHistorial);
    }

    [Fact]
    public async Task CrearEmpleado_ConJefeDirectoValido_PersisteJerarquiaCorrectamente()
    {
        // Arrange
        using var context = CrearDbContextEnMemoria();
        var pinHasher = new PinHasher();

        var cargoJefe = new Cargo { Nombre = "Gerente TI", Funciones = "Gestión", Responsabilidades = "Liderazgo", Activo = true };
        var cargoDev = new Cargo { Nombre = "Desarrollador", Funciones = "Desarrollo", Responsabilidades = "Código", Activo = true };
        await context.Cargos.AddRangeAsync(cargoJefe, cargoDev);
        await context.SaveChangesAsync();

        var jefe = new Empleado
        {
            IdCargo = cargoJefe.IdCargo,
            CodigoEmpleado = "EMP-BOSS",
            PinHash = pinHasher.CrearHash("1111"),
            Identificacion = "001-111180-0001A",
            Nombres = "Roberto",
            Apellidos = "Martínez",
            FechaContratacion = new DateOnly(2025, 1, 1),
            SalarioBase = 60000m,
            Estado = EstadoEmpleado.Activo
        };
        await context.Empleados.AddAsync(jefe);
        await context.SaveChangesAsync();

        var handler = new CrearEmpleadoHandler(context, pinHasher);
        var command = new CrearEmpleadoCommand
        {
            IdCargo = cargoDev.IdCargo,
            IdJefeDirecto = jefe.IdEmpleado,
            CodigoEmpleado = "EMP-003",
            Pin = "2222",
            Identificacion = "001-222295-0003C",
            Nombres = "Lucía",
            Apellidos = "Fernández",
            FechaContratacion = new DateOnly(2026, 3, 1),
            SalarioBase = 32000m
        };

        // Act
        int idEmpleadoCreado = await handler.Handle(command, CancellationToken.None);

        // Assert
        var subordinado = await context.Empleados
            .Include(e => e.JefeDirecto)
            .FirstOrDefaultAsync(e => e.IdEmpleado == idEmpleadoCreado);

        Assert.NotNull(subordinado);
        Assert.Equal(jefe.IdEmpleado, subordinado.IdJefeDirecto);
        Assert.Equal("Roberto", subordinado.JefeDirecto?.Nombres);
    }

    [Fact]
    public async Task CrearEmpleado_ConJefeInexistente_LanzaExcepcion()
    {
        // Arrange
        using var context = CrearDbContextEnMemoria();
        var pinHasher = new PinHasher();

        var cargo = new Cargo { Nombre = "Analista", Funciones = "Análisis", Responsabilidades = "Docs", Activo = true };
        await context.Cargos.AddAsync(cargo);
        await context.SaveChangesAsync();

        var handler = new CrearEmpleadoHandler(context, pinHasher);
        var command = new CrearEmpleadoCommand
        {
            IdCargo = cargo.IdCargo,
            IdJefeDirecto = 99999, // Inexistente
            CodigoEmpleado = "EMP-004",
            Pin = "3333",
            Identificacion = "001-333390-0004D",
            Nombres = "Mario",
            Apellidos = "López",
            FechaContratacion = new DateOnly(2026, 4, 1),
            SalarioBase = 28000m
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task ActualizarEmpleado_AutoAsignadoComoJefe_LanzaExcepcion()
    {
        // Arrange
        using var context = CrearDbContextEnMemoria();
        var pinHasher = new PinHasher();

        var cargo = new Cargo { Nombre = "Supervisor", Funciones = "Supervisión", Responsabilidades = "Equipo", Activo = true };
        await context.Cargos.AddAsync(cargo);
        await context.SaveChangesAsync();

        var empleado = new Empleado
        {
            IdCargo = cargo.IdCargo,
            CodigoEmpleado = "EMP-005",
            PinHash = pinHasher.CrearHash("4444"),
            Identificacion = "001-444485-0005E",
            Nombres = "Esteban",
            Apellidos = "Rojas",
            FechaContratacion = new DateOnly(2026, 1, 1),
            SalarioBase = 45000m,
            Estado = EstadoEmpleado.Activo
        };
        await context.Empleados.AddAsync(empleado);
        await context.SaveChangesAsync();

        var updateHandler = new ActualizarEmpleadoHandler(context);
        var updateCommand = new ActualizarEmpleadoCommand
        {
            IdEmpleado = empleado.IdEmpleado,
            IdCargo = cargo.IdCargo,
            IdJefeDirecto = empleado.IdEmpleado, // Auto-asignado
            CodigoEmpleado = "EMP-005",
            Identificacion = "001-444485-0005E",
            Nombres = "Esteban",
            Apellidos = "Rojas",
            FechaContratacion = new DateOnly(2026, 1, 1),
            SalarioBase = 45000m
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => updateHandler.Handle(updateCommand, CancellationToken.None));
    }

    [Fact]
    public async Task ActualizarEmpleado_CambioDeDepartamento_CierraHistorialAnteriorYAbreNuevo()
    {
        // Arrange
        using var context = CrearDbContextEnMemoria();
        var pinHasher = new PinHasher();

        var cargo = new Cargo { Nombre = "Especialista", Funciones = "Operaciones", Responsabilidades = "Calidad", Activo = true };
        var deptoA = new Departamento { Nombre = "Ventas", Activo = true };
        var deptoB = new Departamento { Nombre = "Marketing", Activo = true };
        await context.Cargos.AddAsync(cargo);
        await context.Departamentos.AddRangeAsync(deptoA, deptoB);
        await context.SaveChangesAsync();

        // 1. Crear empleado en deptoA
        var createHandler = new CrearEmpleadoHandler(context, pinHasher);
        int idEmpleado = await createHandler.Handle(new CrearEmpleadoCommand
        {
            IdCargo = cargo.IdCargo,
            IdDepartamento = deptoA.IdDepartamento,
            CodigoEmpleado = "EMP-006",
            Pin = "7777",
            Identificacion = "001-777790-0006F",
            Nombres = "Gabriela",
            Apellidos = "Mendoza",
            FechaContratacion = new DateOnly(2026, 1, 1),
            SalarioBase = 38000m
        }, CancellationToken.None);

        // 2. Cambiar a deptoB
        var updateHandler = new ActualizarEmpleadoHandler(context);
        var dto = await updateHandler.Handle(new ActualizarEmpleadoCommand
        {
            IdEmpleado = idEmpleado,
            IdCargo = cargo.IdCargo,
            IdDepartamento = deptoB.IdDepartamento,
            CodigoEmpleado = "EMP-006",
            Identificacion = "001-777790-0006F",
            Nombres = "Gabriela",
            Apellidos = "Mendoza",
            FechaContratacion = new DateOnly(2026, 1, 1),
            SalarioBase = 40000m
        }, CancellationToken.None);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(deptoB.IdDepartamento, dto.IdDepartamento);

        var historiales = await context.EmpleadoDepartamentoHistorials
            .Where(h => h.IdEmpleado == idEmpleado)
            .OrderBy(h => h.FechaInicio)
            .ToListAsync();

        Assert.Equal(2, historiales.Count);

        // Historial 1 (Ventas) cerrado con FechaFin
        Assert.Equal(deptoA.IdDepartamento, historiales[0].IdDepartamento);
        Assert.NotNull(historiales[0].FechaFin);

        // Historial 2 (Marketing) abierto sin FechaFin
        Assert.Equal(deptoB.IdDepartamento, historiales[1].IdDepartamento);
        Assert.Null(historiales[1].FechaFin);
    }
}
