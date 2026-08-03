using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsistenciaPyme.Application.Feautures.Cargos.Commands
{
    public class CrearCargoCommand : IRequest<int>
    {
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public string Funciones { get; set; } = string.Empty;

        public string Responsabilidades { get; set; } = string.Empty;
    }
}
