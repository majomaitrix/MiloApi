using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Milo.Application.Models;
using Milo.Application.Models.DTOs;
using Milo.Application.Usuarios;
using Milo.Application.Usuarios.DTOs;
using Milo.Domain.Entities;
using Milo.Infrastructure.Persistence;
using Moq;
using Xunit;
using System;

namespace MiloAPI.Tests.Handlers
{
    public class CreateUserQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CreateUserQueryHandler _handler;

        public CreateUserQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mediatorMock = new Mock<IMediator>();

            _handler = new CreateUserQueryHandler(_mediatorMock.Object, _context);
        }

        [Fact]
        public async Task Handle_UsuarioValido_DeberiaCrearUsuarioExitosamente()
        {
            // Arrange
            var rol = new Rol { Id = 1, Nombre = "Admin" };
            _context.roles.Add(rol);
            await _context.SaveChangesAsync();

            var request = new CreateUserQuery(new CreateUserIn
            {
                Nombre = "Test User",
                Email = "test@example.com",
                Contraseña = "Password123",
                Rol = 1,
                Direccion = "Test Address",
                Telefono = "123456789"
            });

            // Act
            var resultado = await _handler.Handle(request, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.code.Should().Be(Constants.CodeSuccess);
            resultado.message.Should().Be(Constants.MessageSuccess);

            // Verificar que se guardó en la BD
            var usuarioGuardado = await _context.usuarios.FirstOrDefaultAsync(u => u.Email == "test@example.com");
            usuarioGuardado.Should().NotBeNull();
            usuarioGuardado!.Nombre.Should().Be("Test User");
        }

        [Fact]
        public async Task Handle_EmailDuplicado_DeberiaRetornarError()
        {
            // Arrange
            var rol = new Rol { Id = 1, Nombre = "Admin" };
            _context.roles.Add(rol);
            
            var usuarioExistente = new Usuario
            {
                Id = 1,
                Nombre = "Existing User",
                Email = "test@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Password123"),
                RolId = 1
            };
            _context.usuarios.Add(usuarioExistente);
            await _context.SaveChangesAsync();

            var request = new CreateUserQuery(new CreateUserIn
            {
                Nombre = "New User",
                Email = "test@example.com", // Email duplicado
                Contraseña = "Password123",
                Rol = 1
            });

            // Act
            var resultado = await _handler.Handle(request, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.code.Should().Be(Constants.CodeCatch);
            resultado.message_error.Should().Contain("duplicado");
        }

        [Fact]
        public async Task Handle_RolNoExiste_DeberiaRetornarError()
        {
            // Arrange
            var request = new CreateUserQuery(new CreateUserIn
            {
                Nombre = "Test User",
                Email = "test@example.com",
                Contraseña = "Password123",
                Rol = 999 // Rol que no existe
            });

            // Act
            var resultado = await _handler.Handle(request, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.code.Should().Be(Constants.CodeCatch);
            resultado.message_error.Should().Contain("rol");
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
