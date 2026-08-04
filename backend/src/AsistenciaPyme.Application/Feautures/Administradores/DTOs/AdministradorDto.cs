namespace AsistenciaPyme.Application.Features.Administradores.DTOs;

public class AdministradorDto
{
    public int IdAdministrador { get; set; }

    public string Nombres { get; set; } =
        string.Empty;

    public string Apellidos { get; set; } =
        string.Empty;

    public string Correo { get; set; } =
        string.Empty;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }
}