namespace AsistenciaPyme.Application.Common.Models;

public class TokenGenerado
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiracionUtc { get; set; }
}