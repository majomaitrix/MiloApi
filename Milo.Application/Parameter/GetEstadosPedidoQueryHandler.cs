using MediatR;
using Milo.Application.Models;
using Milo.Application.Models.DTOs;
using Milo.Application.Parameter.DTOs;
using Milo.Application.Parameter.Services;
using Milo.Domain.Entities;

namespace Milo.Application.Parameter
{
    public sealed class GetEstadosPedidoQueryHandler : IRequestHandler<GetEstadosPedidoQuery, Info_data_list<EstadoPedidoDTO>>
    {
        private readonly IEstadoPedidoService _estadoPedidoService;

        public GetEstadosPedidoQueryHandler(IEstadoPedidoService estadoPedidoService)
        {
            _estadoPedidoService = estadoPedidoService;
        }

        public Task<Info_data_list<EstadoPedidoDTO>> Handle(GetEstadosPedidoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var estados = Enum.GetValues<EstadoPedido>()
                    .Select(e => new EstadoPedidoDTO
                    {
                        Id = (int)e,
                        Nombre = e.ToString(),
                        Descripcion = _estadoPedidoService.ObtenerDescripcionEstado(e)
                    })
                    .ToList();

                return Task.FromResult(new Info_data_list<EstadoPedidoDTO>
                {
                    code = Constants.CodeSuccess,
                    message = Constants.MessageSuccess,
                    message_error = "",
                    List = estados
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new Info_data_list<EstadoPedidoDTO>
                {
                    code = Constants.CodeCatch,
                    message = Constants.MessageCatch,
                    message_error = ex.Message,
                    List = new List<EstadoPedidoDTO>()
                });
            }
        }
    }
}
