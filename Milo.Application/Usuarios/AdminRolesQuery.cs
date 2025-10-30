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
    public class AdminRolesQuery : IRequest<Info_data>
    {
        public string Opcion {  get; set; }
        public GetRolesOut Rol {  get; set; }
        public AdminRolesQuery(string opcion, GetRolesOut rol) {
            this.Rol=rol;
            this.Opcion = opcion;
        }
    }
}
