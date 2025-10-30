using Microsoft.EntityFrameworkCore;
using Milo.Infrastructure.Persistence;

namespace Milo.Application.Services
{
    public interface IHealthCheckService
    {
        Task<HealthCheckResult> CheckHealthAsync();
    }

    public class HealthCheckService : IHealthCheckService
    {
        private readonly ApplicationDbContext _context;

        public HealthCheckService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync()
        {
            var result = new HealthCheckResult
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Checks = new List<HealthCheckItem>()
            };

            // Verificar base de datos
            var dbCheck = await CheckDatabaseAsync();
            result.Checks.Add(dbCheck);

            // Verificar servicios externos (si los hay)
            var externalCheck = CheckExternalServices();
            result.Checks.Add(externalCheck);

            // Determinar estado general
            if (result.Checks.Any(c => c.Status == "Unhealthy"))
            {
                result.Status = "Unhealthy";
            }
            else if (result.Checks.Any(c => c.Status == "Degraded"))
            {
                result.Status = "Degraded";
            }

            return result;
        }

        private async Task<HealthCheckItem> CheckDatabaseAsync()
        {
            try
            {
                // Verificar conexión con una consulta simple
                await _context.Database.ExecuteSqlRawAsync("SELECT 1");
                
                return new HealthCheckItem
                {
                    Name = "Database",
                    Status = "Healthy",
                    Description = "Database connection is working",
                    Duration = TimeSpan.Zero
                };
            }
            catch (Exception ex)
            {
                return new HealthCheckItem
                {
                    Name = "Database",
                    Status = "Unhealthy",
                    Description = $"Database connection failed: {ex.Message}",
                    Duration = TimeSpan.Zero
                };
            }
        }

        private HealthCheckItem CheckExternalServices()
        {
            // Por ahora solo verificamos que no hay servicios externos
            // En el futuro se pueden agregar verificaciones de APIs externas
            return new HealthCheckItem
            {
                Name = "External Services",
                Status = "Healthy",
                Description = "No external services configured",
                Duration = TimeSpan.Zero
            };
        }
    }

    public class HealthCheckResult
    {
        public string Status { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public List<HealthCheckItem> Checks { get; set; } = new List<HealthCheckItem>();
    }

    public class HealthCheckItem
    {
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
    }
}


