using AsistenciaPyme.Application.Feautures.Cargos.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsistenciaPyme.Application.Feautures.Cargos.Queries
{
    public class ObtenerCargosQuery : IRequest<List<CargoDTO>>
    {
    }
}
