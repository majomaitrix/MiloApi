using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Milo.Application.Models;
using Milo.Application.Models.DTOs;
using Milo.Application.Parameter.Services;
using Milo.Application.Services;
using Milo.Application.Usuarios;
using Milo.Infrastructure.Persistence;
using MiloAPI.Configuration;
using Serilog;
using System.Reflection;
using System.Text;

namespace MiloAPI
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Mapea todo el appsettings.json a AppConfiguration
            var appConfig = new AppConfiguration();
            _configuration.Bind(appConfig);
            services.AddSingleton(appConfig); // Inyectable en toda la app
            // Configurar la base de datos (prioriza variable de entorno y luego appsettings)
            var defaultConnection = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                                   ?? _configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(defaultConnection));

            // Configurar CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddSwaggerDocumentation();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUserQueryHandler).Assembly));
            
            // Configurar validación automática de modelos
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = false;
            });

            // Configurar Serilog
            services.AddSerilog();

            // Configurar autenticación JWT
            var jwtSettings = _configuration.GetSection("Jwt").Get<JwtSettings>();
            if (jwtSettings != null)
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                            ValidateIssuer = true,
                            ValidIssuer = jwtSettings.Issuer,
                            ValidateAudience = true,
                            ValidAudience = jwtSettings.Audience,
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero
                        };
                    });
            }

            // Configurar autorización
            services.AddAuthorization();

            // Registrar servicios de aplicación
            services.AddScoped<IEstadoPedidoService, EstadoPedidoService>();
            services.AddScoped<IHealthCheckService, HealthCheckService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            // Habilitar Swagger en todos los entornos (incluyendo Production)
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Milo API v1");
                c.RoutePrefix = string.Empty; // Sirve Swagger en "/"
            });

            // Middleware de Serilog para logging de requests
            app.UseSerilogRequestLogging();

            // Habilitar CORS
            app.UseCors("AllowFrontend");

            // Middleware personalizado para manejo de errores
            app.UseMiddleware<Middleware.ValidationMiddleware>();

            // Middleware JWT para autenticación
            app.UseMiddleware<Middleware.JwtMiddleware>();

            // Configurar autenticación y autorización
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}
