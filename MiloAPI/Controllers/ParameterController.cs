using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Milo.Application.Models;
using Milo.Application.Models.DTOs;
using Milo.Application.Parameter;
using Milo.Application.Parameter.DTOs;
using Milo.Application.Usuarios;
using Milo.Application.Usuarios.DTOs;
using Milo.Domain.Entities;

namespace MiloAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParameterController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ParameterController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gestiona categorías (crear, editar, eliminar)
        /// </summary>
        /// <param name="request">Datos de la categoría</param>
        /// <param name="opcion">Operación a realizar: "crear", "editar" o "eliminar"</param>
        /// <returns>Respuesta indicando el resultado de la operación</returns>
        /// <response code="200">Operación ejecutada correctamente</response>
        /// <response code="400">Error en los datos proporcionados</response>
        /// <response code="401">No autorizado - Token JWT requerido</response>
        /// <response code="403">Prohibido - Se requiere rol de Administrador</response>
        [HttpPost("admin-categorias")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> AdminRoles([FromBody] CategoriaDTO request, [FromQuery] string opcion)
        {
            ApiRequestResponse response = new ApiRequestResponse();
            var data = await _mediator.Send(new AdminCategoriaQuery(opcion, request));
            if (data.code.Equals(Constants.CodeSuccess))
            {
                response = new ApiRequestResponse()
                {
                    Code = 200,
                    Message = Constants.MessageSuccess,
                    Type = "Administracion de Categorias."
                };
                return Ok(response);
            }
            else
            {
                response = new ApiRequestResponse()
                {
                    Code = 400,
                    Message = Constants.MessageError,
                    Type = "Administracion de Categorias.",
                    StackTrace = data.message_error
                };
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Obtiene una lista paginada de categorías
        /// </summary>
        /// <param name="page">Número de página (default: 1, mínimo: 1)</param>
        /// <param name="pageSize">Tamaño de página (default: 10, máximo: 100)</param>
        /// <param name="searchTerm">Término de búsqueda para filtrar por nombre</param>
        /// <param name="sortBy">Campo por el cual ordenar los resultados</param>
        /// <param name="sortDescending">Indica si el ordenamiento es descendente (default: false)</param>
        /// <returns>Lista paginada de categorías</returns>
        /// <response code="200">Lista de categorías obtenida correctamente</response>
        [HttpGet("get-categorias")]
        public async Task<IActionResult> GetCategorias(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDescending = false)
        {
            // Validar parámetros
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var pagedRequest = new PagedRequest
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                SortDescending = sortDescending
            };

            var data = await _mediator.Send(new GetCategoriasQuery(pagedRequest));
            return Ok(data);
        }

        /// <summary>
        /// Gestiona pedidos (crear, editar, eliminar)
        /// </summary>
        /// <param name="request">Datos del pedido</param>
        /// <param name="opcion">Operación a realizar: "crear", "editar" o "eliminar"</param>
        /// <returns>Respuesta indicando el resultado de la operación</returns>
        /// <response code="200">Operación ejecutada correctamente</response>
        /// <response code="400">Error en los datos proporcionados</response>
        [HttpPost("admin-pedidos")]
        public async Task<IActionResult> AdminPedidos([FromBody] PedidoDTO request, [FromQuery] string opcion)
        {
            ApiRequestResponse response = new ApiRequestResponse();
            var data = await _mediator.Send(new AdminPedidosQuery(request, opcion));
            if (data.code.Equals(Constants.CodeSuccess))
            {
                response = new ApiRequestResponse()
                {
                    Code = 200,
                    Message = Constants.MessageSuccess,
                    Type = "Administracion de Pedidos."
                };
                return Ok(response);
            }
            else
            {
                response = new ApiRequestResponse()
                {
                    Code = 400,
                    Message = Constants.MessageError,
                    Type = "Administracion de Pedidos.",
                    StackTrace = data.message_error
                };
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Obtiene una lista paginada de productos
        /// </summary>
        /// <param name="page">Número de página (default: 1, mínimo: 1)</param>
        /// <param name="pageSize">Tamaño de página (default: 10, máximo: 100)</param>
        /// <param name="searchTerm">Término de búsqueda para filtrar por nombre o descripción</param>
        /// <param name="sortBy">Campo por el cual ordenar: "nombre", "precio", "stock" o "categoria"</param>
        /// <param name="sortDescending">Indica si el ordenamiento es descendente (default: false)</param>
        /// <returns>Lista paginada de productos con imágenes en Base64</returns>
        /// <response code="200">Lista de productos obtenida correctamente</response>
        [HttpGet("get-productos")]
        public async Task<IActionResult> GetProductos(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDescending = false)
        {
            // Validar parámetros
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var pagedRequest = new PagedRequest
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                SortDescending = sortDescending
            };

            var data = await _mediator.Send(new GetProductosQuery(pagedRequest));
            return Ok(data);
        }

        /// <summary>
        /// Gestiona productos (crear, editar, eliminar)
        /// </summary>
        /// <param name="request">Datos del producto incluyendo imagen en Base64</param>
        /// <param name="opcion">Operación a realizar: "crear", "editar" o "eliminar"</param>
        /// <returns>Respuesta indicando el resultado de la operación</returns>
        /// <response code="200">Operación ejecutada correctamente</response>
        /// <response code="400">Error en los datos proporcionados</response>
        /// <response code="401">No autorizado - Token JWT requerido</response>
        /// <response code="403">Prohibido - Se requiere rol de Administrador</response>
        [HttpPost("admin-productos")]
        public async Task<IActionResult> AdminProductos([FromBody] ProductoDTO request, [FromQuery] string opcion)
        {
            ApiRequestResponse response = new ApiRequestResponse();
            var data = await _mediator.Send(new AdminProductosQuery(opcion, request));
            if (data.code.Equals(Constants.CodeSuccess))
            {
                response = new ApiRequestResponse()
                {
                    Code = 200,
                    Message = Constants.MessageSuccess,
                    Type = "Administracion de Productos."
                };
                return Ok(response);
            }
            else
            {
                response = new ApiRequestResponse()
                {
                    Code = 400,
                    Message = Constants.MessageError,
                    Type = "Administracion de Productos.",
                    StackTrace = data.message_error
                };
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Obtiene la lista de estados disponibles para pedidos
        /// </summary>
        /// <returns>Lista de estados con sus descripciones</returns>
        /// <response code="200">Lista de estados obtenida correctamente</response>
        [HttpGet("get-estados-pedido")]
        public async Task<IActionResult> GetEstadosPedido()
        {
            var data = await _mediator.Send(new GetEstadosPedidoQuery());
            return Ok(data);
        }

        /// <summary>
        /// Cambia el estado de un pedido validando que la transición sea válida
        /// </summary>
        /// <param name="request">Datos con el ID del pedido y el nuevo estado</param>
        /// <returns>Respuesta indicando el resultado de la operación</returns>
        /// <remarks>
        /// Transiciones válidas:
        /// - Pendiente (1) puede cambiar a: Preparando (2) o Cancelado (5)
        /// - Preparando (2) puede cambiar a: Listo (3) o Cancelado (5)
        /// - Listo (3) puede cambiar a: Entregado (4)
        /// - Entregado (4) y Cancelado (5) son estados finales
        /// </remarks>
        /// <response code="200">Estado cambiado correctamente</response>
        /// <response code="400">Transición inválida o datos incorrectos</response>
        /// <response code="401">No autorizado - Token JWT requerido</response>
        /// <response code="403">Prohibido - Se requiere rol de Administrador o Mesero</response>
        [HttpPost("cambiar-estado-pedido")]
        [Authorize(Roles = "Administrador,Mesero")]
        public async Task<IActionResult> CambiarEstadoPedido([FromBody] CambiarEstadoPedidoIn request)
        {
            // Validar el modelo
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var validationResponse = new ApiRequestResponse()
                {
                    Code = 400,
                    Message = "Error de validación",
                    Type = "Cambio de Estado de Pedido",
                    StackTrace = string.Join("; ", errors)
                };
                return BadRequest(validationResponse);
            }

            var data = await _mediator.Send(new CambiarEstadoPedidoQuery(request));
            return Ok(data);
        }

        /// <summary>
        /// Obtiene una lista paginada de productos con filtros avanzados
        /// </summary>
        /// <param name="page">Número de página (default: 1, mínimo: 1)</param>
        /// <param name="pageSize">Tamaño de página (default: 10, máximo: 100)</param>
        /// <param name="searchTerm">Término de búsqueda para filtrar por nombre o descripción</param>
        /// <param name="sortBy">Campo por el cual ordenar los resultados</param>
        /// <param name="sortDescending">Indica si el ordenamiento es descendente (default: false)</param>
        /// <param name="precioMinimo">Precio mínimo para filtrar productos</param>
        /// <param name="precioMaximo">Precio máximo para filtrar productos</param>
        /// <param name="stockMinimo">Stock mínimo para filtrar productos</param>
        /// <param name="stockMaximo">Stock máximo para filtrar productos</param>
        /// <param name="categoriaId">ID de la categoría para filtrar productos</param>
        /// <param name="soloStockBajo">Indica si solo se deben mostrar productos con stock bajo</param>
        /// <param name="limiteStockBajo">Límite para considerar un producto con stock bajo</param>
        /// <param name="soloDisponibles">Indica si solo se deben mostrar productos activos</param>
        /// <returns>Lista paginada de productos filtrados</returns>
        /// <response code="200">Lista de productos obtenida correctamente</response>
        [HttpGet("get-productos-filtros")]
        public async Task<IActionResult> GetProductosConFiltros(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDescending = false,
            [FromQuery] decimal? precioMinimo = null,
            [FromQuery] decimal? precioMaximo = null,
            [FromQuery] int? stockMinimo = null,
            [FromQuery] int? stockMaximo = null,
            [FromQuery] int? categoriaId = null,
            [FromQuery] bool? soloStockBajo = null,
            [FromQuery] int? limiteStockBajo = null,
            [FromQuery] bool? soloDisponibles = null)
        {
            // Validar parámetros
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var pagedRequest = new PagedRequestWithFilters
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                SortDescending = sortDescending,
                PrecioMinimo = precioMinimo,
                PrecioMaximo = precioMaximo,
                StockMinimo = stockMinimo,
                StockMaximo = stockMaximo,
                CategoriaId = categoriaId,
                SoloStockBajo = soloStockBajo,
                LimiteStockBajo = limiteStockBajo,
                SoloDisponibles = soloDisponibles
            };

            var data = await _mediator.Send(new GetProductosConFiltrosQuery(pagedRequest));
            return Ok(data);
        }

        /// <summary>
        /// Obtiene una lista paginada de pedidos con filtros avanzados
        /// </summary>
        /// <param name="page">Número de página (default: 1, mínimo: 1)</param>
        /// <param name="pageSize">Tamaño de página (default: 10, máximo: 100)</param>
        /// <param name="searchTerm">Término de búsqueda para filtrar pedidos</param>
        /// <param name="sortBy">Campo por el cual ordenar los resultados</param>
        /// <param name="sortDescending">Indica si el ordenamiento es descendente (default: false)</param>
        /// <param name="estadoFiltro">Estado del pedido para filtrar: 1=Pendiente, 2=Preparando, 3=Listo, 4=Entregado, 5=Cancelado</param>
        /// <param name="fechaDesde">Fecha inicial del rango para filtrar pedidos</param>
        /// <param name="fechaHasta">Fecha final del rango para filtrar pedidos</param>
        /// <param name="usuarioId">ID del usuario para filtrar pedidos</param>
        /// <param name="totalMinimo">Total mínimo para filtrar pedidos</param>
        /// <param name="totalMaximo">Total máximo para filtrar pedidos</param>
        /// <returns>Lista paginada de pedidos con sus detalles</returns>
        /// <response code="200">Lista de pedidos obtenida correctamente</response>
        /// <response code="401">No autorizado - Token JWT requerido</response>
        /// <response code="403">Prohibido - Se requiere rol de Administrador o Mesero</response>
        [HttpGet("get-pedidos")]
        [Authorize(Roles = "Administrador,Mesero")]
        public async Task<IActionResult> GetPedidos(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDescending = false,
            [FromQuery] int? estadoFiltro = null,
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null,
            [FromQuery] int? usuarioId = null,
            [FromQuery] decimal? totalMinimo = null,
            [FromQuery] decimal? totalMaximo = null)
        {
            // Validar parámetros
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var pagedRequest = new PagedRequestWithFilters
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                SortDescending = sortDescending,
                PrecioMinimo = totalMinimo, // Reutilizamos PrecioMinimo para TotalMinimo
                PrecioMaximo = totalMaximo  // Reutilizamos PrecioMaximo para TotalMaximo
            };

            EstadoPedido? estado = null;
            if (estadoFiltro.HasValue && Enum.IsDefined(typeof(EstadoPedido), estadoFiltro.Value))
            {
                estado = (EstadoPedido)estadoFiltro.Value;
            }

            var data = await _mediator.Send(new GetPedidosQuery(pagedRequest, estado, fechaDesde, fechaHasta, usuarioId));
            return Ok(data);
        }
    }
}
