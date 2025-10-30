using Microsoft.EntityFrameworkCore;
using Milo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Producto> productos { get; set; }
        public DbSet<Usuario> usuarios { get; set; }
        public DbSet<Pedido> pedidos { get; set; }
        public DbSet<Rol> roles { get; set; }
        public DbSet<Mesa> mesas { get; set; }
        public DbSet<Categoria> categorias { get; set; }
        public DbSet<PedidoDetalle> pedidosDetalle { get; set; }
        public DbSet<Reservacion> reservaciones { get; set; }
        public DbSet<Venta> ventas { get; set; }
        public DbSet<VentaDetalle> ventasDetalle { get; set; }
        public DbSet<RefreshToken> refreshTokens { get; set; }
    }
}
