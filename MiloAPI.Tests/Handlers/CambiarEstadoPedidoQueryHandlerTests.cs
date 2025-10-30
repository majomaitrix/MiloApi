using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Milo.Application.Models;
using Milo.Application.Parameter;
using Milo.Application.Parameter.DTOs;
using Milo.Application.Parameter.Services;
using Milo.Domain.Entities;
using Milo.Infrastructure.Persistence;
using Moq;
using Xunit;
using System;

namespace MiloAPI.Tests.Handlers
{
    public class CambiarEstadoPedidoQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IEstadoPedidoService> _estadoPedidoServiceMock;
        private readonly CambiarEstadoPedidoQueryHandler _handler;

        public CambiarEstadoPedidoQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _estadoPedidoServiceMock = new Mock<IEstadoPedidoService>();
            _handler = new CambiarEstadoPedidoQueryHandler(_context, _estadoPedidoServiceMock.Object);
        }

        [Fact]
        public async Task Handle_TransicionValida_DeberiaCambiarEstadoExitosamente()
        {
            // Arrange
            var pedido = new Pedido
            {
                Id = 1,
                UsuarioId = 1,
                Fecha = DateTime.UtcNow,
                Total = 25.50m,
                Estado = EstadoPedido.Pendiente
            };
            _context.pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            _estadoPedidoServiceMock
                .Setup(s => s.EsTransicionValida(EstadoPedido.Pendiente, EstadoPedido.Preparando))
                .Returns(true);

            _estadoPedidoServiceMock
                .Setup(s => s.ObtenerDescripcionEstado(EstadoPedido.Preparando))
                .Returns("En cocina preparando el pedido");

            var request = new CambiarEstadoPedidoQuery(new CambiarEstadoPedidoIn
            {
                PedidoId = 1,
                NuevoEstado = EstadoPedido.Preparando,
                Observaciones = "Pedido en preparación"
            });

            // Act
            var resultado = await _handler.Handle(request, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.code.Should().Be(Constants.CodeSuccess);
            resultado.message.Should().Be(Constants.MessageSuccess);
            resultado.Data.Should().NotBeNull();
            resultado.Data!.Estado.Should().Be(EstadoPedido.Preparando);
            resultado.Data.Observaciones.Should().Be("Pedido en preparación");
            resultado.Data.EstadoNombre.Should().Be("En cocina preparando el pedido");

            // Verificar que se guardó en la BD
            var pedidoActualizado = await _context.pedidos.FindAsync(1);
            pedidoActualizado!.Estado.Should().Be(EstadoPedido.Preparando);
        }

        [Fact]
        public async Task Handle_TransicionInvalida_DeberiaRetornarError()
        {
            // Arrange
            var pedido = new Pedido
            {
                Id = 1,
                UsuarioId = 1,
                Fecha = DateTime.UtcNow,
                Total = 25.50m,
                Estado = EstadoPedido.Pendiente
            };
            _context.pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            _estadoPedidoServiceMock
                .Setup(s => s.EsTransicionValida(EstadoPedido.Pendiente, EstadoPedido.Entregado))
                .Returns(false);

            var request = new CambiarEstadoPedidoQuery(new CambiarEstadoPedidoIn
            {
                PedidoId = 1,
                NuevoEstado = EstadoPedido.Entregado // Transición inválida
            });

            // Act
            var resultado = await _handler.Handle(request, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.code.Should().Be(Constants.CodeCatch);
            resultado.message.Should().Be("Transición de estado inválida");
            resultado.message_error.Should().Contain("No se puede cambiar");
            resultado.Data.Should().BeNull();

            // Verificar que NO se cambió en la BD
            var pedidoSinCambios = await _context.pedidos.FindAsync(1);
            pedidoSinCambios!.Estado.Should().Be(EstadoPedido.Pendiente);
        }

        [Fact]
        public async Task Handle_PedidoNoExiste_DeberiaRetornarError()
        {
            // Arrange
            _estadoPedidoServiceMock
                .Setup(s => s.EsTransicionValida(It.IsAny<EstadoPedido>(), It.IsAny<EstadoPedido>()))
                .Returns(true);

            var request = new CambiarEstadoPedidoQuery(new CambiarEstadoPedidoIn
            {
                PedidoId = 999, // Pedido que no existe
                NuevoEstado = EstadoPedido.Preparando
            });

            // Act
            var resultado = await _handler.Handle(request, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.code.Should().Be(Constants.CodeCatch);
            resultado.message.Should().Be(Constants.MessageNotFound);
            resultado.message_error.Should().Be("Pedido no encontrado");
            resultado.Data.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ActualizarObservaciones_DeberiaGuardarObservaciones()
        {
            // Arrange
            var pedido = new Pedido
            {
                Id = 1,
                UsuarioId = 1,
                Fecha = DateTime.UtcNow,
                Total = 25.50m,
                Estado = EstadoPedido.Pendiente,
                Observaciones = "Observación inicial"
            };
            _context.pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            _estadoPedidoServiceMock
                .Setup(s => s.EsTransicionValida(EstadoPedido.Pendiente, EstadoPedido.Preparando))
                .Returns(true);

            _estadoPedidoServiceMock
                .Setup(s => s.ObtenerDescripcionEstado(EstadoPedido.Preparando))
                .Returns("En cocina preparando el pedido");

            var request = new CambiarEstadoPedidoQuery(new CambiarEstadoPedidoIn
            {
                PedidoId = 1,
                NuevoEstado = EstadoPedido.Preparando,
                Observaciones = "Nueva observación"
            });

            // Act
            var resultado = await _handler.Handle(request, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.code.Should().Be(Constants.CodeSuccess);
            resultado.Data!.Observaciones.Should().Be("Nueva observación");

            // Verificar que se guardó en la BD
            var pedidoActualizado = await _context.pedidos.FindAsync(1);
            pedidoActualizado!.Observaciones.Should().Be("Nueva observación");
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}


