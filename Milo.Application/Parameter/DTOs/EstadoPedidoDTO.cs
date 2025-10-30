using System.ComponentModel.DataAnnotations;
using Milo.Domain.Entities;

namespace Milo.Application.Parameter.DTOs
{
    public class CambiarEstadoPedidoIn
    {
        [Required(ErrorMessage = "El ID del pedido es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del pedido debe ser mayor a 0")]
        public int PedidoId { get; set; }

        [Required(ErrorMessage = "El nuevo estado es obligatorio")]
        [Range(1, 5, ErrorMessage = "El estado debe ser válido (1-5)")]
        public EstadoPedido NuevoEstado { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
        public string? Observaciones { get; set; }
    }

    public class EstadoPedidoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }

    public class PedidoConEstadoDTO
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public EstadoPedido Estado { get; set; }
        public string? Observaciones { get; set; }
        public string EstadoNombre { get; set; } = string.Empty;
    }

    public class PedidoDTO
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public EstadoPedido Estado { get; set; }
        public string? Observaciones { get; set; }
        public string EstadoNombre { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
        public string UsuarioEmail { get; set; } = string.Empty;
    }

    public class PedidoConDetallesDTO : PedidoDTO
    {
        public List<PedidoDetalleDTO> Detalles { get; set; } = new List<PedidoDetalleDTO>();
    }

    public class PedidoDetalleDTO
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}

