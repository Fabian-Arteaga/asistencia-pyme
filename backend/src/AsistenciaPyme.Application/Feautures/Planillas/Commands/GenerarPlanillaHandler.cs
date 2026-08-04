using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Planillas.DTOs;
using AsistenciaPyme.Domain.Entities;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Planillas.Commands;

public class GenerarPlanillaHandler
    : IRequestHandler<GenerarPlanillaCommand, PlanillaDto?>
{
    private readonly IAsistenciaPymeDbContext _context;

    public GenerarPlanillaHandler(
        IAsistenciaPymeDbContext context)
    {
        _context = context;
    }

    public async Task<PlanillaDto?> Handle(
        GenerarPlanillaCommand request,
        CancellationToken cancellationToken)
    {
        string codigoEmpleado =
            request.CodigoEmpleado.Trim();

        Empleado? empleado = await _context.Empleados
            .AsNoTracking()
            .FirstOrDefaultAsync(
                e =>
                    e.CodigoEmpleado.ToLower() ==
                    codigoEmpleado.ToLower(),
                cancellationToken);

        if (empleado is null)
        {
            return null;
        }

        if (empleado.Estado != EstadoEmpleado.Activo)
        {
            throw new InvalidOperationException(
                "El empleado está inactivo.");
        }

        if (empleado.SalarioBase <= 0)
        {
            throw new InvalidOperationException(
                "El empleado no tiene un salario base válido.");
        }

        Administrador? administrador =
            await _context.Administradores
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    a =>
                        a.IdAdministrador ==
                        request.IdAdministrador &&
                        a.Activo,
                    cancellationToken);

        if (administrador is null)
        {
            throw new InvalidOperationException(
                "El administrador no existe o está inactivo.");
        }

        if (request.FechaFinPeriodo <
            request.FechaInicioPeriodo)
        {
            throw new InvalidOperationException(
                "La fecha final no puede ser anterior a la fecha inicial.");
        }

        if (request.IngresosAdicionales < 0)
        {
            throw new InvalidOperationException(
                "Los ingresos adicionales no pueden ser negativos.");
        }

        bool planillaDuplicada = await _context.Planillas
            .AsNoTracking()
            .AnyAsync(
                p =>
                    p.IdEmpleado == empleado.IdEmpleado &&
                    p.FechaInicioPeriodo ==
                        request.FechaInicioPeriodo &&
                    p.FechaFinPeriodo ==
                        request.FechaFinPeriodo,
                cancellationToken);

        if (planillaDuplicada)
        {
            throw new InvalidOperationException(
                "Ya existe una planilla para ese empleado y periodo.");
        }

        List<DeduccionAplicarDto> deduccionesSolicitadas =
            request.Deducciones ??
            new List<DeduccionAplicarDto>();

        bool existenTiposRepetidos =
            deduccionesSolicitadas
                .GroupBy(d => d.IdTipoDeduccion)
                .Any(grupo => grupo.Count() > 1);

        if (existenTiposRepetidos)
        {
            throw new InvalidOperationException(
                "No se puede aplicar dos veces el mismo tipo de deducción.");
        }

        List<int> idsTiposDeduccion =
            deduccionesSolicitadas
                .Select(d => d.IdTipoDeduccion)
                .Distinct()
                .ToList();

        List<TipoDeduccion> tiposDeduccion =
            await _context.TiposDeduccion
                .AsNoTracking()
                .Where(tipo =>
                    idsTiposDeduccion.Contains(
                        tipo.IdTipoDeduccion) &&
                    tipo.Activo)
                .ToListAsync(cancellationToken);

        if (tiposDeduccion.Count !=
            idsTiposDeduccion.Count)
        {
            throw new InvalidOperationException(
                "Uno o más tipos de deducción no existen o están inactivos.");
        }

        decimal salarioBasePeriodo =
            empleado.SalarioBase;

        decimal salarioBruto =
            salarioBasePeriodo +
            request.IngresosAdicionales;

        var deduccionesPreparadas =
            new List<(
                DeduccionPlanilla Entidad,
                TipoDeduccion Tipo)>();

        decimal totalDeducciones = 0;

        foreach (DeduccionAplicarDto solicitud
                 in deduccionesSolicitadas)
        {
            TipoDeduccion tipo =
                tiposDeduccion.First(
                    t =>
                        t.IdTipoDeduccion ==
                        solicitud.IdTipoDeduccion);

            decimal valorAplicado =
                solicitud.ValorAplicado ??
                tipo.ValorPredeterminado ??
                0;

            if (valorAplicado < 0)
            {
                throw new InvalidOperationException(
                    $"El valor de la deducción {tipo.Nombre} " +
                    "no puede ser negativo.");
            }

            decimal montoCalculado;

            if (tipo.TipoCalculo ==
                TipoCalculoDeduccion.Fijo)
            {
                montoCalculado = valorAplicado;
            }
            else if (tipo.TipoCalculo ==
                     TipoCalculoDeduccion.Porcentaje)
            {
                if (valorAplicado > 100)
                {
                    throw new InvalidOperationException(
                        $"El porcentaje de {tipo.Nombre} " +
                        "no puede superar el 100%.");
                }

                montoCalculado = Math.Round(
                    salarioBruto *
                    valorAplicado / 100m,
                    2,
                    MidpointRounding.AwayFromZero);
            }
            else
            {
                throw new InvalidOperationException(
                    $"El tipo de cálculo de {tipo.Nombre} " +
                    "no es válido.");
            }

            totalDeducciones += montoCalculado;

            var deduccionPlanilla =
                new DeduccionPlanilla
                {
                    IdTipoDeduccion =
                        tipo.IdTipoDeduccion,

                    ValorAplicado =
                        valorAplicado,

                    MontoCalculado =
                        montoCalculado,

                    Observacion =
                        string.IsNullOrWhiteSpace(
                            solicitud.Observacion)
                            ? null
                            : solicitud.Observacion.Trim(),

                    FechaCreacion =
                        DateTime.UtcNow
                };

            deduccionesPreparadas.Add(
                (deduccionPlanilla, tipo));
        }

        totalDeducciones = Math.Round(
            totalDeducciones,
            2,
            MidpointRounding.AwayFromZero);

        if (totalDeducciones > salarioBruto)
        {
            throw new InvalidOperationException(
                "El total de deducciones no puede superar el salario bruto.");
        }

        decimal salarioNeto = Math.Round(
            salarioBruto - totalDeducciones,
            2,
            MidpointRounding.AwayFromZero);

        DateTime fechaActual = DateTime.UtcNow;

        var planilla = new Planilla
        {
            IdEmpleado = empleado.IdEmpleado,

            IdAdministrador =
                administrador.IdAdministrador,

            FechaInicioPeriodo =
                request.FechaInicioPeriodo,

            FechaFinPeriodo =
                request.FechaFinPeriodo,

            SalarioBasePeriodo =
                salarioBasePeriodo,

            IngresosAdicionales =
                request.IngresosAdicionales,

            TotalDeducciones =
                totalDeducciones,

            SalarioNeto =
                salarioNeto,

            Estado =
                EstadoPlanilla.Generada,

            FechaCreacion =
                fechaActual,

            FechaActualizacion =
                null
        };

        await _context.Planillas.AddAsync(
            planilla,
            cancellationToken);

        foreach (var deduccionPreparada
                 in deduccionesPreparadas)
        {
            deduccionPreparada.Entidad.Planilla =
                planilla;

            await _context.DeduccionesPlanilla.AddAsync(
                deduccionPreparada.Entidad,
                cancellationToken);
        }

        await _context.SaveChangesAsync(
            cancellationToken);

        return new PlanillaDto
        {
            IdPlanilla =
                planilla.IdPlanilla,

            IdEmpleado =
                empleado.IdEmpleado,

            CodigoEmpleado =
                empleado.CodigoEmpleado,

            NombreEmpleado =
                empleado.Nombres + " " +
                empleado.Apellidos,

            IdAdministrador =
                administrador.IdAdministrador,

            NombreAdministrador =
                administrador.Nombres + " " +
                administrador.Apellidos,

            FechaInicioPeriodo =
                planilla.FechaInicioPeriodo,

            FechaFinPeriodo =
                planilla.FechaFinPeriodo,

            SalarioBasePeriodo =
                planilla.SalarioBasePeriodo,

            IngresosAdicionales =
                planilla.IngresosAdicionales,

            SalarioBruto =
                salarioBruto,

            TotalDeducciones =
                planilla.TotalDeducciones,

            SalarioNeto =
                planilla.SalarioNeto,

            Estado =
                planilla.Estado,

            FechaCreacion =
                planilla.FechaCreacion,

            FechaActualizacion =
                planilla.FechaActualizacion,

            Deducciones =
                deduccionesPreparadas
                    .Select(item =>
                        new DeduccionPlanillaDto
                        {
                            IdDeduccionPlanilla =
                                item.Entidad
                                    .IdDeduccionPlanilla,

                            IdTipoDeduccion =
                                item.Tipo
                                    .IdTipoDeduccion,

                            NombreTipoDeduccion =
                                item.Tipo.Nombre,

                            TipoCalculo =
                                item.Tipo.TipoCalculo,

                            ValorAplicado =
                                item.Entidad
                                    .ValorAplicado,

                            MontoCalculado =
                                item.Entidad
                                    .MontoCalculado,

                            Observacion =
                                item.Entidad
                                    .Observacion
                        })
                    .ToList()
        };
    }
}