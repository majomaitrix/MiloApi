using Microsoft.AspNetCore.Mvc;
using Milo.Application.Services;

namespace MiloAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly IHealthCheckService _healthCheckService;

        public HealthController(IHealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        /// <summary>
        /// Verifica el estado general de salud del sistema
        /// </summary>
        /// <returns>Información detallada del estado de salud incluyendo base de datos y servicios externos</returns>
        /// <response code="200">Sistema saludable o degradado</response>
        /// <response code="503">Sistema no saludable</response>
        [HttpGet]
        public async Task<IActionResult> GetHealth()
        {
            var healthResult = await _healthCheckService.CheckHealthAsync();
            
            // Retornar código de estado HTTP apropiado
            var statusCode = healthResult.Status switch
            {
                "Healthy" => 200,
                "Degraded" => 200,
                "Unhealthy" => 503,
                _ => 200
            };

            return StatusCode(statusCode, healthResult);
        }

        /// <summary>
        /// Verifica si la aplicación está lista para recibir tráfico
        /// </summary>
        /// <returns>Estado de readiness del sistema</returns>
        /// <remarks>
        /// Este endpoint verifica que todos los componentes críticos estén funcionando correctamente.
        /// Retorna 200 solo si el sistema está completamente saludable.
        /// </remarks>
        /// <response code="200">Aplicación lista para recibir tráfico</response>
        /// <response code="503">Aplicación no lista</response>
        [HttpGet("ready")]
        public async Task<IActionResult> GetReadiness()
        {
            var healthResult = await _healthCheckService.CheckHealthAsync();
            
            // Solo retorna 200 si está completamente saludable
            if (healthResult.Status == "Healthy")
            {
                return Ok(new { status = "Ready", timestamp = DateTime.UtcNow });
            }
            
            return StatusCode(503, new { status = "Not Ready", timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// Verifica si la aplicación está viva y ejecutándose
        /// </summary>
        /// <returns>Estado de liveness del sistema</returns>
        /// <remarks>
        /// Este endpoint realiza una verificación simple de que la aplicación está corriendo.
        /// No verifica dependencias externas como base de datos.
        /// </remarks>
        /// <response code="200">Aplicación está viva</response>
        [HttpGet("live")]
        public IActionResult GetLiveness()
        {
            // Liveness check simple - solo verifica que la aplicación está corriendo
            return Ok(new { status = "Alive", timestamp = DateTime.UtcNow });
        }
    }
}


