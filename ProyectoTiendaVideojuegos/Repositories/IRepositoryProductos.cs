using NugetProyectoTinedaVideojuegosAzure;
using System.Collections.Generic;

namespace ProyectoTiendaVideojuegos.Repositories
{
    public interface IRepositoryProductos
    {
        List<Producto> GetProductosPS4();
        List<Producto> GetProductosPS5();
        List<Producto> GetTazas();
        List<Categoria>GetCategorias();
        List<SubCategoria> GetSubCategorias();
        List<Producto> GetPorductosGrid(int id);
        List<Producto> GetTodosProductos();
        List<Producto> GetBuscadorProductos(string buscar);
        List<Producto> FiltrarPorPlataforma(List<string> plataformas);
        List<Producto> FiltrarPorGenero(List<string> generos);
        List<Producto> FiltrarPorPrecio(int? precioMinimo, int? precioMaximo);
        Producto DetallesProductos(int idproducto);
        List<Producto> BuscarProductoCarrito(List<int>? idproductoCarrito);
        List<Producto> BuscarProductoFavorito(List<int>? idproductoFav);
        List<Pedido> GetPedidos();
        void AgregarPedido(List<Producto> productos, int idCliente, int precioTotal, List<int> cantidad);
        void DeleteProductos(int idproducto);
        void UpdatePorducto(Producto producto);

        List<DetallesPedido> MostrarPedidos(int idcliente);
    }
}
