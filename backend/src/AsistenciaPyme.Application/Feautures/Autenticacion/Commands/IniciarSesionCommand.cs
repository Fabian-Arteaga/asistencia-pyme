using AsistenciaPyme.Application
    .Features.Autenticacion.DTOs;
using MediatR;

namespace AsistenciaPyme.Application
    .Features.Autenticacion.Commands;

public class IniciarSesionCommand
    : IRequest<LoginResponseDto?>
{
    public string Correo { get; set; } =
        string.Empty;

    public string Contrasena { get; set; } =
        string.Empty;
}