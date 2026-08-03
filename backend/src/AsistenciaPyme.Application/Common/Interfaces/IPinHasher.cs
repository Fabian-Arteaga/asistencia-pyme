using System;
using System.Collections.Generic;
using System.Text;

namespace AsistenciaPyme.Application.Common.Interfaces
{
    public interface IPinHasher
    {
        string CrearHash(string pin);

        bool Verificar(string pin, string pinHash);
    }
}
