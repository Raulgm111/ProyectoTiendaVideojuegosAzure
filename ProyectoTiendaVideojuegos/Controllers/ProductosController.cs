using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NugetProyectoTinedaVideojuegosAzure;
using ProyectoTiendaVideojuegos.Extensions;
using ProyectoTiendaVideojuegos.Filters;
using ProyectoTiendaVideojuegos.Repositories;

namespace ProyectoTiendaVideojuegos.Controllers
{
    public class ProductosController : Controller
    {
        private IRepositoryProductos repo;

        public ProductosController(IRepositoryProductos repo)
        {
            this.repo = repo;
        }

        public IActionResult BuscarProductos(string buscar)
        {
            var productos = this.repo.GetBuscadorProductos(buscar);
            return PartialView("_ResultadosBusqueda", productos);
        }


        #region VISTAS COMPLETAS
        public IActionResult VistasGridTodosProductos(List<string> plataforma,
        List<string> generos, int? precioMinimo, int? precioMaximo, int? idproductoCarrito)
        {
            CategoriasViewModel enlace = new CategoriasViewModel();
            enlace.Categorias = this.repo.GetCategorias();
            enlace.Subcategorias = this.repo.GetSubCategorias();

            if (plataforma != null && plataforma.Any())
            {
                enlace.Productos = this.repo.FiltrarPorPlataforma(plataforma);
            }
            else if (generos != null && generos.Any())
            {
                enlace.Productos = this.repo.FiltrarPorGenero(generos);
            }
            else
            {
                enlace.Productos = this.repo.GetTodosProductos();
            }

            if (precioMinimo.HasValue && precioMaximo.HasValue)
            {
                enlace.Productos = enlace.Productos.Where(p => p.Precio >= precioMinimo.Value && p.Precio <= precioMaximo.Value).ToList();
            }
            return View(enlace);
        }




        public IActionResult VistasGrid(int id, List<string> plataforma,
        List<string> generos, int? precioMinimo, int? precioMaximo)
        {
            CategoriasViewModel enlace = new CategoriasViewModel();
            enlace.Categorias = this.repo.GetCategorias();
            enlace.Subcategorias = this.repo.GetSubCategorias();
            enlace.Productos = this.repo.GetPorductosGrid(id);
            if (plataforma != null && plataforma.Any())
            {
                enlace.Productos = this.repo.FiltrarPorPlataforma(plataforma);
            }
            else if (generos != null && generos.Any())
            {
                enlace.Productos = this.repo.FiltrarPorGenero(generos);
            }

            if (precioMinimo.HasValue && precioMaximo.HasValue)
            {
                enlace.Productos = enlace.Productos.Where(p => p.Precio >= precioMinimo.Value && p.Precio <= precioMaximo.Value).ToList();
            }
            return View(enlace);
        }

        public IActionResult MisVistas(string nombre)
        {
            CategoriasViewModel enlace = new CategoriasViewModel();
            enlace.Categorias = this.repo.GetCategorias();
            enlace.ProductosPS4 = this.repo.GetProductosPS4();
            enlace.ProductosPS5 = this.repo.GetProductosPS5();
            enlace.Tazas = this.repo.GetTazas();
            enlace.Subcategorias = this.repo.GetSubCategorias();
            return View(enlace);

        }

        public IActionResult VistasDetalles(int idproducto,int? idproductoAñadir, int?idproductoAñadirFav)
        {
            CategoriasViewModel enlace = new CategoriasViewModel();
            enlace.Categorias = this.repo.GetCategorias();
            enlace.Subcategorias = this.repo.GetSubCategorias();
            if (idproductoAñadir != null)
            {
                List<int> carrito;
                if (HttpContext.Session.GetObject<List<int>>("CARRITO") == null)
                {
                    carrito = new List<int>();
                }
                else
                {
                    carrito = HttpContext.Session.GetObject<List<int>>("CARRITO");
                }
                if (carrito.Contains(idproductoAñadir.Value) == false)
                {
                    carrito.Add(idproductoAñadir.Value);
                    HttpContext.Session.SetObject("CARRITO", carrito);
                }

            }
            if (idproductoAñadirFav != null)
            {
                List<int> favoritos;
                if (HttpContext.Session.GetObject<List<int>>("FAVORITO") == null)
                {
                    favoritos = new List<int>();
                }
                else
                {
                    favoritos = HttpContext.Session.GetObject<List<int>>("FAVORITO");
                }
                if (favoritos.Contains(idproductoAñadirFav.Value) == false)
                {
                    favoritos.Add(idproductoAñadirFav.Value);
                    HttpContext.Session.SetObject("FAVORITO", favoritos);
                }

            }
            enlace.Producto = this.repo.DetallesProductos(idproducto);
            return View(enlace);
        }

        [HttpPost]
        [AuthorizeClientes]
        public IActionResult VistasDetalles(int idproducto, int cantidad, string accion)
        {
            if (accion == "AgregarCarrito")
            {
                List<int> carrito = HttpContext.Session.GetObject<List<int>>("CARRITO");
                if (carrito == null)
                {
                    carrito = new List<int>();
                }

                for (int i = 0; i < cantidad; i++)
                {
                    carrito.Add(idproducto);
                }

                HttpContext.Session.SetObject("CARRITO", carrito);
            }
            else if (accion == "AgregarFavoritos")
            {
                List<int> favoritos = HttpContext.Session.GetObject<List<int>>("FAVORITO");
                if (favoritos == null)
                {
                    favoritos = new List<int>();
                }

                for (int i = 0; i < cantidad; i++)
                {
                    favoritos.Add(idproducto);
                }

                HttpContext.Session.SetObject("FAVORITO", favoritos);
            }

            return RedirectToAction("MisVistas");
        }


