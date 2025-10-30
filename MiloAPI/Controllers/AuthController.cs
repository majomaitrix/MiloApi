using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Milo.Application.Models.DTOs;
using Milo.Application.Models;
using Milo.Application.Services;
using Milo.Application.Usuarios;
using Milo.Application.Usuarios.DTOs;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace MiloAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IMediator mediator, 
            IRefreshTokenService refreshTokenService,
            ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _refreshTokenService = refreshTokenService;
            _logger = logger;
        }

        /// <summary>
        /// Iniciar sesión de usuario
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                // Validar el modelo
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new ApiRequestResponse
                    {
                        Code = 400,
                        Message = "Error de validación",
                        Type = "Login",
                        StackTrace = string.Join("; ", errors)
                    });
                }

                _logger.LogInformation("Intento de login para usuario: {Email}", request.Email);

                var loginQuery = new LoginUserQuery(new LoginUserIn 
                { 
                    Email = request.Email, 
                    Password = request.Password 
                });

                var result = await _mediator.Send(loginQuery);

                if (result.code != Constants.CodeSuccess || result.Data == null)
                {
                    _logger.LogWarning("Login fallido para usuario: {Email}", request.Email);
                    return Unauthorized(new ApiRequestResponse
                    {
                        Code = 401,
                        Message = "Credenciales inválidas",
                        Type = "Login",
                        StackTrace = result.message_error
                    });
                }

                // Generar refresh token
                var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(result.Data.id);

                var response = new LoginResponse
                {
                    AccessToken = result.Data.token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60), // 1 hora
                    User = new UserInfo
                    {
                        Id = result.Data.id,
                        Name = result.Data.nombre,
                        Email = result.Data.email,
                        Role = result.Data.Rol.Nombre,
                        Address = result.Data.direccion,
                        Phone = result.Data.telefono
                    }
                };

                _logger.LogInformation("Login exitoso para usuario: {Email} con rol: {Role}", 
                    request.Email, result.Data.Rol.Nombre);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el login para usuario: {Email}", request.Email);
                return StatusCode(500, new ApiRequestResponse
                {
                    Code = 500,
                    Message = "Error interno del servidor",
                    Type = "Login",
                    StackTrace = ex.Message
                });
            }
        }

        /// <summary>
        /// Renovar token de acceso usando refresh token
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiRequestResponse
                    {
                        Code = 400,
                        Message = "Refresh token requerido",
                        Type = "Refresh Token"
                    });
                }

                // Aquí necesitaríamos obtener el userId del refresh token
                // Por simplicidad, vamos a implementar una lógica básica
                // En un sistema real, el refresh token debería contener el userId

                _logger.LogInformation("Intento de renovación de token");

                // TODO: Implementar lógica completa de refresh token
                return BadRequest(new ApiRequestResponse
                {
                    Code = 400,
                    Message = "Funcionalidad de refresh token en desarrollo",
                    Type = "Refresh Token"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la renovación de token");
                return StatusCode(500, new ApiRequestResponse
                {
                    Code = 500,
                    Message = "Error interno del servidor",
                    Type = "Refresh Token",
                    StackTrace = ex.Message
                });
            }
        }

        /// <summary>
        /// Cerrar sesión y revocar refresh token
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiRequestResponse
                    {
                        Code = 400,
                        Message = "Refresh token requerido",
                        Type = "Logout"
                    });
                }

                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken);

                _logger.LogInformation("Logout exitoso para usuario: {UserId}", userId);

                return Ok(new ApiRequestResponse
                {
                    Code = 200,
                    Message = "Sesión cerrada exitosamente",
                    Type = "Logout"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el logout");
                return StatusCode(500, new ApiRequestResponse
                {
                    Code = 500,
                    Message = "Error interno del servidor",
                    Type = "Logout",
                    StackTrace = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtener información del usuario autenticado
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var userEmail = GetCurrentUserEmail();
                var userRole = GetCurrentUserRole();

                var userInfo = new UserInfo
                {
                    Id = userId.Value,
                    Email = userEmail ?? "",
                    Role = userRole ?? "",
                    Name = User.FindFirst("name")?.Value ?? "",
                    Address = User.FindFirst("address")?.Value,
                    Phone = User.FindFirst("phone")?.Value
                };

                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener información del usuario");
                return StatusCode(500, new ApiRequestResponse
                {
                    Code = 500,
                    Message = "Error interno del servidor",
                    Type = "Get User Info",
                    StackTrace = ex.Message
                });
            }
        }

        /// <summary>
        /// Cambiar contraseña del usuario autenticado
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new ApiRequestResponse
                    {
                        Code = 400,
                        Message = "Error de validación",
                        Type = "Change Password",
                        StackTrace = string.Join("; ", errors)
                    });
                }

                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                // TODO: Implementar lógica de cambio de contraseña
                _logger.LogInformation("Cambio de contraseña solicitado para usuario: {UserId}", userId);

                return Ok(new ApiRequestResponse
                {
                    Code = 200,
                    Message = "Funcionalidad de cambio de contraseña en desarrollo",
                    Type = "Change Password"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el cambio de contraseña");
                return StatusCode(500, new ApiRequestResponse
                {
                    Code = 500,
                    Message = "Error interno del servidor",
                    Type = "Change Password",
                    StackTrace = ex.Message
                });
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        private string? GetCurrentUserEmail()
        {
            return User.FindFirst("email")?.Value;
        }

        private string? GetCurrentUserRole()
        {
            return User.FindFirst("role")?.Value;
        }
    }
}
