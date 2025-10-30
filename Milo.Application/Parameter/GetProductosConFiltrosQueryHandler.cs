using MediatR;
using Microsoft.EntityFrameworkCore;
using Milo.Application.Models.DTOs;
using Milo.Application.Parameter.DTOs;
using Milo.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Milo.Application.Parameter
{
    public sealed class GetProductosConFiltrosQueryHandler : IRequestHandler<GetProductosConFiltrosQuery, PagedResponse<ProductoDTO>>
    {
        private readonly ApplicationDbContext _context;

        public GetProductosConFiltrosQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<ProductoDTO>> Handle(GetProductosConFiltrosQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Query base con includes
                var query = _context.productos
                    .Include(p => p.Categoria)
                    .AsQueryable();

                // Filtro por término de búsqueda
                if (!string.IsNullOrEmpty(request.PagedRequest.SearchTerm))
                {
                    query = query.Where(p =>
                        p.Nombre.Contains(request.PagedRequest.SearchTerm) ||
                        (p.Descripcion != null && p.Descripcion.Contains(request.PagedRequest.SearchTerm)));
                }

                // Filtro por rango de precios
                if (request.PagedRequest.PrecioMinimo.HasValue)
                {
                    query = query.Where(p => p.Precio >= request.PagedRequest.PrecioMinimo.Value);
                }
                if (request.PagedRequest.PrecioMaximo.HasValue)
                {
                    query = query.Where(p => p.Precio <= request.PagedRequest.PrecioMaximo.Value);
                }

                // Filtro por rango de stock
                if (request.PagedRequest.StockMinimo.HasValue)
                {
                    query = query.Where(p => p.Stock >= request.PagedRequest.StockMinimo.Value);
                }
                if (request.PagedRequest.StockMaximo.HasValue)
                {
                    query = query.Where(p => p.Stock <= request.PagedRequest.StockMaximo.Value);
                }

                // Filtro por categoría
                if (request.PagedRequest.CategoriaId.HasValue)
                {
                    query = query.Where(p => p.CategoriaId == request.PagedRequest.CategoriaId.Value);
                }

                // Filtro por stock bajo
                if (request.PagedRequest.SoloStockBajo == true)
                {
                    var limite = request.PagedRequest.LimiteStockBajo ?? 10;
                    query = query.Where(p => p.Stock <= limite);
                }

                // Filtro por disponibilidad (solo productos con stock > 0)
                if (request.PagedRequest.SoloDisponibles == true)
                {
                    query = query.Where(p => p.Stock > 0);
                }

                // Ordenamiento
                if (!string.IsNullOrEmpty(request.PagedRequest.SortBy))
                {
                    switch (request.PagedRequest.SortBy.ToLower())
                    {
                        case "nombre":
                            query = request.PagedRequest.SortDescending
                                ? query.OrderByDescending(p => p.Nombre)
                                : query.OrderBy(p => p.Nombre);
                            break;
                        case "precio":
                            query = request.PagedRequest.SortDescending
                                ? query.OrderByDescending(p => p.Precio)
                                : query.OrderBy(p => p.Precio);
                            break;
                        case "stock":
                            query = request.PagedRequest.SortDescending
                                ? query.OrderByDescending(p => p.Stock)
                                : query.OrderBy(p => p.Stock);
                            break;
                        case "categoria":
                            query = request.PagedRequest.SortDescending
                                ? query.OrderByDescending(p => p.Categoria.Nombre)
                                : query.OrderBy(p => p.Categoria.Nombre);
                            break;
                        default:
                            query = query.OrderBy(p => p.Id);
                            break;
                    }
                }
                else
                {
                    query = query.OrderBy(p => p.Id);
                }

                var totalItems = await query.CountAsync(cancellationToken);
                var totalPages = (int)Math.Ceiling((double)totalItems / request.PagedRequest.PageSize);

                var productos = await query
                    .Skip((request.PagedRequest.Page - 1) * request.PagedRequest.PageSize)
                    .Take(request.PagedRequest.PageSize)
                    .Select(p => new ProductoDTO
                    {
                        id = p.Id,
                        nombre = p.Nombre,
                        descripcion = p.Descripcion,
                        precio = p.Precio,
                        stock = p.Stock,
                        categoriaId = (int)p.CategoriaId,
                        imagen = p.Imagen,
                        activo = p.Activo,
                        categoria = p.Categoria != null ? new CategoriaDTO
                        {
                            id = p.Categoria.Id,
                            nombre = p.Categoria.Nombre
                        } : null
                    })
                    .ToListAsync(cancellationToken);

                var paginationMetadata = new PaginationMetadata
                {
                    Page = request.PagedRequest.Page,
                    PageSize = request.PagedRequest.PageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    HasNext = request.PagedRequest.Page < totalPages,
                    HasPrevious = request.PagedRequest.Page > 1
                };

                return new PagedResponse<ProductoDTO>
                {
                    Data = productos,
                    Pagination = paginationMetadata
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                return new PagedResponse<ProductoDTO>
                {
                    Data = new List<ProductoDTO>(),
                    Pagination = new PaginationMetadata
                    {
                        Page = request.PagedRequest.Page,
                        PageSize = request.PagedRequest.PageSize,
                        TotalItems = 0,
                        TotalPages = 0,
                        HasNext = false,
                        HasPrevious = false
                    }
                };
            }
        }
    }
}

