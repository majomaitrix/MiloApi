using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Domain.Entities
{
    public class Producto
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("nombre")]
        public string Nombre { get; set; }
        [Column("descripcion")]
        public string Descripcion { get; set; }
        [Required]
        [Column("precio")]
        public decimal Precio { get; set; }
        [Required]
        [Column("stock")]
        public int Stock { get; set; } = 0;
        [ForeignKey("Categoria")]
        [Column("categoria_id")]
        public int? CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
        
        [Column("imagen")]
        public string? Imagen { get; set; }
        
        [Column("activo")]
        public bool Activo { get; set; } = true;
    }
}
