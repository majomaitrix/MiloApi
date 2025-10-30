using System.ComponentModel.DataAnnotations;

namespace Milo.Application.Models.DTOs
{
    public class PagedRequestWithFilters : PagedRequest
    {
        // Filtros por rango de precios
        public decimal? PrecioMinimo { get; set; }
        public decimal? PrecioMaximo { get; set; }

        // Filtros por stock
        public int? StockMinimo { get; set; }
        public int? StockMaximo { get; set; }

        // Filtro por categoría
        public int? CategoriaId { get; set; }

        // Filtro por stock bajo (productos con stock menor a X)
        public bool? SoloStockBajo { get; set; }
        public int? LimiteStockBajo { get; set; } = 10;

        // Filtro por disponibilidad
        public bool? SoloDisponibles { get; set; } = true;
    }
}


