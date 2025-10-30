using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Application.Usuarios.DTOs
{
    public class LoginUserOut
    {
        public int id { get; set; }
        public string nombre {  get; set; }
        public string email { get; set; }
        public GetRolesOut Rol {  get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        public string token { get; set; }
    }
}
