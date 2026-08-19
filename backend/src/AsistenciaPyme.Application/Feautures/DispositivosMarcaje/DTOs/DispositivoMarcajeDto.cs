namespace AsistenciaPyme.Application.Feautures.DispositivosMarcaje.DTOs;

public class DispositivoMarcajeDto
{
    public int IdDispositivoMarcaje { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Tipo { get; set; }
    public int? IdDepartamento { get; set; }
    public string NombreDepartamento { get; set; } = string.Empty;
    public string Identificador { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
