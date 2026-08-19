using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.ConfiguracionNomina.DTOs;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AsistenciaPyme.Application.Feautures.ConfiguracionNomina.Queries;
public class ObtenerConfiguracionNominaHandler : IRequestHandler<ObtenerConfiguracionNominaQuery, ConfiguracionNominaDto>
{
    private readonly IAsistenciaPymeDbContext _context;
    public ObtenerConfiguracionNominaHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }
    public async Task<ConfiguracionNominaDto> Handle(ObtenerConfiguracionNominaQuery request, CancellationToken cancellationToken)
    {
        var configuracion = await _context.ConfiguracionesNomina.AsNoTracking().OrderBy(c => c.IdConfiguracionNomina).FirstOrDefaultAsync(cancellationToken);
        if (configuracion is null)
        {
            configuracion = new AsistenciaPyme.Domain.Entities.ConfiguracionNomina();
            await _context.ConfiguracionesNomina.AddAsync(configuracion, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        return new ConfiguracionNominaDto
        {
            IdConfiguracionNomina = configuracion.IdConfiguracionNomina,
            MinutosToleranciaEntrada = configuracion.MinutosToleranciaEntrada,
            MultiplicadorHoraExtra = configuracion.MultiplicadorHoraExtra,
            DiasVacacionesPorMes = configuracion.DiasVacacionesPorMes,
            DiasBaseProrrateoVacaciones = configuracion.DiasBaseProrrateoVacaciones,
            PoliticaDescuentoTardanza = (int)configuracion.PoliticaDescuentoTardanza,
            MinutosMaximosVerificacionDepartamento = configuracion.MinutosMaximosVerificacionDepartamento,
            TasaINSS = configuracion.TasaINSS,
            FechaCreacion = configuracion.FechaCreacion,
            FechaActualizacion = configuracion.FechaActualizacion
        };
    }
}
