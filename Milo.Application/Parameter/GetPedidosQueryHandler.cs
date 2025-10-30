using MediatR;
using Microsoft.EntityFrameworkCore;
using Milo.Application.Models.DTOs;
using Milo.Application.Parameter.DTOs;
using Milo.Application.Parameter.Services;
using Milo.Domain.Entities;
using Milo.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Milo.Application.Parameter
{
    public sealed class GetPedidosQueryHandler : IRequestHandler<GetPedidosQuery, PagedResponse<PedidoDTO>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IEstadoPedidoService _estadoPedidoService;

        public GetPedidosQueryHandler(ApplicationDbContext context, IEstadoPedidoService estadoPedidoService)
        {
            _context = context;
            _estadoPedidoService = estadoPedidoService;
        }

        public async Task<PagedResponse<PedidoDTO>> Handle(GetPedidosQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Query base con includes
                var query = _context.pedidos
                    .Include(p => p.Usuario)
                    .AsQueryable();

                // Filtro por estado
                if (request.EstadoFiltro.HasValue)
                {
                    query = query.Where(p => p.Estado == request.EstadoFiltro.Value);
                }

                // Filtro por usuario
                if (request.UsuarioId.HasValue)
                {
                    query = query.Where(p => p.UsuarioId == request.UsuarioId.Value);
                }

                // Filtro por rango de fechas
                if (request.FechaDesde.HasValue)
                {
                    query = query.Where(p => p.Fecha >= request.FechaDesde.Value);
                }
                if (request.FechaHasta.HasValue)
                {
                    query = query.Where(p => p.Fecha <= request.FechaHasta.Value);
                }

                // Filtro por término de búsqueda (en observaciones)
                if (!string.IsNullOrEmpty(request.PagedRequest.SearchTerm))
                {
                    query = query.Where(p => 
                        (p.Observaciones != null && p.Observaciones.Contains(request.PagedRequest.SearchTerm)) ||
                        (p.Usuario != null && p.Usuario.Nombre.Contains(request.PagedRequest.SearchTerm)) ||
                        (p.Usuario != null && p.Usuario.Email.Contains(request.PagedRequest.SearchTerm)));
                }

                // Filtro por rango de total
                if (request.PagedRequest.PrecioMinimo.HasValue)
                {
                    query = query.Where(p => p.Total >= request.PagedRequest.PrecioMinimo.Value);
                }
                if (request.PagedRequest.PrecioMaximo.HasValue)
                {
                    query = query.Where(p => p.Total <= request.PagedRequest.PrecioMaximo.Value);
                }

                // Ordenamiento
                if (!string.IsNullOrEmpty(request.PagedRequest.SortBy))
                {
                    switch (request.PagedRequest.SortBy.ToLower())
                    {
                        case "fecha":
                            query = request.PagedRequest.SortDescending
                                ? query.OrderByDescending(p => p.Fecha)
                                : query.OrderBy(p => p.Fecha);
                            break;
                        case "total":
                            query = request.PagedRequest.SortDescending
                                ? query.OrderByDescending(p => p.Total)
                                : query.OrderBy(p => p.Total);
                            break;
                        case "estado":
                            query = request.PagedRequest.SortDescending
                                ? query.OrderByDescending(p => p.Estado)
                                : query.OrderBy(p => p.Estado);
                            break;
                        case "usuario":
                            query = request.PagedRequest.SortDescending
                                ? query.OrderByDescending(p => p.Usuario.Nombre)
                                : query.OrderBy(p => p.Usuario.Nombre);
                            break;
                        default:
                            query = query.OrderByDescending(p => p.Fecha); // Por defecto ordenar por fecha descendente
                            break;
                    }
                }
                else
                {
                    query = query.OrderByDescending(p => p.Fecha); // Por defecto ordenar por fecha descendente
                }

                var totalItems = await query.CountAsync(cancellationToken);
                var totalPages = (int)Math.Ceiling((double)totalItems / request.PagedRequest.PageSize);

                var pedidos = await query
                    .Skip((request.PagedRequest.Page - 1) * request.PagedRequest.PageSize)
                    .Take(request.PagedRequest.PageSize)
                    .Select(p => new PedidoDTO
                    {
                        Id = p.Id,
                        UsuarioId = p.UsuarioId ?? 0,
                        Fecha = p.Fecha,
                        Total = p.Total,
                        Estado = p.Estado,
                        Observaciones = p.Observaciones,
                        EstadoNombre = _estadoPedidoService.ObtenerDescripcionEstado(p.Estado),
                        UsuarioNombre = p.Usuario != null ? p.Usuario.Nombre : "Sin usuario",
                        UsuarioEmail = p.Usuario != null ? p.Usuario.Email : ""
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

                return new PagedResponse<PedidoDTO>
                {
                    Data = pedidos,
                    Pagination = paginationMetadata
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                return new PagedResponse<PedidoDTO>
                {
                    Data = new List<PedidoDTO>(),
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


