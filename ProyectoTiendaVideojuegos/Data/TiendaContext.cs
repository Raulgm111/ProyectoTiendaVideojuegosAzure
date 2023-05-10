using Microsoft.EntityFrameworkCore;
using NugetProyectoTinedaVideojuegosAzure;

namespace ProyectoTiendaVideojuegos.Data
{
    public class TiendaContext :DbContext
    {
        public TiendaContext(DbContextOptions<TiendaContext> options)
            : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<SubCategoria> SubCategorias { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallesPedido> DetallesPedido { get;set; }
    }
}
