using MediatR;
using Microsoft.EntityFrameworkCore;
using Milo.Application.Models.DTOs;
using Milo.Application.Parameter.DTOs;
using Milo.Infrastructure.Persistence;

namespace Milo.Application.Parameter
{
    public sealed class GetProductosQueryHandler : IRequestHandler<GetProductosQuery, PagedResponse<ProductoDTO>>
    {
        private readonly ApplicationDbContext _context;

        public GetProductosQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<ProductoDTO>> Handle(GetProductosQuery req, CancellationToken cancellationToken)
        {
            try
            {
                // Query base con includes
                var query = _context.productos
                    .Include(p => p.Categoria)
                    .AsQueryable();

                // Filtros
                if (!string.IsNullOrEmpty(req.PagedRequest.SearchTerm))
                {
                    query = query.Where(p => 
                        p.Nombre.Contains(req.PagedRequest.SearchTerm) ||
                        p.Descripcion.Contains(req.PagedRequest.SearchTerm));
                }

                // Ordenamiento
                if (!string.IsNullOrEmpty(req.PagedRequest.SortBy))
                {
                    switch (req.PagedRequest.SortBy.ToLower())
                    {
                        case "nombre":
                            query = req.PagedRequest.SortDescending 
                                ? query.OrderByDescending(p => p.Nombre)
                                : query.OrderBy(p => p.Nombre);
                            break;
                        case "precio":
                            query = req.PagedRequest.SortDescending 
                                ? query.OrderByDescending(p => p.Precio)
                                : query.OrderBy(p => p.Precio);
                            break;
                        case "stock":
                            query = req.PagedRequest.SortDescending 
                                ? query.OrderByDescending(p => p.Stock)
                                : query.OrderBy(p => p.Stock);
                            break;
                        case "categoria":
                            query = req.PagedRequest.SortDescending 
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

                // Contar total de elementos
                var totalItems = await query.CountAsync(cancellationToken);

                // Paginación
                var productos = await query
                    .Skip((req.PagedRequest.Page - 1) * req.PagedRequest.PageSize)
                    .Take(req.PagedRequest.PageSize)
                    .Select(p => new ProductoDTO
                    {
                        id = p.Id,
                        nombre = p.Nombre,
                        descripcion = p.Descripcion,
                        precio = p.Precio,
                        stock = p.Stock,
                        categoriaId = (int)p.CategoriaId,
                        imagen = p.Imagen, // Base64 de la imagen
                        activo = p.Activo,
                        categoria = p.Categoria != null ? new CategoriaDTO
                        {
                            id = p.Categoria.Id,
                            nombre = p.Categoria.Nombre
                        } : null
                    })
                    .ToListAsync(cancellationToken);

                // Calcular metadatos de paginación
                var totalPages = (int)Math.Ceiling((double)totalItems / req.PagedRequest.PageSize);

                return new PagedResponse<ProductoDTO>
                {
                    Data = productos,
                    Pagination = new PaginationMetadata
                    {
                        Page = req.PagedRequest.Page,
                        PageSize = req.PagedRequest.PageSize,
                        TotalItems = totalItems,
                        TotalPages = totalPages,
                        HasNext = req.PagedRequest.Page < totalPages,
                        HasPrevious = req.PagedRequest.Page > 1
                    }
                };
            }
            catch (Exception ex)
            {
                // En caso de error, retornar respuesta vacía
                return new PagedResponse<ProductoDTO>
                {
                    Data = new List<ProductoDTO>(),
                    Pagination = new PaginationMetadata
                    {
                        Page = req.PagedRequest.Page,
                        PageSize = req.PagedRequest.PageSize,
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


