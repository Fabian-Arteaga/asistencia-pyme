using System.Threading;
using System.Threading.Tasks;

namespace AsistenciaPyme.Application.Common.Interfaces;

public interface IHistoricalDataSeeder
{
    Task SeedHistoricalDataAsync(CancellationToken cancellationToken = default);
}
