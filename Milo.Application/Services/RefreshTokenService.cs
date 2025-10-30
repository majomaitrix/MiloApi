using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Milo.Application.Models.DTOs;
using Milo.Domain.Entities;
using Milo.Infrastructure.Persistence;
using System.Security.Cryptography;
using System.Text;

namespace Milo.Application.Services
{
    public interface IRefreshTokenService
    {
        Task<string> GenerateRefreshTokenAsync(int userId);
        Task<bool> ValidateRefreshTokenAsync(string refreshToken, int userId);
        Task RevokeRefreshTokenAsync(string refreshToken);
        Task RevokeAllUserTokensAsync(int userId);
    }

    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public RefreshTokenService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> GenerateRefreshTokenAsync(int userId)
        {
            // Generar token aleatorio seguro
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            var refreshToken = Convert.ToBase64String(randomBytes);
            var expiresAt = DateTime.UtcNow.AddDays(30); // Refresh token válido por 30 días

            // Guardar en base de datos
            var tokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = userId,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            _context.refreshTokens.Add(tokenEntity);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<bool> ValidateRefreshTokenAsync(string refreshToken, int userId)
        {
            var tokenEntity = await _context.refreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

            if (tokenEntity == null)
                return false;

            if (tokenEntity.IsRevoked)
                return false;

            if (tokenEntity.ExpiresAt < DateTime.UtcNow)
            {
                // Token expirado, marcarlo como revocado
                tokenEntity.IsRevoked = true;
                await _context.SaveChangesAsync();
                return false;
            }

            return true;
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var tokenEntity = await _context.refreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (tokenEntity != null)
            {
                tokenEntity.IsRevoked = true;
                tokenEntity.RevokedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RevokeAllUserTokensAsync(int userId)
        {
            var userTokens = await _context.refreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in userTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}
