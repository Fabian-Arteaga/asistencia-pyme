namespace AsistenciaPyme.Application.Common.Interfaces;

public interface IContrasenaHasher
{
    string CrearHash(string contrasena);

    bool Verificar(
        string contrasena,
        string contrasenaHash);
}