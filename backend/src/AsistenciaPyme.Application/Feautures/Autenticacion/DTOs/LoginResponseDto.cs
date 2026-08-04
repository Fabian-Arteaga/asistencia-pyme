namespace AsistenciaPyme.Application
    .Features.Autenticacion.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } =
        string.Empty;

    public string TipoToken { get; set; } =
        "Bearer";

    public DateTime ExpiracionUtc { get; set; }

    public int IdAdministrador { get; set; }

    public string NombreCompleto { get; set; } =
        string.Empty;

    public string Correo { get; set; } =
        string.Empty;

    public string Rol { get; set; } =
        "Administrador";
}