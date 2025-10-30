using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Domain.Entities
{
    public class VentaDetalle
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [ForeignKey("Venta")]
        [Column("venta_id")]
        public int VentaId { get; set; }
        public Venta Venta { get; set; }

        [ForeignKey("Producto")]
        [Column("producto_id")]
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        [Required]
        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Required]
        [Column("precio_unitario")]
        public decimal PrecioUnitario { get; set; }
    }
}
