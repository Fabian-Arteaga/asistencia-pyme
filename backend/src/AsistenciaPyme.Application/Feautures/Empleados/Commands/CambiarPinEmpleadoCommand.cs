using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AsistenciaPyme.Application.Features.Empleados.Commands;

public class CambiarPinEmpleadoCommand : IRequest<bool>
{
    [JsonIgnore]
    public int IdEmpleado { get; set; }

    [Required(ErrorMessage = "El nuevo PIN es obligatorio.")]
    [RegularExpression(
        @"^\d{4,6}$",
        ErrorMessage =
            "El PIN debe contener únicamente entre 4 y 6 números.")]
    public string NuevoPin { get; set; } = string.Empty;
}