using AsistenciaPyme.Application.Common.Models;
using AsistenciaPyme.Domain.Entities;

namespace AsistenciaPyme.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    TokenGenerado Generar(
        Administrador administrador);
}