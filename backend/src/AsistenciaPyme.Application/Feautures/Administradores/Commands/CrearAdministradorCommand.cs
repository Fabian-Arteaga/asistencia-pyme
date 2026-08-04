using AsistenciaPyme.Application.Features.Administradores.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Features.Administradores.Commands;

public class CrearAdministradorCommand
    : IRequest<AdministradorDto>
{
    public string Nombres { get; set; } =
        string.Empty;

    public string Apellidos { get; set; } =
        string.Empty;

    public string Correo { get; set; } =
        string.Empty;

    public string Contrasena { get; set; } =
        string.Empty;
}