using MediatR;
using Milo.Application.Models.DTOs;
using Milo.Application.Parameter.DTOs;

namespace Milo.Application.Parameter
{
    public class GetProductosConFiltrosQuery : IRequest<PagedResponse<ProductoDTO>>
    {
        public PagedRequestWithFilters PagedRequest { get; set; }

        public GetProductosConFiltrosQuery(PagedRequestWithFilters pagedRequest)
        {
            PagedRequest = pagedRequest;
        }
    }
}


