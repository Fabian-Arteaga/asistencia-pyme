using AsistenciaPyme.Application.Feautures.VerificacionesPresencia.Commands;

namespace AsistenciaPyme.Application.Feautures.VerificacionesPresencia.Validators;

public class RegistrarVerificacionPresenciaValidator
{
    public void Validate(RegistrarVerificacionPresenciaCommand command)
    {
        if (command.IdEmpleado <= 0)
        {
            throw new InvalidOperationException("Debe indicar un empleado válido.");
        }

        if (command.IdDispositivoMarcaje <= 0)
        {
            throw new InvalidOperationException("Debe indicar un dispositivo válido.");
        }
    }
}