        #endregion

        #region CARRITO
        [AuthorizeClientes]
        public IActionResult Carrito(int? idproductoCarrito, int? ideliminar, int? eliminarTodo, int? cantidad)
        {
            List<int> carrito = HttpContext.Session.GetObject<List<int>>("CARRITO");
            if (carrito == null)
            {
                return View();
            }
            else
            {
                if (eliminarTodo != null)
                {
                    carrito.RemoveAll(id => id == eliminarTodo.Value);
                    if (carrito.Count == 0)
                    {
                        HttpContext.Session.Remove("CARRITO");
                    }
                    else
                    {
                        HttpContext.Session.SetObject("CARRITO", carrito);
                    }
                }
                else if (ideliminar != null)
                {
                    if (cantidad != null)
                    {
                        for (int i = 0; i < cantidad.Value; i++)
                        {
                            carrito.Remove(ideliminar.Value);
                        }
                    }
                    else
                    {
                        carrito.Remove(ideliminar.Value);
                    }

                    if (carrito.Count == 0)
                    {
                        HttpContext.Session.Remove("CARRITO");
                    }
                    else
                    {
                        HttpContext.Session.SetObject("CARRITO", carrito);
                    }
                }
                else if (idproductoCarrito != null)
                {
                    if (cantidad != null)
                    {
                        for (int i = 0; i < cantidad.Value; i++)
                        {
                            carrito.Add(idproductoCarrito.Value);
                        }
                    }
                    else
                    {
                        carrito.Add(idproductoCarrito.Value);
                    }

                    HttpContext.Session.SetObject("CARRITO", carrito);
                }

                List<Producto> productos = this.repo.BuscarProductoCarrito(carrito);
                return View(productos);
            }



        }
        #endregion

        public IActionResult MostrarPedidos(int idcliente)
        {
            List<DetallesPedido> pedidos = this.repo.MostrarPedidos(idcliente);
            return View(pedidos);
        }

        [HttpPost]
        public IActionResult Pedidos()
        {
            List<int> carrito = HttpContext.Session.GetObject<List<int>>("CARRITO");
            int idCliente = int.Parse(HttpContext.User.FindFirst("IdCliente").Value);

            List<Producto> productos = this.repo.BuscarProductoCarrito(carrito);

            int precioTotal = productos.Sum(p => p.Precio);

            this.repo.AgregarPedido(productos, idCliente, precioTotal,carrito);

            HttpContext.Session.Remove("CARRITO");

            return RedirectToAction("MostrarPedidos", new { idcliente = idCliente });
                  
        }






        //public IActionResult Pedidos()
        //{
        //    List<int> carrito = HttpContext.Session.GetObject<List<int>>("CARRITO");
        //    List<Producto> productos = this.repo.BuscarProductoCarrito(carrito);
        //    HttpContext.Session.Remove("CARRITO");
        //    return View(productos);
        //}


        [AuthorizeClientes]
        public IActionResult Favoritos(int? idproductoFav, int? ideliminar)
        {
            List<int> favoritos = HttpContext.Session.GetObject<List<int>>("FAVORITO");
            if (favoritos == null)
            {
                return View();
            }
            else
            {
                if (ideliminar != null)
                {
                    favoritos.Remove(ideliminar.Value);
                    if (favoritos.Count == 0)
                    {
                        HttpContext.Session.Remove("FAVORITO");
                    }
                    else
                    {
                        if (idproductoFav != null)
                        {
                            favoritos.Remove(idproductoFav.Value);
                            HttpContext.Session.SetObject("FAVORITO", favoritos);
                        }
                    }
                }
                List<Producto> productos = this.repo.BuscarProductoFavorito(favoritos);
                return View(productos);
            }
        }

        [AuthorizeClientes(Policy = "AdminOnly")]
        public IActionResult GetProductosAdmin()
        {
            List<Producto> productos =
                 this.repo.GetTodosProductos();
            return View(productos);
        }

        [AuthorizeClientes(Policy = "AdminOnly")]
        public IActionResult DeleteProducto(int ididproducto)
        {
            Producto producto =
                 this.repo.DetallesProductos(ididproducto);
            return View(producto);
        }
        [AuthorizeClientes(Policy = "AdminOnly")]
        public IActionResult EliminarProducto(int ididproducto)
        {
            this.repo.DeleteProductos(ididproducto);
            return RedirectToAction("GetProductosAdmin");
        }

        [AuthorizeClientes(Policy = "AdminOnly")]
        public IActionResult EditarProdAdmin(int idproducto)
        {
            Producto porducto = this.repo.DetallesProductos(idproducto);
            return View(porducto);
        }

        [HttpPost]
        [AuthorizeClientes(Policy = "AdminOnly")]
        public IActionResult EditarProdAdmin(Producto producto)
        {
            this.repo.UpdatePorducto(producto);
            return RedirectToAction("GetProductosAdmin");
        }


    }
}
