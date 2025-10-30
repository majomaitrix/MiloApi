using MediatR;
using Microsoft.EntityFrameworkCore;
using Milo.Application.Models;
using Milo.Application.Models.DTOs;
using Milo.Application.Parameter.DTOs;
using Milo.Application.Usuarios;
using Milo.Application.Usuarios.DTOs;
using Milo.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Application.Parameter
{
    public sealed class GetCategoriasQueryHandler : IRequestHandler<GetCategoriasQuery, PagedResponse<CategoriaDTO>>
    {
        private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;

        public GetCategoriasQueryHandler(IMediator mediator, ApplicationDbContext context)
        {
            this._mediator = mediator;
            this._context = context;
        }

        public async Task<PagedResponse<CategoriaDTO>> Handle(GetCategoriasQuery req, CancellationToken cancellationToken)
        {
            try
            {
                // Obtener todas las categorías
                var query = _context.categorias.AsQueryable();

                // Aplicar filtro de búsqueda si existe
                if (!string.IsNullOrEmpty(req.PagedRequest.SearchTerm))
                {
                    query = query.Where(c => c.Nombre.Contains(req.PagedRequest.SearchTerm));
                }

                // Aplicar ordenamiento
                if (!string.IsNullOrEmpty(req.PagedRequest.SortBy))
                {
                    switch (req.PagedRequest.SortBy.ToLower())
                    {
                        case "nombre":
                            query = req.PagedRequest.SortDescending 
                                ? query.OrderByDescending(c => c.Nombre)
                                : query.OrderBy(c => c.Nombre);
                            break;
                        case "id":
                            query = req.PagedRequest.SortDescending 
                                ? query.OrderByDescending(c => c.Id)
                                : query.OrderBy(c => c.Id);
                            break;
                        default:
                            query = query.OrderBy(c => c.Nombre);
                            break;
                    }
                }
                else
                {
                    query = query.OrderBy(c => c.Nombre);
                }

                // Contar total de elementos
                var totalItems = await query.CountAsync(cancellationToken);

                // Aplicar paginación
                var categorias = await query
                    .Skip((req.PagedRequest.Page - 1) * req.PagedRequest.PageSize)
                    .Take(req.PagedRequest.PageSize)
                    .ToListAsync(cancellationToken);

                // Mapear a DTOs
                var categoriaDTOs = categorias.Select(c => new CategoriaDTO
                {
                    id = c.Id,
                    nombre = c.Nombre
                }).ToList();

                // Calcular metadatos de paginación
                var totalPages = (int)Math.Ceiling((double)totalItems / req.PagedRequest.PageSize);

                return new PagedResponse<CategoriaDTO>
                {
                    Data = categoriaDTOs,
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
                return new PagedResponse<CategoriaDTO>
                {
                    Data = new List<CategoriaDTO>(),
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
