using AsistenciaPyme.Application.Common.Interfaces;
using AsistenciaPyme.Application.Feautures.ConfiguracionNomina.DTOs;
using AsistenciaPyme.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AsistenciaPyme.Application.Feautures.ConfiguracionNomina.Commands;
public class ActualizarConfiguracionNominaHandler : IRequestHandler<ActualizarConfiguracionNominaCommand, ConfiguracionNominaDto>
{
    private readonly IAsistenciaPymeDbContext _context;
    public ActualizarConfiguracionNominaHandler(IAsistenciaPymeDbContext context)
    {
        _context = context;
    }
    public async Task<ConfiguracionNominaDto> Handle(ActualizarConfiguracionNominaCommand request, CancellationToken cancellationToken)
    {
        var configuracion = await _context.ConfiguracionesNomina.FirstOrDefaultAsync(c => c.IdConfiguracionNomina == request.IdConfiguracionNomina, cancellationToken);
        if (configuracion is null)
        {
            configuracion = new AsistenciaPyme.Domain.Entities.ConfiguracionNomina();
            await _context.ConfiguracionesNomina.AddAsync(configuracion, cancellationToken);
        }
        configuracion.MinutosToleranciaEntrada = request.MinutosToleranciaEntrada;
        configuracion.MultiplicadorHoraExtra = request.MultiplicadorHoraExtra;
        configuracion.DiasVacacionesPorMes = request.DiasVacacionesPorMes;
        configuracion.DiasBaseProrrateoVacaciones = request.DiasBaseProrrateoVacaciones;
        configuracion.PoliticaDescuentoTardanza = (PoliticaDescuentoTardanza)request.PoliticaDescuentoTardanza;
        configuracion.MinutosMaximosVerificacionDepartamento = request.MinutosMaximosVerificacionDepartamento;
        configuracion.TasaINSS = request.TasaINSS;
        configuracion.FechaActualizacion = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
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
