using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Domain.Entities
{
    public class PedidoDetalle
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [ForeignKey("Pedido")]
        [Column("pedido_id")]
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }
        [ForeignKey("Producto")]
        [Column("producto_id")]
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        [Required]
        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Required]
        [Column("subtotal")]
        public decimal Subtotal { get; set; }
    }
}
