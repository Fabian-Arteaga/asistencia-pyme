using AsistenciaPyme.Application.Features.Planillas.DTOs;
using MediatR;
using System;

namespace AsistenciaPyme.Application.Features.Planillas.Commands
{
    public class GenerarPlanillaDepartamentoCommand : IRequest<PlanillaDto?>
    {
        public int IdDepartamento { get; set; }

        public int IdAdministrador { get; set; }

        public DateOnly FechaInicioPeriodo { get; set; }

        public DateOnly FechaFinPeriodo { get; set; }

        public System.Collections.Generic.List<int>? IdsEmpleadosSeleccionados { get; set; }
    }
}
