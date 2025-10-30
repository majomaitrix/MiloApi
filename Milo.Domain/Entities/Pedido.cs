using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Domain.Entities
{
    public class Pedido
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [ForeignKey("Usuario")]
        [Column("usuario_id")]
        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        [Required]
        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("total")]
        public decimal Total { get; set; }

        [Required]
        [Column("estado")]
        public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente;

        [Column("observaciones")]
        public string? Observaciones { get; set; }
    }
}
