using AsistenciaPyme.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AsistenciaPyme.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
      this IServiceCollection services,
      IConfiguration configuration)
        {
            string connectionString =
                configuration.GetConnectionString("AsistenciaPymeDatabase")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión AsistenciaPymeDatabase.");

            services.AddDbContext<AsistenciaPymeDbContext>(options =>
                options.UseNpgsql(connectionString));

            return services;
        }
    }
}
