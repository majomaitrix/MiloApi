using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Application.Parameter.DTOs
{
    public class MesasDTO
    {
        public int id {  get; set; }
        public string nombre { get; set; }
        public int numero { get; set; }
        public string estado { get; set; }
        public PedidoDTO pedido { get; set; }
        public MeseroUserDTO mesero { get; set; }
    }
}
