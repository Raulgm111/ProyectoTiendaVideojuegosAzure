using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NugetProyectoTinedaVideojuegosAzure;
using ProyectoTiendaVideojuegos.Extensions;
using ProyectoTiendaVideojuegos.Filters;
using ProyectoTiendaVideojuegosAzure.Services;
using System.Collections.Generic;

namespace ProyectoTiendaVideojuegos.Controllers
{
    public class ProductosController : Controller
    {
        private ServiceApiProductos service;

        public ProductosController(ServiceApiProductos service)
        {
            this.service = service;
        }

        public async Task<IActionResult> BuscarProductos(string buscar)
        {
            List<Producto> productos = await this.service.BuscarProductos(buscar);
            return PartialView("_ResultadosBusqueda", productos);
        }

        public async Task<IActionResult> VistasGridTodosProductos(List<string> plataforma,
                List<string> generos, int? precioMinimo, int? precioMaximo, int? idproductoCarrito)
        {
            CategoriasViewModel enlace = await this.service.VistasGridTodosProductos(plataforma, generos, precioMinimo, precioMaximo, idproductoCarrito);
            return View(enlace);
        }

        public async Task<IActionResult> VistasGrid(int id, List<string> plataforma,
            List<string> generos, int? precioMinimo, int? precioMaximo)
        {
            CategoriasViewModel enlace = await this.service.VistasGrid(id, plataforma, generos, precioMinimo, precioMaximo);
            return View(enlace);
        }

        public async Task<IActionResult> MisVistas(string nombre)
        {
            CategoriasViewModel enlace = await this.service.MisVistas(nombre);
            return View(enlace);
        }

        public async Task<IActionResult> VistasDetalles(int idproducto, int? idproductoAñadir, int? idproductoAñadirFav)
        {
            CategoriasViewModel enlace = await this.service.VistasDetalles(idproducto, idproductoAñadir, idproductoAñadirFav);
            return View(enlace);
        }

        [HttpPost]
        public async Task<IActionResult> VistasDetalles(int idproducto, int cantidad, string accion)
        {
            await this.service.VistasDetalles(idproducto, cantidad, accion);
            return RedirectToAction("MisVistas");
        }

        public async Task<IActionResult> Carrito(int? idproductoCarrito, int? ideliminar, int? eliminarTodo, int? cantidad)
        {
            List<Producto> carrito = await this.service.Carrito(idproductoCarrito, ideliminar, eliminarTodo, cantidad);
            return View(carrito);
        }

        public async Task<IActionResult> MostrarPedidos(int idcliente)
        {
            List<DetallesPedido> pedidos = await this.service.MostrarPedidos(idcliente);
            return View(pedidos);
        }

        [HttpPost]
        public async Task<IActionResult> Pedidos(int idcliente)
        {
            await this.service.Pedidos();
            return RedirectToAction("MostrarPedidos", new { idcliente = idcliente });
        }

        public async Task<IActionResult> Favoritos(int? idproductoFav, int? ideliminar)
        {
            List<Producto> fav = await this.service.Favoritos(idproductoFav, ideliminar);
            return View(fav);
        }

        public async Task<IActionResult> GetProductosAdmin()
        {
            List<Producto> productos = await this.service.GetProductosAdmin();
            return View(productos);
        }

        public async Task<IActionResult> DeleteProducto(int ididproducto)
        {
            Producto producto = await this.service.DeleteProducto(ididproducto);
            return View(producto);
        }

        public async Task<IActionResult> EliminarProducto(int ididproducto)
        {
            await this.service.EliminarProducto(ididproducto);
            return RedirectToAction("GetProductosAdmin");
        }

        public async Task<IActionResult> EditarProdAdmin(int idproducto)
        {
            Producto producto = await this.service.EditarProdAdmin(idproducto);
            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> EditarProdAdmin(Producto producto)
        {
            await this.service.EditarProdAdmin(producto);
            return RedirectToAction("GetProductosAdmin");
        }
    }
}
