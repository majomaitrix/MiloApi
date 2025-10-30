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
    public class CreateUserQuery : IRequest<Info_data>
    {
        public CreateUserIn User { get; set; }
        public CreateUserQuery(CreateUserIn user)
        {
            this.User = user;
        }
    }
}
