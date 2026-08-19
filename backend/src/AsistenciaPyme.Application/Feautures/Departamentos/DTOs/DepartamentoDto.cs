namespace AsistenciaPyme.Application.Feautures.Departamentos.DTOs;

public class DepartamentoDto
{
    public int IdDepartamento { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
