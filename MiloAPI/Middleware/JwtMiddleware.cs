using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Milo.Application.Models.DTOs;
using Milo.Application.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace MiloAPI.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = ExtractTokenFromHeader(context);
            
            // Log para debugging
            Console.WriteLine($"🔍 JWT Middleware - Path: {context.Request.Path}");
            Console.WriteLine($"🔍 JWT Middleware - Token found: {!string.IsNullOrEmpty(token)}");
            Console.WriteLine($"🔍 JWT Middleware - Requires Auth: {RequiresAuthentication(context)}");

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    await AttachUserToContext(context, token);
                    Console.WriteLine($"✅ JWT Middleware - Token validated successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ JWT Middleware - Token validation failed: {ex.Message}");
                    // Si el token es inválido, solo fallar si el endpoint requiere autenticación
                    if (RequiresAuthentication(context))
                    {
                        await SetUnauthorizedResponse(context, $"Token validation failed: {ex.Message}");
                        return;
                    }
                    // Si no requiere autenticación, continuar sin el token
                }
            }
            else if (RequiresAuthentication(context))
            {
                Console.WriteLine($"❌ JWT Middleware - No token provided for protected endpoint");
                await SetUnauthorizedResponse(context, "No token provided");
                return;
            }

            await _next(context);
        }

        private string? ExtractTokenFromHeader(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }

            return null;
        }

        private async Task AttachUserToContext(HttpContext context, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSettings = _configuration.GetSection("Jwt").Get<JwtSettings>();
            
            if (jwtSettings == null)
            {
                throw new InvalidOperationException("JWT configuration not found");
            }

            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);
            
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            
            // Extraer claims del token
            var userId = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
            var userEmail = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value;
            var userRole = jwtToken.Claims.First(x => x.Type == ClaimTypes.Role).Value;

            // Crear identity y agregar al contexto
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("userId", userId),
                new Claim("email", userEmail),
                new Claim("role", userRole),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, userEmail),
                new Claim(ClaimTypes.Role, userRole)
            });

            context.User = new ClaimsPrincipal(identity);
        }

        private bool RequiresAuthentication(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>() != null)
            {
                return true;
            }
            return false;
        }

        private async Task SetUnauthorizedResponse(HttpContext context, string message)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var response = new ApiRequestResponse
            {
                Code = 401,
                Message = "Unauthorized",
                Type = "Authentication",
                StackTrace = message
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
