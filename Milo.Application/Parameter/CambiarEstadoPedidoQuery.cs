using MediatR;
using Milo.Application.Models;
using Milo.Application.Models.DTOs;
using Milo.Application.Parameter.DTOs;
using Milo.Application.Parameter.Services;
using Milo.Domain.Entities;

namespace Milo.Application.Parameter
{
    public class CambiarEstadoPedidoQuery : IRequest<Info_data_obj<PedidoConEstadoDTO>>
    {
        public CambiarEstadoPedidoIn Request { get; set; }

        public CambiarEstadoPedidoQuery(CambiarEstadoPedidoIn request)
        {
            Request = request;
        }
    }
}
