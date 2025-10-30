using MediatR;
using Milo.Application.Models.DTOs;
using Milo.Application.Parameter.DTOs;

namespace Milo.Application.Parameter
{
    public class GetProductosQuery : IRequest<PagedResponse<ProductoDTO>>
    {
        public PagedRequest PagedRequest { get; set; }

        public GetProductosQuery(PagedRequest pagedRequest)
        {
            PagedRequest = pagedRequest;
        }
    }
}



