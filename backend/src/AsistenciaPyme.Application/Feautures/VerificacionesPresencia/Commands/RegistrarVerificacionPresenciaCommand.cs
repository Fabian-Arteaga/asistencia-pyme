using AsistenciaPyme.Application.Feautures.VerificacionesPresencia.DTOs;
using MediatR;

namespace AsistenciaPyme.Application.Feautures.VerificacionesPresencia.Commands;

public class RegistrarVerificacionPresenciaCommand : IRequest<VerificacionPresenciaDto>
{
    public int IdEmpleado { get; set; }
    public int IdDispositivoMarcaje { get; set; }
    public DateTime FechaHora { get; set; }
    public int? IdAsistencia { get; set; }
}
