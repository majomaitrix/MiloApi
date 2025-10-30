using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Domain.Entities
{
    public class Reservacion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [ForeignKey("Usuario")]
        [Column("usuario_id")]
        public int? UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        [Required]
        [Column("fecha_reserva")]
        public DateTime FechaReserva { get; set; }

        [Required]
        [Column("cantidad_personas")]
        public int CantidadPersonas { get; set; }

        [Required]
        [Column("estado")]
        public string Estado { get; set; } = "pendiente";
    }
}
