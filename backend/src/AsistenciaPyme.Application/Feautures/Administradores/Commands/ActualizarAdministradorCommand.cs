using AsistenciaPyme.Application.Features.Administradores.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace AsistenciaPyme.Application.Features.Administradores.Commands;

public class ActualizarAdministradorCommand
    : IRequest<AdministradorDto?>
{
    [JsonIgnore]
    public int IdAdministrador { get; set; }

    public string Nombres { get; set; } =
        string.Empty;

    public string Apellidos { get; set; } =
        string.Empty;

    public string Correo { get; set; } =
        string.Empty;

    public string? NuevaContrasena { get; set; }
}