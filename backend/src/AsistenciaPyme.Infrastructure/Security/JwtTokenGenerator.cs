using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Common.Models;
using AsistenciaPyme.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AsistenciaPyme.Infrastructure.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TokenGenerado Generar(Administrador administrador)
    {
        string secretKey =
            _configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException(
                "No se encontró JwtSettings:SecretKey.");

        string issuer =
            _configuration["JwtSettings:Issuer"]
            ?? throw new InvalidOperationException(
                "No se encontró JwtSettings:Issuer.");

        string audience =
            _configuration["JwtSettings:Audience"]
            ?? throw new InvalidOperationException(
                "No se encontró JwtSettings:Audience.");

        int expirationMinutes = int.TryParse(
            _configuration["JwtSettings:ExpirationMinutes"],
            out int minutes)
                ? minutes
                : 480;

        if (Encoding.UTF8.GetByteCount(secretKey) < 32)
        {
            throw new InvalidOperationException(
                "La clave JWT debe tener al menos 32 caracteres.");
        }

        DateTime fechaActualUtc = DateTime.UtcNow;

        DateTime expiracionUtc =
            fechaActualUtc.AddMinutes(expirationMinutes);

        var claims = new List<Claim>
        {
            new Claim(
                "idAdministrador",
                administrador.IdAdministrador.ToString()),
            new Claim(
                ClaimTypes.NameIdentifier,
                administrador.IdAdministrador.ToString()),

            new Claim(
                ClaimTypes.Name,
                $"{administrador.Nombres} {administrador.Apellidos}"),

            new Claim(
                ClaimTypes.Email,
                administrador.Correo),

            new Claim(
                ClaimTypes.Role,
                "Administrador"),

            new Claim(
                "jti",
                Guid.NewGuid().ToString())
        };

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: fechaActualUtc,
            expires: expiracionUtc,
            signingCredentials: credentials);

        string token = new JwtSecurityTokenHandler()
            .WriteToken(jwtToken);

        return new TokenGenerado
        {
            Token = token,
            ExpiracionUtc = expiracionUtc
        };
    }
}