using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Domain.Entities
{
    public class Mesa
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Required]
        [Column("numero")]
        public int Numero { get; set; }

        [Required]
        [Column("estado")]
        public string Estado { get; set; } = "libre";

        [ForeignKey("Pedido")]
        [Column("pedido_id")]
        public int? PedidoId { get; set; }
        public Pedido? Pedido { get; set; }

        [ForeignKey("Usuario")]
        [Column("mesero_id")]
        public int? MeseroId { get; set; }
        public Usuario? Mesero { get; set; }
    }
}
