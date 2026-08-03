using AsistenciaPyme.Application.Feautures.Cargos.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsistenciaPyme.Application.Feautures.Cargos.Queries
{
    public class ObtenerCargoPorIdQuery : IRequest<CargoDTO?>
    {
        public int IdCargo { get; set; }
    }
}
