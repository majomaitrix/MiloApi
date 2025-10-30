using Microsoft.OpenApi.Models;
using System.Reflection;

namespace MiloAPI.Configuration
{
    internal static class SwaggerExtensions
    {
        internal static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Milo API - Sistema de Gestión de Restaurante",
                    Version = "v1",
                    Description = "API REST para la gestión de un sistema de restaurante. Incluye funcionalidades de productos, categorías, pedidos, usuarios y autenticación con JWT.",
                    Contact = new OpenApiContact
                    {
                        Name = "Milo API Support"
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            return services;
        }

        internal static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "InventarioAPiServices V1");
                c.InjectStylesheet("../assets/css/swagger-custom-style.css");
            });

            return app;
        }
    }
}
