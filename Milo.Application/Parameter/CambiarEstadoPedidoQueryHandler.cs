using MediatR;
using Microsoft.EntityFrameworkCore;
using Milo.Application.Models;
using Milo.Application.Models.DTOs;
using Milo.Application.Parameter.DTOs;
using Milo.Application.Parameter.Services;
using Milo.Domain.Entities;
using Milo.Infrastructure.Persistence;
using Serilog;

namespace Milo.Application.Parameter
{
    public sealed class CambiarEstadoPedidoQueryHandler : IRequestHandler<CambiarEstadoPedidoQuery, Info_data_obj<PedidoConEstadoDTO>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IEstadoPedidoService _estadoPedidoService;

        public CambiarEstadoPedidoQueryHandler(ApplicationDbContext context, IEstadoPedidoService estadoPedidoService)
        {
            _context = context;
            _estadoPedidoService = estadoPedidoService;
        }

        public async Task<Info_data_obj<PedidoConEstadoDTO>> Handle(CambiarEstadoPedidoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Log.Information("Iniciando cambio de estado para pedido {PedidoId} a estado {NuevoEstado}", 
                    request.Request.PedidoId, request.Request.NuevoEstado);

                // 1. Buscar el pedido
                var pedido = await _context.pedidos
                    .FirstOrDefaultAsync(p => p.Id == request.Request.PedidoId, cancellationToken);

                if (pedido == null)
                {
                    Log.Warning("Pedido {PedidoId} no encontrado", request.Request.PedidoId);
                    return new Info_data_obj<PedidoConEstadoDTO>
                    {
                        code = Constants.CodeCatch,
                        message = Constants.MessageNotFound,
                        message_error = "Pedido no encontrado",
                        Data = null
                    };
                }

                // 2. Validar transición de estado
                if (!_estadoPedidoService.EsTransicionValida(pedido.Estado, request.Request.NuevoEstado))
                {
                    Log.Warning("Transición inválida de {EstadoActual} a {NuevoEstado} para pedido {PedidoId}", 
                        pedido.Estado, request.Request.NuevoEstado, request.Request.PedidoId);
                    
                    return new Info_data_obj<PedidoConEstadoDTO>
                    {
                        code = Constants.CodeCatch,
                        message = "Transición de estado inválida",
                        message_error = $"No se puede cambiar de {pedido.Estado} a {request.Request.NuevoEstado}",
                        Data = null
                    };
                }

                // 3. Actualizar el pedido
                var estadoAnterior = pedido.Estado;
                pedido.Estado = request.Request.NuevoEstado;
                
                if (!string.IsNullOrEmpty(request.Request.Observaciones))
                {
                    pedido.Observaciones = request.Request.Observaciones;
                }

                await _context.SaveChangesAsync(cancellationToken);

                Log.Information("Estado del pedido {PedidoId} cambiado de {EstadoAnterior} a {NuevoEstado}", 
                    request.Request.PedidoId, estadoAnterior, request.Request.NuevoEstado);

                // 4. Retornar el pedido actualizado
                var pedidoActualizado = new PedidoConEstadoDTO
                {
                    Id = pedido.Id,
                    UsuarioId = pedido.UsuarioId ?? 0,
                    Fecha = pedido.Fecha,
                    Total = pedido.Total,
                    Estado = pedido.Estado,
                    Observaciones = pedido.Observaciones,
                    EstadoNombre = _estadoPedidoService.ObtenerDescripcionEstado(pedido.Estado)
                };

                return new Info_data_obj<PedidoConEstadoDTO>
                {
                    code = Constants.CodeSuccess,
                    message = Constants.MessageSuccess,
                    message_error = "",
                    Data = pedidoActualizado
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al cambiar estado del pedido {PedidoId}", request.Request.PedidoId);
                return new Info_data_obj<PedidoConEstadoDTO>
                {
                    code = Constants.CodeCatch,
                    message = Constants.MessageCatch,
                    message_error = ex.Message,
                    Data = null
                };
            }
        }
    }
}
