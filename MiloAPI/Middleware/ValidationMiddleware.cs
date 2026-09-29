using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace MiloAPI.Middleware
{
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                await HandleValidationExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    code = 500,
                    message = "An unexpected error occurred.",
                    type = "InternalServerError"
                };

                var jsonResponse = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(jsonResponse);
            }
        }

        private static async Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";

            var response = new
            {
                Code = 400,
                Message = "Error de validación",
                Type = "Validación de datos",
                StackTrace = ex.Message,
                Errors = new[] { ex.Message }
            };

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}



