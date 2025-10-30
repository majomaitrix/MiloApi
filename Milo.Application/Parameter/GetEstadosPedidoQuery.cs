using MediatR;
using Milo.Application.Models;
using Milo.Application.Models.DTOs;
using Milo.Application.Parameter.DTOs;
using Milo.Application.Parameter.Services;
using Milo.Domain.Entities;

namespace Milo.Application.Parameter
{
    public class GetEstadosPedidoQuery : IRequest<Info_data_list<EstadoPedidoDTO>>
    {
    }
}
