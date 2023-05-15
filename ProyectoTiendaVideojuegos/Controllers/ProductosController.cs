using Azure.Storage.Blobs;
using Azure.Storage.Sas;
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
        private ServiceStorageBlobs serviceStorageBlobs;
        private string containerName = "imagenesproductos";

        public ProductosController(ServiceApiProductos service, ServiceStorageBlobs serviceStorageBlobs, IConfiguration configuration)
        {
            this.service = service;
            this.serviceStorageBlobs = serviceStorageBlobs;
        }

        public async Task<IActionResult> BuscarProductos(string buscar)
        {
            List<Producto> productos = await this.service.BuscarProductos(buscar);
            return PartialView("_ResultadosBusqueda", productos);
        }

        public async Task<IActionResult> VistasGridTodosProductos(List<string> plataformas,
                List<string> generos, int? precioMinimo, int? precioMaximo, int? idproductoCarrito)
        {
            CategoriasViewModel enlace = await this.service.VistasGridTodosProductos(plataformas, generos, precioMinimo, precioMaximo, idproductoCarrito);
            foreach (Producto prod in enlace.Productos)
            {
                string blobname = prod.Imagen;
                if (blobname != null)
                {
                    BlobContainerClient blobcontainerclient = await this.serviceStorageBlobs.GetContainersAsync(this.containerName);
                    BlobClient blocclient = blobcontainerclient.GetBlobClient(blobname);

                    BlobSasBuilder sasbuilder = new BlobSasBuilder()
                    {
                        BlobContainerName = this.containerName,
                        BlobName = blobname,
                        Resource = "b",
                        StartsOn = DateTimeOffset.UtcNow,
                        ExpiresOn = DateTime.UtcNow.AddHours(1)
                    };
                    sasbuilder.SetPermissions(BlobSasPermissions.Read);
                    var uri = blocclient.GenerateSasUri(sasbuilder);
                    prod.Imagen = uri.ToString();
                }
            }
            return View(enlace);
        }

        public async Task<IActionResult> VistasGrid(int id, List<string> plataforma,
            List<string> generos, int? precioMinimo, int? precioMaximo)
        {
            CategoriasViewModel enlace = await this.service.VistasGrid(id, plataforma, generos, precioMinimo, precioMaximo);
            foreach (Producto prod in enlace.Productos)
            {
                string blobname = prod.Imagen;
                if (blobname != null)
                {
                    BlobContainerClient blobcontainerclient = await this.serviceStorageBlobs.GetContainersAsync(this.containerName);
                    BlobClient blocclient = blobcontainerclient.GetBlobClient(blobname);

                    BlobSasBuilder sasbuilder = new BlobSasBuilder()
                    {
                        BlobContainerName = this.containerName,
                        BlobName = blobname,
                        Resource = "b",
                        StartsOn = DateTimeOffset.UtcNow,
                        ExpiresOn = DateTime.UtcNow.AddHours(1)
                    };
                    sasbuilder.SetPermissions(BlobSasPermissions.Read);
                    var uri = blocclient.GenerateSasUri(sasbuilder);
                    prod.Imagen = uri.ToString();
                }
            }
            return View(enlace);
        }

        public async Task<IActionResult> MisVistas(string nombre)
        {
            CategoriasViewModel enlace = await this.service.MisVistas(nombre);

            await GenerarUrlsImagenes(enlace.ProductosPS4);
            await GenerarUrlsImagenes(enlace.ProductosPS5);
            await GenerarUrlsImagenes(enlace.Tazas);

            return View(enlace);
        }

        private async Task GenerarUrlsImagenes(List<Producto> productos)
        {
            foreach (Producto prod in productos)
            {
                string blobname = prod.Imagen;
                if (blobname != null)
                {
                    BlobContainerClient blobcontainerclient = await this.serviceStorageBlobs.GetContainersAsync(this.containerName);
                    BlobClient blocclient = blobcontainerclient.GetBlobClient(blobname);

                    BlobSasBuilder sasbuilder = new BlobSasBuilder()
                    {
                        BlobContainerName = this.containerName,
                        BlobName = blobname,
                        Resource = "b",
                        StartsOn = DateTimeOffset.UtcNow,
                        ExpiresOn = DateTime.UtcNow.AddHours(1)
                    };
                    sasbuilder.SetPermissions(BlobSasPermissions.Read);
                    var uri = blocclient.GenerateSasUri(sasbuilder);
                    prod.Imagen = uri.ToString();
                }
            }
        }


        public async Task<IActionResult> VistasDetalles(int idproducto, int? idproductoAñadir, int? idproductoAñadirFav)
        {
            CategoriasViewModel enlace = await this.service.VistasDetalles(idproducto, idproductoAñadir, idproductoAñadirFav);

            Producto prod = enlace.Producto;
            if (prod != null)
            {
                string blobname = prod.Imagen;
                if (blobname != null)
                {
                    BlobContainerClient blobcontainerclient = await this.serviceStorageBlobs.GetContainersAsync(this.containerName);
                    BlobClient blocclient = blobcontainerclient.GetBlobClient(blobname);

                    BlobSasBuilder sasbuilder = new BlobSasBuilder()
                    {
                        BlobContainerName = this.containerName,
                        BlobName = blobname,
                        Resource = "b",
                        StartsOn = DateTimeOffset.UtcNow,
                        ExpiresOn = DateTime.UtcNow.AddHours(1)
                    };
                    sasbuilder.SetPermissions(BlobSasPermissions.Read);
                    var uri = blocclient.GenerateSasUri(sasbuilder);
                    prod.Imagen = uri.ToString();
                }
            }

            return View(enlace);
        }


        [HttpPost]
        [AuthorizeClientes]
        public async Task<IActionResult> VistasDetalles(int idproducto, int cantidad, string accion)
        {
            await this.service.VistasDetalles(idproducto, cantidad, accion);
            return RedirectToAction("MisVistas");
        }

        [AuthorizeClientes]
        public async Task<IActionResult> Carrito(int? idproductoCarrito, int? ideliminar, int? eliminarTodo, int? cantidad)
        {
            List<Producto> carrito = await this.service.Carrito(idproductoCarrito, ideliminar, eliminarTodo, cantidad);

            if (carrito != null && carrito.Count > 0)
            {
                foreach (Producto prod in carrito)
                {
                    string blobname = prod.Imagen;
                    if (blobname != null)
                    {
                        BlobContainerClient blobcontainerclient = await this.serviceStorageBlobs.GetContainersAsync(this.containerName);
                        BlobClient blocclient = blobcontainerclient.GetBlobClient(blobname);

                        BlobSasBuilder sasbuilder = new BlobSasBuilder()
                        {
                            BlobContainerName = this.containerName,
                            BlobName = blobname,
                            Resource = "b",
                            StartsOn = DateTimeOffset.UtcNow,
                            ExpiresOn = DateTime.UtcNow.AddHours(1)
                        };
                        sasbuilder.SetPermissions(BlobSasPermissions.Read);
                        var uri = blocclient.GenerateSasUri(sasbuilder);
                        prod.Imagen = uri.ToString();
                    }
                }
            }
            if (carrito == null || carrito.Count == 0)
            {
                ViewBag.MensajeCarritoVacio = "No hay productos en el carrito";
            }
            else
            {
                ViewBag.MensajeCarritoVacio = null;
            }

            return View(carrito);
        }



        public async Task<IActionResult> MostrarPedidos(int idcliente)
        {
            List<DetallesPedido> pedidos = await this.service.MostrarPedidos(idcliente);
            return View(pedidos);
        }

        [HttpPost]
        public async Task<IActionResult> Pedidos()
        {
            await this.service.Pedidos();
            int idcliente = int.Parse(HttpContext.User.FindFirst("IdCliente").Value);
            return RedirectToAction("MostrarPedidos", new { idcliente = idcliente });
        }

        [AuthorizeClientes]
        public async Task<IActionResult> Favoritos(int? idproductoFav, int? ideliminar)
        {
            List<Producto> fav = await this.service.Favoritos(idproductoFav, ideliminar);
            if (fav != null && fav.Count > 0)
            {
                foreach (Producto prod in fav)
                {
                    string blobname = prod.Imagen;
                    if (blobname != null)
                    {
                        BlobContainerClient blobcontainerclient = await this.serviceStorageBlobs.GetContainersAsync(this.containerName);
                        BlobClient blocclient = blobcontainerclient.GetBlobClient(blobname);

                        BlobSasBuilder sasbuilder = new BlobSasBuilder()
                        {
                            BlobContainerName = this.containerName,
                            BlobName = blobname,
                            Resource = "b",
                            StartsOn = DateTimeOffset.UtcNow,
                            ExpiresOn = DateTime.UtcNow.AddHours(1)
                        };
                        sasbuilder.SetPermissions(BlobSasPermissions.Read);
                        var uri = blocclient.GenerateSasUri(sasbuilder);
                        prod.Imagen = uri.ToString();
                    }
                }
            }
            if (fav == null || fav.Count == 0)
            {
                ViewBag.MensajeCarritoVacio = "No hay productos favoritos";
            }
            else
            {
                ViewBag.MensajeCarritoVacio = null;
            }
            return View(fav);
        }

        [AuthorizeClientes(Policy = "AdminOnly")]
        public async Task<IActionResult> GetProductosAdmin()
        {
            List<Producto> productos = await this.service.GetProductosAdmin();
            return View(productos);
        }

        [AuthorizeClientes(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteProducto(int ididproducto)
        {
            Producto producto = await this.service.DeleteProducto(ididproducto);
            return View(producto);
        }

        [AuthorizeClientes(Policy = "AdminOnly")]
        public async Task<IActionResult> EliminarProducto(int ididproducto)
        {
            await this.service.EliminarProducto(ididproducto);
            return RedirectToAction("GetProductosAdmin");
        }

        [AuthorizeClientes(Policy = "AdminOnly")]
        public async Task<IActionResult> EditarProdAdmin(int idproducto)
        {
            Producto producto = await this.service.EditarProdAdmin(idproducto);
            return View(producto);
        }

        [HttpPost]
        [AuthorizeClientes(Policy = "AdminOnly")]
        public async Task<IActionResult> EditarProdAdmin(Producto producto)
        {
            await this.service.EditarProdAdmin(producto);
            return RedirectToAction("GetProductosAdmin");
        }
    }
}
