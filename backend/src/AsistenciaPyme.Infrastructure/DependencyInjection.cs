using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Infrastructure.Persistence;
using AsistenciaPyme.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AsistenciaPyme.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string? connectionString =
            configuration.GetConnectionString(
                "AsistenciaPymeDatabase");

        if (string.IsNullOrWhiteSpace(
                connectionString))
        {
            throw new InvalidOperationException(
                "No se encontró la cadena de conexión AsistenciaPymeDatabase.");
        }

        services.AddDbContext<
            AsistenciaPymeDbContext>(
            options =>
                options.UseNpgsql(
                    connectionString));

        services.AddScoped<
            IAsistenciaPymeDbContext>(
            provider =>
                provider.GetRequiredService<
                    AsistenciaPymeDbContext>());

        services.AddScoped<
            IPinHasher,
            PinHasher>();

        services.AddScoped<
            IContrasenaHasher,
            ContrasenaHasher>();

        services.AddScoped<
            IJwtTokenGenerator,
            JwtTokenGenerator>();

        services.AddScoped<AsistenciaPyme.Application.Common.Interfaces.ICalculadorHorasExtras, Persistence.CalculadorHorasExtras>();
        services.AddScoped<AsistenciaPyme.Application.Common.Interfaces.ICalculadorIndemnizacion, Persistence.CalculadorIndemnizacion>();

        return services;
    }
}