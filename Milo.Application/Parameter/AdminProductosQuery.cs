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
    public class AdminProductosQuery : IRequest<Info_data>
    {
        public string Opcion {  get; set; }
        public ProductoDTO Producto { get; set; }

        public AdminProductosQuery(string opcion, ProductoDTO producto)
        {
            this.Producto = producto;
            this.Opcion = opcion;
        }
    }
}
