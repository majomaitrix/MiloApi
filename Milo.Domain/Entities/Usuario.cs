using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Domain.Entities
{
    public class Usuario
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("nombre")]
        public string Nombre { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("password_hash")]
        public string Password { get; set; } // En la DB es password_hash
        [ForeignKey("Rol")]
        [Column("rol_id")]
        public int? RolId { get; set; }
        public Rol Rol { get; set; }
        [Column("creado_en")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        // Nuevas columnas
        [Column("direccion")]
        public string? Direccion { get; set; }
        [Column("telefono")]
        public string? Telefono { get; set; }
    }
}
