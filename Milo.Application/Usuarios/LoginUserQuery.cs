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
    public class LoginUserQuery : IRequest<Info_data_obj<LoginUserOut>>
    {
        public LoginUserIn User {  get; set; }
        public LoginUserQuery(LoginUserIn user) {
            this.User = user;
        }
    }
}
