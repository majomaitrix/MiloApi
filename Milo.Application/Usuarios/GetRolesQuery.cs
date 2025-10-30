using MediatR;
using Milo.Application.Models.DTOs;
using Milo.Application.Usuarios.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Application.Usuarios
{
    public class GetRolesQuery : IRequest<Info_data_list<GetRolesOut>>
    {
        public GetRolesQuery() { }
    }
}
