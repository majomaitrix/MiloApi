using MediatR;
using Milo.Application.Models.DTOs;
using Milo.Application.Parameter.DTOs;
using Milo.Domain.Entities;

namespace Milo.Application.Parameter
{
    public class GetPedidosQuery : IRequest<PagedResponse<PedidoDTO>>
    {
        public PagedRequestWithFilters PagedRequest { get; set; }
        public EstadoPedido? EstadoFiltro { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int? UsuarioId { get; set; }

        public GetPedidosQuery(PagedRequestWithFilters pagedRequest, EstadoPedido? estadoFiltro = null, 
            DateTime? fechaDesde = null, DateTime? fechaHasta = null, int? usuarioId = null)
        {
            PagedRequest = pagedRequest;
            EstadoFiltro = estadoFiltro;
            FechaDesde = fechaDesde;
            FechaHasta = fechaHasta;
            UsuarioId = usuarioId;
        }
    }
}


