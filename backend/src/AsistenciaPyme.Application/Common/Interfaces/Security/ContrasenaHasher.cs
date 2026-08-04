using AsistenciaPyme.Application.Common.Interfaces;
using System.Security.Cryptography;

namespace AsistenciaPyme.Infrastructure.Security;

public class ContrasenaHasher : IContrasenaHasher
{
    private const int Iteraciones = 100000;
    private const int TamanoSalt = 16;
    private const int TamanoHash = 32;

    public string CrearHash(string contrasena)
    {
        byte[] salt =
            RandomNumberGenerator.GetBytes(TamanoSalt);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            contrasena,
            salt,
            Iteraciones,
            HashAlgorithmName.SHA256,
            TamanoHash);

        return string.Join(
            "$",
            "PBKDF2-SHA256",
            Iteraciones,
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash));
    }

    public bool Verificar(
        string contrasena,
        string contrasenaHash)
    {
        try
        {
            string[] partes =
                contrasenaHash.Split('$');

            if (partes.Length != 4 ||
                partes[0] != "PBKDF2-SHA256")
            {
                return false;
            }

            int iteraciones =
                int.Parse(partes[1]);

            byte[] salt =
                Convert.FromBase64String(partes[2]);

            byte[] hashGuardado =
                Convert.FromBase64String(partes[3]);

            byte[] hashCalculado =
                Rfc2898DeriveBytes.Pbkdf2(
                    contrasena,
                    salt,
                    iteraciones,
                    HashAlgorithmName.SHA256,
                    hashGuardado.Length);

            return CryptographicOperations.FixedTimeEquals(
                hashGuardado,
                hashCalculado);
        }
        catch
        {
            return false;
        }
    }
}