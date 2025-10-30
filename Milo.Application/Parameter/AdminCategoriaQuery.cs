using MediatR;
using Milo.Application.Models.DTOs;
using Milo.Application.Parameter.DTOs;
using Milo.Application.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Application.Parameter
{
    public class AdminCategoriaQuery : IRequest<Info_data>
    {
        public string Opcion {  get; set; }
        public CategoriaDTO Categoria {  get; set; }
        public AdminCategoriaQuery(string opcion, CategoriaDTO categoria)
        {
            this.Opcion = opcion;
            this.Categoria = categoria;
        }
    }
}
