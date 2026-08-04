using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AsistenciaPyme.Application.Features.Administradores.Commands;

public class CambiarContrasenaAdministradorCommand
    : IRequest<bool>
{
    [JsonIgnore]
    public int IdAdministrador { get; set; }

    [Required(
        ErrorMessage = "La contraseña actual es obligatoria.")]
    [StringLength(
        200,
        ErrorMessage =
            "La contraseña actual no puede superar los 200 caracteres.")]
    public string ContrasenaActual { get; set; } =
        string.Empty;

    [Required(
        ErrorMessage = "La nueva contraseña es obligatoria.")]
    [StringLength(
        100,
        MinimumLength = 8,
        ErrorMessage =
            "La nueva contraseña debe tener entre 8 y 100 caracteres.")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)\S{8,100}$",
        ErrorMessage =
            "La nueva contraseña debe contener una mayúscula, " +
            "una minúscula, un número y no debe tener espacios.")]
    public string NuevaContrasena { get; set; } =
        string.Empty;

    [Required(
        ErrorMessage =
            "Debe confirmar la nueva contraseña.")]
    [Compare(
        nameof(NuevaContrasena),
        ErrorMessage =
            "La confirmación no coincide con la nueva contraseña.")]
    public string ConfirmarNuevaContrasena { get; set; } =
        string.Empty;
}