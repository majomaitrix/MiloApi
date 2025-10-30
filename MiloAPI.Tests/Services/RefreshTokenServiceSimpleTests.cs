using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Milo.Application.Services;
using Milo.Domain.Entities;
using Milo.Infrastructure.Persistence;
using Xunit;

namespace MiloAPI.Tests.Services
{
    public class RefreshTokenServiceSimpleTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly RefreshTokenService _service;

        public RefreshTokenServiceSimpleTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new RefreshTokenService(_context, null!);

            // Seed data
            var usuario = new Usuario
            {
                Id = 1,
                Nombre = "Test User",
                Email = "test@example.com",
                Password = "hashed-password",
                RolId = 1
            };
            _context.usuarios.Add(usuario);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GenerateRefreshTokenAsync_UsuarioValido_DeberiaGenerarToken()
        {
            // Arrange
            var userId = 1;

            // Act
            var refreshToken = await _service.GenerateRefreshTokenAsync(userId);

            // Assert
            refreshToken.Should().NotBeNullOrEmpty();
            refreshToken.Should().HaveLength(88); // Base64 de 64 bytes = 88 caracteres

            var tokenEntity = await _context.refreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
            tokenEntity.Should().NotBeNull();
            tokenEntity!.UserId.Should().Be(userId);
            tokenEntity.IsRevoked.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateRefreshTokenAsync_TokenValido_DeberiaRetornarTrue()
        {
            // Arrange
            var userId = 1;
            var refreshToken = await _service.GenerateRefreshTokenAsync(userId);

            // Act
            var isValid = await _service.ValidateRefreshTokenAsync(refreshToken, userId);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateRefreshTokenAsync_TokenRevocado_DeberiaRetornarFalse()
        {
            // Arrange
            var userId = 1;
            var refreshToken = await _service.GenerateRefreshTokenAsync(userId);
            await _service.RevokeRefreshTokenAsync(refreshToken);

            // Act
            var isValid = await _service.ValidateRefreshTokenAsync(refreshToken, userId);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public async Task RevokeRefreshTokenAsync_TokenValido_DeberiaRevocarToken()
        {
            // Arrange
            var userId = 1;
            var refreshToken = await _service.GenerateRefreshTokenAsync(userId);

            // Act
            await _service.RevokeRefreshTokenAsync(refreshToken);

            // Assert
            var tokenEntity = await _context.refreshTokens.FirstAsync(rt => rt.Token == refreshToken);
            tokenEntity.IsRevoked.Should().BeTrue();
            tokenEntity.RevokedAt.Should().NotBeNull();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}


