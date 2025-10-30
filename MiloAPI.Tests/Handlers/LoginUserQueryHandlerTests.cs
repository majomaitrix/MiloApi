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

namespace MiloAPI.Tests.Handlers
{
    public class LoginUserQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AppConfiguration _appConfiguration;
        private readonly LoginUserQueryHandler _handler;

        public LoginUserQueryHandlerTests()
        {
            // Configurar base de datos en memoria para tests
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mediatorMock = new Mock<IMediator>();
            
            // Configurar AppConfiguration para tests
            _appConfiguration = new AppConfiguration
            {
                Jwt = new JwtSettings
                {
                    Secret = "test-secret-key-for-testing-purposes-only",
                    LifespanMinutes = 60,
                    Issuer = "test-issuer",
                    Audience = "test-audience"
                }
            };

            _handler = new LoginUserQueryHandler(_mediatorMock.Object, _context, _appConfiguration);
        }

        [Fact]
        public async Task Handle_UsuarioValido_DeberiaRetornarLoginExitoso()
        {
            // Arrange (Preparar)
            var rol = new Rol { Id = 1, Nombre = "Administrador" };
            var usuario = new Usuario
            {
                Id = 1,
                Email = "admin@milo.com",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                Nombre = "Admin",
                Rol = rol,
                RolId = rol.Id
            };

            await _context.roles.AddAsync(rol);
            await _context.usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();

            var loginRequest = new LoginUserIn
            {
                Email = "admin@milo.com",
                Password = "123456"
            };

            var query = new LoginUserQuery(loginRequest);

            // Act (Actuar)
            var resultado = await _handler.Handle(query, CancellationToken.None);

            // Assert (Verificar)
            resultado.Should().NotBeNull();
            resultado.code.Should().Be(Constants.CodeSuccess);
            resultado.message.Should().Be(Constants.MessageSuccess);
            resultado.Data.Should().NotBeNull();
            resultado.Data.email.Should().Be("admin@milo.com");
            resultado.Data.nombre.Should().Be("Admin");
            resultado.Data.token.Should().NotBeNull();
            resultado.Data.Rol.Should().NotBeNull();
            resultado.Data.Rol.Nombre.Should().Be("Administrador");
        }

        [Fact]
        public async Task Handle_UsuarioNoEncontrado_DeberiaRetornarError()
        {
            // Arrange (Preparar)
            var loginRequest = new LoginUserIn
            {
                Email = "noexiste@milo.com",
                Password = "123456"
            };

            var query = new LoginUserQuery(loginRequest);

            // Act (Actuar)
            var resultado = await _handler.Handle(query, CancellationToken.None);

            // Assert (Verificar)
            resultado.Should().NotBeNull();
            resultado.code.Should().Be(Constants.CodeCatch);
            resultado.message.Should().Be(Constants.MessageNotFound);
            resultado.message_error.Should().Be(Constants.MessageNotFoundErr);
            resultado.Data.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ContraseñaIncorrecta_DeberiaRetornarError()
        {
            // Arrange (Preparar)
            var rol = new Rol { Id = 1, Nombre = "Administrador" };
            var usuario = new Usuario
            {
                Id = 1,
                Email = "admin@milo.com",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                Nombre = "Admin",
                Rol = rol,
                RolId = rol.Id
            };

            await _context.roles.AddAsync(rol);
            await _context.usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();

            var loginRequest = new LoginUserIn
            {
                Email = "admin@milo.com",
                Password = "contraseña-incorrecta"
            };

            var query = new LoginUserQuery(loginRequest);

            // Act (Actuar)
            var resultado = await _handler.Handle(query, CancellationToken.None);

            // Assert (Verificar)
            resultado.Should().NotBeNull();
            resultado.code.Should().Be(Constants.CodeCatch);
            resultado.message.Should().Be(Constants.MessageNotFound);
            resultado.message_error.Should().Be("Credenciales inválidas");
            resultado.Data.Should().BeNull();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}