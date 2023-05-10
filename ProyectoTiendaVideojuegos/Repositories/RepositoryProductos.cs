using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NugetProyectoTinedaVideojuegosAzure;
using ProyectoTiendaVideojuegos.Data;
using System.Data.Entity;

namespace ProyectoTiendaVideojuegos.Repositories
{
    #region PROCEDIMIENTOS
    //CREATE PROCEDURE SP_PRODUCTOS_PS4
    //AS
    //      select top 4 * from Productos where Productos.IdCategoria=2
    //	    ORDER BY NEWID()
    //GO
    #endregion
    public class RepositoryProductos : IRepositoryProductos
    {
        private TiendaContext context;

        public RepositoryProductos(TiendaContext context)
        {
            this.context = context;
        }
        public List<Producto> GetProductosPS4()
        {
            var consulta = from datos in this.context.Productos
                           where datos.IdSubCategoria == 6
                           orderby Guid.NewGuid()
                           select datos;
            var productos = consulta.Take(4).ToList();
            return productos;

        }
        public List<Producto> GetProductosPS5()
        {
            var consulta = from datos in this.context.Productos
                           where datos.IdSubCategoria == 7
                           orderby Guid.NewGuid()
                           select datos;
            var productos = consulta.Take(4).ToList();
            return productos;

        }
        public List<Producto> GetTazas()
        {
            var consulta = from datos in this.context.Productos
                           where datos.IdSubCategoria == 12
                           orderby Guid.NewGuid()
                           select datos;
            var productos = consulta.Take(4).ToList();
            return productos;

        }
        public List<Categoria> GetCategorias()
        {
            var consulta = from datos in this.context.Categorias
                           select datos;
            return consulta.ToList();
        }

        public List<SubCategoria> GetSubCategorias()
        {
            var consulta = from datos in this.context.SubCategorias
                           select datos;
            return consulta.ToList();

        }

        public List<Producto> GetPorductosGrid(int id)
        {
            var consulta = from datos in context.Productos
                           where datos.IdSubCategoria == id
                           select datos;

            return consulta.ToList();
        }

        public List<Producto> GetTodosProductos()
        {
            var consulta = from datos in context.Productos
                           select datos;
            return consulta.ToList();
        }

        public List<Producto> GetBuscadorProductos(string buscar)
        {
            var consulta = from datos in this.context.Productos
                           select datos;

            if (!string.IsNullOrEmpty(buscar))
            {
                consulta = consulta.Where(x => x.NombreProducto.Contains(buscar));
            }

            return consulta.ToList();
        }

        public List<Producto> FiltrarPorPlataforma(List<string> plataformas)
        {
            var consulta = from productos in context.Productos
                           join categorias in context.Categorias
                           on productos.IdCategoria equals categorias.IdCategoria
                           where plataformas.Contains(categorias.NombreCategoria)
                           select productos;

            return consulta.ToList();
        }

        public List<Producto> FiltrarPorGenero(List<string> generos)
        {
            var consulta = (from productos in context.Productos
                           where generos.Contains(productos.Genero)
                           select productos).Distinct();
            return consulta.ToList();
        }

        public List<Producto> FiltrarPorPrecio(int? precioMinimo, int? precioMaximo)
        {
                        var consulta = from productos in context.Productos
                           where productos.Precio >= precioMinimo && productos.Precio <= precioMaximo
                           select productos;
            return consulta.ToList();
        }

        public Producto DetallesProductos(int idproducto)
        {
            var consulta = from productos in context.Productos
                           where productos.IdProducto == idproducto
                           select productos;
            return consulta.FirstOrDefault();
        }

        public List<Producto> BuscarProductoCarrito(List<int>? idproductoCarrito)
        {
            var consulta = from datos in this.context.Productos
                           where idproductoCarrito.Contains(datos.IdProducto)
                           select datos;
            return consulta.ToList();
        }

        public List<Producto> BuscarProductoFavorito(List<int> idproductoFav)
        {
            var consulta = from datos in this.context.Productos
                           where idproductoFav.Contains(datos.IdProducto)
                           select datos;
            return consulta.ToList();
        }

        private int GetMaxIdDetallesPedido()
        {
            if (this.context.DetallesPedido.Count() == 0)
            {
                return 1;
            }
            else
            {
                return this.context.DetallesPedido.Max(z => z.IdDetallesPedido) + 1;
            }
        }

        private int GetMaxIdPedido()
        {
            if (this.context.Pedidos.Count() == 0)
            {
                return 1;
            }
            else
            {
                return this.context.Pedidos.Max(z => z.IdPedido) + 1;
            }
        }

        public List<Pedido> GetPedidos()
        {
            var consulta = from datos in context.Pedidos
                           select datos;
            return consulta.ToList();
        }

        public void AgregarPedido(List<Producto> productos, int idCliente, int precioTotal, List<int> cantidad)
        {
            int idPedido = GetMaxIdPedido();

            Pedido pedidoGeneral = new Pedido();
            pedidoGeneral.IdPedido = idPedido;
            pedidoGeneral.IdCliente = idCliente;
            pedidoGeneral.PrecioTotal = precioTotal;
            pedidoGeneral.Cantidad = productos.Count;

            context.Pedidos.Add(pedidoGeneral);
            context.SaveChanges();

            int idDetallesPedido = GetMaxIdDetallesPedido(); 
            foreach (Producto producto in productos)
            {
                DetallesPedido detallePedido = new DetallesPedido();
                detallePedido.IdDetallesPedido = idDetallesPedido + 1; 
                detallePedido.IdPedido = idPedido;
                detallePedido.IdProducto = producto.IdProducto;
                detallePedido.Cantidad = cantidad.Count(x => x == producto.IdProducto);
                detallePedido.PrecioTotal = producto.Precio * cantidad.Count(x => x == producto.IdProducto);

                context.DetallesPedido.Add(detallePedido);
                idDetallesPedido = detallePedido.IdDetallesPedido; 
            }

            context.SaveChanges();
        }

        public List<DetallesPedido> MostrarPedidos(int idcliente)
        {
            var consulta = from pedido in context.Pedidos
                           join detalle in context.DetallesPedido on pedido.IdPedido equals detalle.IdPedido
                           join producto in context.Productos on detalle.IdProducto equals producto.IdProducto
                           where pedido.IdCliente == idcliente
                           select new DetallesPedido
                           {
                               IdPedido = pedido.IdPedido,
                               IdProducto = detalle.IdProducto,
                               Cantidad = detalle.Cantidad,
                               PrecioTotal = detalle.PrecioTotal,
                               NombreProducto = producto.NombreProducto
                           };

            return consulta.ToList();

        }

        public void DeleteProductos(int idproducto)
        {
            Producto producto = this.DetallesProductos(idproducto);
            if (producto != null)
            {
                this.context.Productos.Remove(producto);
                this.context.SaveChanges();
            }
        }

        public void UpdatePorducto(Producto producto)
        {
            Producto prod = this.DetallesProductos(producto.IdProducto);

            prod.NombreProducto = producto.NombreProducto;
            prod.Lanzamiento = producto.Lanzamiento;
            prod.Imagen = producto.Imagen;
            prod.Precio = producto.Precio;
            prod.Descripcion = producto.Descripcion;
            prod.Genero = producto.Genero;
            this.context.SaveChanges();
        }











    }
}
