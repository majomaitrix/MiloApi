using Milo.Domain.Entities;

namespace Milo.Application.Parameter.Services
{
    public interface IEstadoPedidoService
    {
        bool EsTransicionValida(EstadoPedido estadoActual, EstadoPedido nuevoEstado);
        string ObtenerDescripcionEstado(EstadoPedido estado);
        List<EstadoPedido> ObtenerEstadosSiguientes(EstadoPedido estadoActual);
    }

    public class EstadoPedidoService : IEstadoPedidoService
    {
        private readonly Dictionary<EstadoPedido, List<EstadoPedido>> _transicionesValidas;

        public EstadoPedidoService()
        {
            _transicionesValidas = new Dictionary<EstadoPedido, List<EstadoPedido>>
            {
                [EstadoPedido.Pendiente] = new List<EstadoPedido> { EstadoPedido.Preparando, EstadoPedido.Cancelado },
                [EstadoPedido.Preparando] = new List<EstadoPedido> { EstadoPedido.Listo, EstadoPedido.Cancelado },
                [EstadoPedido.Listo] = new List<EstadoPedido> { EstadoPedido.Entregado },
                [EstadoPedido.Entregado] = new List<EstadoPedido>(), // Estado final
                [EstadoPedido.Cancelado] = new List<EstadoPedido>() // Estado final
            };
        }

        public bool EsTransicionValida(EstadoPedido estadoActual, EstadoPedido nuevoEstado)
        {
            if (!_transicionesValidas.ContainsKey(estadoActual))
                return false;

            return _transicionesValidas[estadoActual].Contains(nuevoEstado);
        }

        public string ObtenerDescripcionEstado(EstadoPedido estado)
        {
            return estado switch
            {
                EstadoPedido.Pendiente => "Pedido recibido, esperando preparación",
                EstadoPedido.Preparando => "En cocina preparando el pedido",
                EstadoPedido.Listo => "Pedido terminado, listo para entregar",
                EstadoPedido.Entregado => "Pedido entregado al cliente",
                EstadoPedido.Cancelado => "Pedido cancelado",
                _ => "Estado desconocido"
            };
        }

        public List<EstadoPedido> ObtenerEstadosSiguientes(EstadoPedido estadoActual)
        {
            return _transicionesValidas.ContainsKey(estadoActual) 
                ? _transicionesValidas[estadoActual] 
                : new List<EstadoPedido>();
        }
    }
}



