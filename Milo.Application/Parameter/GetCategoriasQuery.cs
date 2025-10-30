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
    public class GetCategoriasQuery : IRequest<PagedResponse<CategoriaDTO>>
    {
        public PagedRequest PagedRequest { get; set; }

        public GetCategoriasQuery(PagedRequest pagedRequest)
        {
            PagedRequest = pagedRequest;
        }
    }
}
