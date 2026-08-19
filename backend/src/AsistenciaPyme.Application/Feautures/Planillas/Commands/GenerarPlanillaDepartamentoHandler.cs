using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Features.Planillas.DTOs;
using AsistenciaPyme.Domain.Entities;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaPyme.Application.Features.Planillas.Commands
{
    public class GenerarPlanillaDepartamentoHandler : IRequestHandler<GenerarPlanillaDepartamentoCommand, PlanillaDto?>
    {
        private readonly IAsistenciaPymeDbContext _context;
        private readonly ICalculadorHorasExtras _calculadorHorasExtras;

        public GenerarPlanillaDepartamentoHandler(IAsistenciaPymeDbContext context, ICalculadorHorasExtras calculadorHorasExtras)
        {
            _context = context;
            _calculadorHorasExtras = calculadorHorasExtras;
        }

        public async Task<PlanillaDto?> Handle(GenerarPlanillaDepartamentoCommand request, CancellationToken cancellationToken)
        {
            if (request.FechaFinPeriodo < request.FechaInicioPeriodo)
            {
                throw new InvalidOperationException("La fecha final no puede ser anterior a la fecha inicial.");
            }

            var departamento = await _context.Departamentos
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.IdDepartamento == request.IdDepartamento && d.Activo, cancellationToken);

            if (departamento is null)
            {
                throw new InvalidOperationException("El departamento no existe o está inactivo.");
            }

            bool planillaDuplicada = await _context.Planillas
                .AsNoTracking()
                .AnyAsync(p => p.IdDepartamento == request.IdDepartamento
                    && p.FechaInicioPeriodo == request.FechaInicioPeriodo
                    && p.FechaFinPeriodo == request.FechaFinPeriodo, cancellationToken);

            if (planillaDuplicada)
            {
                throw new InvalidOperationException("Ya existe una planilla para ese departamento y periodo.");
            }

            var administrador = await _context.Administradores
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.IdAdministrador == request.IdAdministrador && a.Activo, cancellationToken);

            if (administrador is null)
            {
                throw new InvalidOperationException("El administrador no existe o está inactivo.");
            }

            var configuracion = await _context.ConfiguracionesNomina
                .AsNoTracking()
                .FirstOrDefaultAsync(c => true, cancellationToken);

            decimal multiplicador = configuracion?.MultiplicadorHoraExtra ?? 2.0m;

            // Obtener todos los empleados activos del departamento
            var empleadosDepto = await _context.Empleados
                .AsNoTracking()
                .Where(e => e.IdDepartamento == request.IdDepartamento && e.Estado == EstadoEmpleado.Activo)
                .ToListAsync(cancellationToken);

            if (!empleadosDepto.Any())
            {
                throw new InvalidOperationException("El departamento seleccionado no tiene colaboradores activos registrados.");
            }

            List<Empleado> empleadosAProcesar;

            if (request.IdsEmpleadosSeleccionados != null)
            {
                if (!request.IdsEmpleadosSeleccionados.Any())
                {
                    throw new InvalidOperationException("Debe seleccionar al menos un colaborador para generar la planilla.");
                }

                var dictEmpleadosDepto = empleadosDepto.ToDictionary(e => e.IdEmpleado);
                var invalidos = request.IdsEmpleadosSeleccionados.Where(id => !dictEmpleadosDepto.ContainsKey(id)).ToList();
                if (invalidos.Any())
                {
                    throw new InvalidOperationException("Uno o más colaboradores seleccionados no pertenecen al departamento indicado o no están activos.");
                }

                empleadosAProcesar = request.IdsEmpleadosSeleccionados
                    .Distinct()
                    .Select(id => dictEmpleadosDepto[id])
                    .ToList();
            }
            else
            {
                empleadosAProcesar = empleadosDepto;
            }

            if (!empleadosAProcesar.Any())
            {
                throw new InvalidOperationException("Debe seleccionar al menos un colaborador para generar la planilla.");
            }

            var fechaInicioUtc = request.FechaInicioPeriodo.ToDateTime(new TimeOnly(0, 0), DateTimeKind.Utc);
            var fechaFinUtc = request.FechaFinPeriodo.ToDateTime(new TimeOnly(23, 59), DateTimeKind.Utc);

            var planilla = new Planilla
            {
                IdDepartamento = departamento.IdDepartamento,
                IdAdministrador = administrador.IdAdministrador,
                FechaInicioPeriodo = request.FechaInicioPeriodo,
                FechaFinPeriodo = request.FechaFinPeriodo,
                Estado = EstadoPlanilla.Calculada,
                FechaGeneracion = DateTime.UtcNow,
                FechaCreacion = DateTime.UtcNow
            };

            await _context.Planillas.AddAsync(planilla, cancellationToken);

            var detalles = new List<DetallePlanilla>();

            foreach (var empleado in empleadosAProcesar)
            {
                // resumen de asistencias en el periodo
                var asistencias = await _context.Asistencias
                    .AsNoTracking()
                    .Where(a => a.IdEmpleado == empleado.IdEmpleado
                        && a.HoraEntrada >= fechaInicioUtc
                        && a.HoraEntrada <= fechaFinUtc)
                    .ToListAsync(cancellationToken);

                int cantidadTardanzas = asistencias.Count(a => a.EsEntradaTardia);
                int minutosTardanza = asistencias.Sum(a => a.MinutosTardanza);

                // horas extra aprobadas en periodo
                var horasExtrasAprobadas = await _context.HorasExtras
                    .AsNoTracking()
                    .Where(h => h.IdEmpleado == empleado.IdEmpleado
                        && h.Fecha >= fechaInicioUtc
                        && h.Fecha <= fechaFinUtc
                        && h.Estado == EstadoHoraExtra.Aprobada)
                    .ToListAsync(cancellationToken);

                int minutosExtrasAprobados = horasExtrasAprobadas.Sum(h => h.MinutosAprobados);
                int minutosExtrasDetectados = horasExtrasAprobadas.Sum(h => h.MinutosDetectados);

                decimal montoHorasExtras = _calculadorHorasExtras.CalcularMontoHorasExtras(empleado.SalarioBase, minutosExtrasAprobados, multiplicador);

                decimal descuentoTardanza = 0m; // politica inicial SinDeduccion

                decimal totalIngresos = montoHorasExtras;
                decimal totalDeducciones = descuentoTardanza;
                decimal salarioNeto = Math.Round(empleado.SalarioBase + totalIngresos - totalDeducciones, 2, MidpointRounding.AwayFromZero);

                var detalle = new DetallePlanilla
                {
                    IdEmpleado = empleado.IdEmpleado,
                    CodigoEmpleado = empleado.CodigoEmpleado,
                    NombreEmpleado = empleado.Nombres + " " + empleado.Apellidos,
                    NumeroINSS = empleado.NumeroINSS,
                    Cargo = empleado.Cargo?.Nombre ?? string.Empty,
                    Departamento = departamento.Nombre,
                    SalarioBase = empleado.SalarioBase,
                    DiasLaborados = 0,
                    MinutosLaborados = 0,
                    CantidadTardanzas = cantidadTardanzas,
                    MinutosTardanza = minutosTardanza,
                    DescuentoTardanza = descuentoTardanza,
                    MinutosExtrasDetectados = minutosExtrasDetectados,
                    MinutosExtrasAprobados = minutosExtrasAprobados,
                    MontoHorasExtras = montoHorasExtras,
                    VacacionesAcumuladasPeriodo = 0m,
                    SaldoVacaciones = 0m,
                    TotalIngresos = totalIngresos,
                    TotalDeducciones = totalDeducciones,
                    SalarioNeto = salarioNeto,
                    IndemnizacionProyectada = 0m,
                    FechaCreacion = DateTime.UtcNow,
                    Planilla = planilla
                };

                detalles.Add(detalle);
            }

            await _context.DetallesPlanilla.AddRangeAsync(detalles, cancellationToken);

            // Calcular totales departamentales
            planilla.CantidadEmpleados = detalles.Count;
            planilla.TotalSalarioBase = detalles.Sum(d => d.SalarioBase);
            planilla.TotalHorasExtras = detalles.Sum(d => d.MontoHorasExtras);
            planilla.TotalIngresos = detalles.Sum(d => d.TotalIngresos);
            planilla.TotalDeducciones = detalles.Sum(d => d.TotalDeducciones);
            planilla.TotalNeto = detalles.Sum(d => d.SalarioNeto);

            await _context.SaveChangesAsync(cancellationToken);

            // Preparar DTO
            var planillaDto = new PlanillaDto
            {
                IdPlanilla = planilla.IdPlanilla,
                IdDepartamento = planilla.IdDepartamento,
                NombreDepartamento = departamento.Nombre,
                FechaInicioPeriodo = planilla.FechaInicioPeriodo,
                FechaFinPeriodo = planilla.FechaFinPeriodo,
                CantidadEmpleados = planilla.CantidadEmpleados,
                TotalSalarioBase = planilla.TotalSalarioBase,
                TotalHorasExtras = planilla.TotalHorasExtras,
                TotalIngresos = planilla.TotalIngresos,
                TotalDeducciones = planilla.TotalDeducciones,
                TotalNeto = planilla.TotalNeto,
                Estado = planilla.Estado,
                FechaGeneracion = planilla.FechaGeneracion,
                FechaCreacion = planilla.FechaCreacion,
                Detalles = detalles.Select(d => new DetallePlanillaDto
                {
                    IdDetallePlanilla = d.IdDetallePlanilla,
                    IdEmpleado = d.IdEmpleado,
                    CodigoEmpleado = d.CodigoEmpleado,
                    NombreEmpleado = d.NombreEmpleado,
                    NumeroINSS = d.NumeroINSS,
                    Cargo = d.Cargo,
                    Departamento = d.Departamento,
                    SalarioBase = d.SalarioBase,
                    DiasLaborados = d.DiasLaborados,
                    MinutosLaborados = d.MinutosLaborados,
                    CantidadTardanzas = d.CantidadTardanzas,
                    MinutosTardanza = d.MinutosTardanza,
                    DescuentoTardanza = d.DescuentoTardanza,
                    MinutosExtrasDetectados = d.MinutosExtrasDetectados,
                    MinutosExtrasAprobados = d.MinutosExtrasAprobados,
                    MontoHorasExtras = d.MontoHorasExtras,
                    VacacionesAcumuladasPeriodo = d.VacacionesAcumuladasPeriodo,
                    SaldoVacaciones = d.SaldoVacaciones,
                    TotalIngresos = d.TotalIngresos,
                    TotalDeducciones = d.TotalDeducciones,
                    SalarioNeto = d.SalarioNeto,
                    IndemnizacionProyectada = d.IndemnizacionProyectada
                }).ToList()
            };

            return planillaDto;
        }
    }
}
