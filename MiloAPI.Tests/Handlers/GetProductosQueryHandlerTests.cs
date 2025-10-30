using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Milo.Application.Models.DTOs;
using Milo.Application.Parameter;
using Milo.Application.Parameter.DTOs;
using Milo.Domain.Entities;
using Milo.Infrastructure.Persistence;
using Xunit;
using System;

namespace MiloAPI.Tests.Handlers
{
    public class GetProductosQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetProductosQueryHandler _handler;

        public GetProductosQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _handler = new GetProductosQueryHandler(_context);
        }

        [Fact]
        public async Task Handle_ProductosExistentes_DeberiaRetornarListaPaginada()
        {
            // Arrange
            var categoria = new Categoria { Id = 1, Nombre = "Bebidas" };
            _context.categorias.Add(categoria);

            var productos = new List<Producto>
            {
                new Producto { Id = 1, Nombre = "Coca Cola", Precio = 2.50m, Stock = 100, CategoriaId = 1, Categoria = categoria },
                new Producto { Id = 2, Nombre = "Pepsi", Precio = 2.30m, Stock = 80, CategoriaId = 1, Categoria = categoria },
                new Producto { Id = 3, Nombre = "Sprite", Precio = 2.40m, Stock = 60, CategoriaId = 1, Categoria = categoria }
            };
            _context.productos.AddRange(productos);
            await _context.SaveChangesAsync();

            var pagedRequest = new PagedRequest
            {
                Page = 1,
                PageSize = 2,
                SearchTerm = null,
                SortBy = "nombre",
                SortDescending = false
            };

            var request = new GetProductosQuery(pagedRequest);

            // Act
            var resultado = await _handler.Handle(request, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Data.Should().HaveCount(2);
            resultado.Pagination.TotalItems.Should().Be(3);
            resultado.Pagination.Page.Should().Be(1);
            resultado.Pagination.PageSize.Should().Be(2);
            resultado.Pagination.HasNext.Should().BeTrue();
            resultado.Pagination.HasPrevious.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_BuscarPorNombre_DeberiaFiltrarCorrectamente()
        {
            // Arrange
            var categoria = new Categoria { Id = 1, Nombre = "Bebidas" };
            _context.categorias.Add(categoria);

            var productos = new List<Producto>
            {
                new Producto { Id = 1, Nombre = "Coca Cola", Precio = 2.50m, Stock = 100, CategoriaId = 1, Categoria = categoria },
                new Producto { Id = 2, Nombre = "Pepsi", Precio = 2.30m, Stock = 80, CategoriaId = 1, Categoria = categoria },
                new Producto { Id = 3, Nombre = "Sprite", Precio = 2.40m, Stock = 60, CategoriaId = 1, Categoria = categoria }
            };
            _context.productos.AddRange(productos);
            await _context.SaveChangesAsync();

            var pagedRequest = new PagedRequest
            {
                Page = 1,
                PageSize = 10,
                SearchTerm = "Coca",
                SortBy = null,
                SortDescending = false
            };

            var request = new GetProductosQuery(pagedRequest);

            // Act
            var resultado = await _handler.Handle(request, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Data.Should().HaveCount(1);
            resultado.Data.First().nombre.Should().Be("Coca Cola");
            resultado.Pagination.TotalItems.Should().Be(1);
        }

        [Fact]
        public async Task Handle_OrdenarPorPrecioDescendente_DeberiaOrdenarCorrectamente()
        {
            // Arrange
            var categoria = new Categoria { Id = 1, Nombre = "Bebidas" };
            _context.categorias.Add(categoria);

            var productos = new List<Producto>
            {
                new Producto { Id = 1, Nombre = "Coca Cola", Precio = 2.50m, Stock = 100, CategoriaId = 1, Categoria = categoria },
                new Producto { Id = 2, Nombre = "Pepsi", Precio = 2.30m, Stock = 80, CategoriaId = 1, Categoria = categoria },
                new Producto { Id = 3, Nombre = "Sprite", Precio = 2.40m, Stock = 60, CategoriaId = 1, Categoria = categoria }
            };
            _context.productos.AddRange(productos);
            await _context.SaveChangesAsync();

            var pagedRequest = new PagedRequest
            {
                Page = 1,
                PageSize = 10,
                SearchTerm = null,
                SortBy = "precio",
                SortDescending = true
            };

            var request = new GetProductosQuery(pagedRequest);

            // Act
            var resultado = await _handler.Handle(request, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Data.Should().HaveCount(3);
            resultado.Data.First().nombre.Should().Be("Coca Cola"); // El más caro
            resultado.Data.Last().nombre.Should().Be("Pepsi"); // El más barato
        }

        [Fact]
        public async Task Handle_SinProductos_DeberiaRetornarListaVacia()
        {
            // Arrange
            var pagedRequest = new PagedRequest
            {
                Page = 1,
                PageSize = 10,
                SearchTerm = null,
                SortBy = null,
                SortDescending = false
            };

            var request = new GetProductosQuery(pagedRequest);

            // Act
            var resultado = await _handler.Handle(request, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Data.Should().BeEmpty();
            resultado.Pagination.TotalItems.Should().Be(0);
            resultado.Pagination.TotalPages.Should().Be(0);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}


