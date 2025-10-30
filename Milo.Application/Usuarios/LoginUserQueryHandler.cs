using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Milo.Application.Models;
using Milo.Application.Models.DTOs;
using Milo.Application.Usuarios.DTOs;
using Milo.Infrastructure.Persistence;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Application.Usuarios
{
    public sealed class LoginUserQueryHandler : IRequestHandler<LoginUserQuery,Info_data_obj<LoginUserOut>>
    {
        private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;
        private readonly AppConfiguration _appConfiguration;

        public LoginUserQueryHandler(IMediator mediator, ApplicationDbContext context, AppConfiguration appConfiguration)
        {
            this._mediator = mediator;
            this._context = context;
            this._appConfiguration = appConfiguration;
        }

        public async Task<Info_data_obj<LoginUserOut>> Handle(LoginUserQuery req, CancellationToken cancellationToken)
        {
            Info_data_obj<LoginUserOut> response = new Info_data_obj<LoginUserOut>();
            LoginUserOut usuarioRes = new LoginUserOut();
            try
            {
                Log.Information("Iniciando proceso de login para usuario {Email}", req.User.Email);

                // 1. Buscar usuario por email
                var usuario = await _context.usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == req.User.Email);

                if(usuario == null)
                {
                    Log.Warning("Usuario no encontrado para email {Email}", req.User.Email);
                    response = new Info_data_obj<LoginUserOut>(){
                        code = Constants.CodeCatch,
                        message = Constants.MessageNotFound,
                        message_error = Constants.MessageNotFoundErr,
                        Data = null
                    };
                    return response;
                }

                Log.Debug("Usuario encontrado: {UserId} con rol {Rol}", usuario.Id, usuario.Rol?.Nombre);

                // 2. Verificar contraseña
                bool passwordCorrecta = BCrypt.Net.BCrypt.Verify(req.User.Password, usuario.Password);

                if (!passwordCorrecta)
                {
                    Log.Warning("Contraseña incorrecta para usuario {UserId}", usuario.Id);
                    response = new Info_data_obj<LoginUserOut>(){
                        code = Constants.CodeCatch,
                        message = Constants.MessageNotFound,
                        message_error = "Credenciales inválidas",
                        Data = null
                    };
                    return response;
                }

                // 3. Generar token
                var token = GenerarToken(usuario.Id,req.User.Email,usuario.Rol.Nombre);
                GetRolesOut rolesUser = new GetRolesOut()
                {
                    Id=usuario.Rol.Id,
                    Nombre=usuario.Rol.Nombre
                };
                usuarioRes = new LoginUserOut()
                {
                    id = usuario.Id,
                    email = usuario.Email,
                    direccion=usuario.Direccion,
                    nombre=usuario.Nombre,
                    telefono=usuario.Telefono,
                    token=token,
                    Rol= rolesUser
                };

                Log.Information("Login exitoso para usuario {UserId} con rol {Rol}", usuario.Id, usuario.Rol.Nombre);

                response = new Info_data_obj<LoginUserOut>()
                {
                    code = Constants.CodeSuccess,
                    message = Constants.MessageSuccess,
                    message_error = "",
                    Data= usuarioRes
                };

                return response;
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error durante el proceso de login para usuario {Email}", req.User.Email);
                return new Info_data_obj<LoginUserOut>
                {
                    code = Constants.CodeCatch,
                    message = Constants.MessageCatch,
                    message_error = ex.Message,
                    Data = null
                };
            }
        }


        public string GenerarToken(int userId, string email, string rol)
        {
            string _jwtSecret=_appConfiguration.Jwt.Secret;
            int _jwtLifespanMinutes= _appConfiguration.Jwt.LifespanMinutes;

            var expiration = DateTime.UtcNow.AddMinutes(_jwtLifespanMinutes);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, rol),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique ID
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                issuer: _appConfiguration.Jwt.Issuer,
                audience: _appConfiguration.Jwt.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return tokenString;
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }
    }
}
