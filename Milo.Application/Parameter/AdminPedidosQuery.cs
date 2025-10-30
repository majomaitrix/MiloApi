using MediatR;
using Milo.Application.Models.DTOs;
using Milo.Application.Parameter.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Application.Parameter
{
    public class AdminPedidosQuery : IRequest<Info_data>
    {
        public string Opcion {  get; set; }
        public PedidoDTO Pedido {  get; set; }
        public AdminPedidosQuery(PedidoDTO pedido, string opcion)
        {
            this.Pedido = pedido;
            this.Opcion = opcion;
        }
    }
}
