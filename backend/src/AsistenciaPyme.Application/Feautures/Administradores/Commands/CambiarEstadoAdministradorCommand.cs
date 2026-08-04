using AsistenciaPyme.Application.Features.Administradores.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace AsistenciaPyme.Application.Features.Administradores.Commands;

public class CambiarEstadoAdministradorCommand
    : IRequest<AdministradorDto?>
{
    [JsonIgnore]
    public int IdAdministrador { get; set; }

    public bool Activo { get; set; }
}