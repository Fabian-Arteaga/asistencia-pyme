using AsistenciaPyme.Application.Features.Planillas.DTOs;
using AsistenciaPyme.Domain.Enums;
using MediatR;
using System.Text.Json.Serialization;

namespace AsistenciaPyme.Application.Features.Planillas.Commands;

public class CambiarEstadoPlanillaCommand
    : IRequest<PlanillaDto?>
{
    [JsonIgnore]
    public int IdPlanilla { get; set; }

    public EstadoPlanilla Estado { get; set; }
}