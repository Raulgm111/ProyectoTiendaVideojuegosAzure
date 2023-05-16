using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using NugetProyectoTinedaVideojuegosAzure;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ProyectoTiendaVideojuegos.Extensions;
using ProyectoTiendaVideojuegos.Filters;
using Microsoft.CodeAnalysis;
using ProyectoTiendaVideojuegos.Helpers;
using System.Net.Http;

namespace ProyectoTiendaVideojuegosAzure.Services
{
    public class ServiceApiProductos
    {
        private MediaTypeWithQualityHeaderValue Header;
        private string UrlApiProductos;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ServiceApiProductos(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.UrlApiProductos =
                configuration.GetValue<string>("ApiUrls:Api0AuthTienda");
            this.Header =
                new MediaTypeWithQualityHeaderValue("application/json");
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetTokenAsync
    (string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/auth/login";
                client.BaseAddress = new Uri(this.UrlApiProductos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel
                {
                    UserName = username,
                    Password = password
                };
                string jsonModel = JsonConvert.SerializeObject(model);
                StringContent content =
                    new StringContent(jsonModel, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data =
                        await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(data);
                    string token =
                        jsonObject.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiProductos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        private async Task<T> CallApiAsync<T>
            (string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiProductos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public async Task<List<Producto>> BuscarProductos(string buscar)
        {
            string request = "/api/productos/GetBuscadorProductos/" + buscar;
            List<Producto> productos =
                await this.CallApiAsync<List<Producto>>(request);
            return productos;
        }

        #region VISTAS COMPLETAS

        public async Task<CategoriasViewModel> VistasGridTodosProductos(List<string> plataformas,
                List<string> generos, int? precioMinimo, int? precioMaximo, int? idproductoCarrito)
        {
            string requestCategorias = "/api/productos/GetCategorias";
            string requestSubCategorias = "/api/productos/GetSubCategorias";
            string requestFiltrarPlataforma = "/api/Filtros/FiltrarPorPlataforma";
            string requestFiltrarGenero = "/api/Filtros/FiltrarPorGenero";
            string requestTodosProductos = "/api/productos/GetTodosProductos";
            CategoriasViewModel enlace = new CategoriasViewModel();
            enlace.Categorias = await this.CallApiAsync<List<Categoria>>(requestCategorias);
            enlace.Subcategorias = await this.CallApiAsync<List<SubCategoria>>(requestSubCategorias);
            if (plataformas != null && plataformas.Any())
            {
                string url = $"{requestFiltrarPlataforma}?plataformas={string.Join("&plataformas=", plataformas)}";
                enlace.Productos = await this.CallApiAsync<List<Producto>>(url);
            }
            else if (generos != null && generos.Any())
            {
                string url = $"{requestFiltrarGenero}?generos={string.Join("&generos=", generos)}";
                enlace.Productos = await this.CallApiAsync<List<Producto>>(url);
            }
            else
            {
                enlace.Productos = await this.CallApiAsync<List<Producto>>(requestTodosProductos);
            }

            if (precioMinimo.HasValue && precioMaximo.HasValue)
            {
                enlace.Productos = enlace.Productos.Where(p => p.Precio >= precioMinimo.Value && p.Precio <= precioMaximo.Value).ToList();
            }
            return enlace;
        }

        public async Task<CategoriasViewModel> VistasGrid(int id, List<string> plataformas,
            List<string> generos, int? precioMinimo, int? precioMaximo)
        {
            string requestCategorias = "/api/productos/GetCategorias";
            string requestSubCategorias = "/api/productos/GetSubCategorias";
            string requestGetGrid = "/api/productos/GetPorductosGrid/" + id;
            string requestFiltrarPlataforma = "/api/Filtros/FiltrarPorPlataforma";
            string requestFiltrarGenero = "/api/Filtros/FiltrarPorGenero";
            CategoriasViewModel enlace = new CategoriasViewModel();
            enlace.Categorias = await this.CallApiAsync<List<Categoria>>(requestCategorias);
            enlace.Subcategorias = await this.CallApiAsync<List<SubCategoria>>(requestSubCategorias);
            enlace.Productos = await this.CallApiAsync<List<Producto>>(requestGetGrid);
            if (plataformas != null && plataformas.Any())
            {
                string url = $"{requestFiltrarPlataforma}?plataformas={string.Join("&plataformas=", plataformas)}";
                enlace.Productos = await this.CallApiAsync<List<Producto>>(url);
            }
            else if (generos != null && generos.Any())
            {
                string url = $"{requestFiltrarGenero}?generos={string.Join("&generos=", generos)}";
                enlace.Productos = await this.CallApiAsync<List<Producto>>(url);
            }

            if (precioMinimo.HasValue && precioMaximo.HasValue)
            {
                enlace.Productos = enlace.Productos.Where(p => p.Precio >= precioMinimo.Value && p.Precio <= precioMaximo.Value).ToList();
            }
            return enlace;
        }

        public async Task<CategoriasViewModel> MisVistas(string nombre)
        {
            string requestCategorias = "/api/productos/GetCategorias";
            string requestProductosPS4 = "/api/productos/GetProductosPS4";
            string requestProductosPS5 = "/api/productos/GetProductosPS5";
            string requestTazas = "/api/productos/GetTazas";
            string requestSubCategorias = "/api/productos/GetSubCategorias";
            CategoriasViewModel enlace = new CategoriasViewModel();
            enlace.Categorias = await this.CallApiAsync<List<Categoria>>(requestCategorias);
            enlace.ProductosPS4 = await this.CallApiAsync<List<Producto>>(requestProductosPS4);
            enlace.ProductosPS5 = await this.CallApiAsync<List<Producto>>(requestProductosPS5);
            enlace.Tazas = await this.CallApiAsync<List<Producto>>(requestTazas);
            enlace.Subcategorias = await this.CallApiAsync<List<SubCategoria>>(requestSubCategorias);
            return enlace;
        }


        public async Task<CategoriasViewModel> VistasDetalles(int idproducto, int? idproductoAñadir, int? idproductoAñadirFav)
        {
            string requestCategorias = "/api/productos/GetCategorias";
            string requestSubCategorias = "/api/productos/GetSubCategorias";
            string requestDetallesProducto = "/api/productos/DetallesProductos/" + idproducto;
            CategoriasViewModel enlace = new CategoriasViewModel();
            enlace.Categorias = await this.CallApiAsync<List<Categoria>>(requestCategorias);
            enlace.Subcategorias = await this.CallApiAsync<List<SubCategoria>>(requestSubCategorias);
            if (idproductoAñadir != null)
            {
                List<int> carrito;
                if (_httpContextAccessor.HttpContext.Session.GetObject<List<int>>("CARRITO") == null)
                {
                    carrito = new List<int>();
                }
                else
                {
                    carrito = _httpContextAccessor.HttpContext.Session.GetObject<List<int>>("CARRITO");
                }
                if (carrito.Contains(idproductoAñadir.Value) == false)
                {
                    carrito.Add(idproductoAñadir.Value);
                    _httpContextAccessor.HttpContext.Session.SetObject("CARRITO", carrito);
                }
            }

            if (idproductoAñadirFav != null)
            {
                List<int> favoritos;
                if (_httpContextAccessor.HttpContext.Session.GetObject<List<int>>("FAVORITO") == null)
                {
                    favoritos = new List<int>();
                }
                else
                {
                    favoritos = _httpContextAccessor.HttpContext.Session.GetObject<List<int>>("FAVORITO");
                }
                if (favoritos.Contains(idproductoAñadirFav.Value) == false)
                {
                    favoritos.Add(idproductoAñadirFav.Value);
                    _httpContextAccessor.HttpContext.Session.SetObject("FAVORITO", favoritos);
                }
            }
            enlace.Producto = await this.CallApiAsync<Producto>(requestDetallesProducto);
            return enlace;
        }
        [HttpPost]
        public async Task VistasDetalles(int idproducto, int cantidad, string accion)
        {
            if (accion == "AgregarCarrito")
            {
                List<int> carrito = _httpContextAccessor.HttpContext.Session.GetObject<List<int>>("CARRITO");
                if (carrito == null)
                {
                    carrito = new List<int>();
                }

                for (int i = 0; i < cantidad; i++)
                {
                    carrito.Add(idproducto);
                }

                _httpContextAccessor.HttpContext.Session.SetObject("CARRITO", carrito);
            }
            else if (accion == "AgregarFavoritos")
            {
                List<int> favoritos = _httpContextAccessor.HttpContext.Session.GetObject<List<int>>("FAVORITO");
                if (favoritos == null)
                {
                    favoritos = new List<int>();
                }

                for (int i = 0; i < cantidad; i++)
                {
                    favoritos.Add(idproducto);
                }

                _httpContextAccessor.HttpContext.Session.SetObject("FAVORITO", favoritos);
            }
        }
        #endregion

        #region CARRITO

        public async Task<List<Producto>> Carrito(int? idproductoCarrito, int? ideliminar, int? eliminarTodo, int? cantidad)
        {
            List<int> carrito = _httpContextAccessor.HttpContext.Session.GetObject<List<int>>("CARRITO");
            if (carrito == null)
            {
                return null;
            }
            else
            {
                if (eliminarTodo != null)
                {
                    carrito.RemoveAll(id => id == eliminarTodo.Value);
                    if (carrito.Count == 0)
                    {
                        _httpContextAccessor.HttpContext.Session.Remove("CARRITO");
                    }
                    else
                    {
                        _httpContextAccessor.HttpContext.Session.SetObject("CARRITO", carrito);
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
                        _httpContextAccessor.HttpContext.Session.Remove("CARRITO");
                    }
                    else
                    {
                        _httpContextAccessor.HttpContext.Session.SetObject("CARRITO", carrito);
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

                    _httpContextAccessor.HttpContext.Session.SetObject("CARRITO", carrito);
                }

                string url = $"/api/productos/BuscarProductoCarrito?idproductoCarrito={string.Join("&idproductoCarrito=", carrito)}";
                List<Producto> productos = await this.CallApiAsync<List<Producto>>(url);
                return productos;
            }
        }


        #endregion

        public async Task<List<DetallesPedido>> MostrarPedidos(int idcliente)
        {
            string request = "/api/pedidos/MostrarPedidos/" + idcliente;
            List<DetallesPedido> pedidos = await this.CallApiAsync<List<DetallesPedido>>(request);
            return pedidos;
        }

        public async Task Pedidos()
        {
            using (HttpClient client = new HttpClient())
            {
                // Establecer la dirección base de la API
                client.BaseAddress = new Uri("https://apioauthproyectotiendavideojuegosrgm.azurewebsites.net/");

                List<int> carrito = _httpContextAccessor.HttpContext.Session.GetObject<List<int>>("CARRITO");
                int idCliente = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst("IdCliente").Value);

                string requestBuscarProducto = $"/api/productos/BuscarProductoCarrito?idproductoCarrito={string.Join("&idproductoCarrito=", carrito)}";
                List<Producto> productos = await this.CallApiAsync<List<Producto>>(requestBuscarProducto);

                int precioTotal = productos.Sum(prod => prod.Precio);

                string requestAgregarPedido = $"/api/pedidos/AgregarPedido/{idCliente}/{precioTotal}";

                // Crear una lista de objetos anónimos con los valores necesarios
                var productosNuevos = productos.Select(prod => new Producto
                {
                    IdProducto = prod.IdProducto,
                    IdCategoria = prod.IdCategoria,
                    IdSubCategoria = prod.IdSubCategoria,
                    NombreProducto = prod.NombreProducto,
                    Lanzamiento = prod.Lanzamiento,
                    Imagen = prod.Imagen,
                    Precio = prod.Precio,
                    Descripcion = prod.Descripcion,
                    Genero = prod.Genero
                }).ToList();

                // Convertir la lista de productos a formato JSON
                string jsonCubo = JsonConvert.SerializeObject(productosNuevos);

                string url = $"{requestAgregarPedido}?productos={jsonCubo}&cantidad={carrito.Count}";
                HttpContent content = new StringContent(jsonCubo, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);


                _httpContextAccessor.HttpContext.Session.Remove("CARRITO");
            }
        }


        public async Task<List<Producto>> Favoritos(int? idproductoFav, int? ideliminar)
        {
            string requestBuscarProducto = "/api/productos/BuscarProductoFavorito";
            List<int> favoritos = _httpContextAccessor.HttpContext.Session.GetObject<List<int>>("FAVORITO");
            if (favoritos == null)
            {
                return null;
            }
            else
            {
                if (ideliminar != null)
                {
                    favoritos.Remove(ideliminar.Value);
                    if (favoritos.Count == 0)
                    {
                        _httpContextAccessor.HttpContext.Session.Remove("FAVORITO");
                    }
                    else
                    {
                        if (idproductoFav != null)
                        {
                            favoritos.Remove(idproductoFav.Value);
                            _httpContextAccessor.HttpContext.Session.SetObject("FAVORITO", favoritos);
                        }
                    }
                }
                string url = $"/api/productos/BuscarProductoFavorito?idproductoFav={string.Join("&idproductoFav=", favoritos)}";
                List<Producto> productos = await this.CallApiAsync<List<Producto>>(url);
                return productos;

            }
        }

        public async Task <List<Producto>> GetProductosAdmin()
        {
            string request = "/api/productos/GetTodosProductos";
            List<Producto> productos = await this.CallApiAsync<List<Producto>>(request);
            return productos;
        }

        public async Task<Producto> DeleteProducto(int ididproducto)
        {
            string request = "/api/productos/DetallesProductos/"+ ididproducto;
            Producto producto = await this.CallApiAsync<Producto>(request);
            return producto;
        }
        public async Task EliminarProducto(int ididproducto)
        {
            string request = "/api/productos/DeleteProductos/" + ididproducto;
            await this.CallApiAsync<Producto>(request);
        }

        public async Task<Producto> EditarProdAdmin(int idproducto)
        {
            string request = "/api/productos/DetallesProductos/" + idproducto;
            Producto producto = await this.CallApiAsync<Producto>(request);
            return producto;
        }

        public async Task EditarProdAdmin(Producto producto)
        {
            string request = "/api/productos/UpdatePorducto";
            await this.CallApiAsync<Producto>(request);
        }
        public async Task RegisterAsync(string nombre, string apellidos, string email, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/auth/register/" + nombre + "/" + apellidos + "/" + email + "/" + password;
                client.BaseAddress = new Uri(this.UrlApiProductos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                string salt = HelperCryptography.GenerateSalt();

                Cliente user = new Cliente();
                    user.Nombre = nombre;
                    user.Apellidos = apellidos;
                    user.Email = email;
                    user.Contraseña = HelperCryptography.EncryptPassword(password, salt);
                string jsonUser =
                    JsonConvert.SerializeObject(user);
                StringContent content =
                    new StringContent(jsonUser, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
                if (!response.IsSuccessStatusCode)
                {
                    // handle error
                    throw new Exception("Failed to register user.");
                }
            }
        }

        public async Task<Cliente> GetPerfilUsuarioAsync
        (string token)
        {
            string request = "/api/auth/PerfilUsuario";
            Cliente usuario = await
                this.CallApiAsync<Cliente>(request, token);
            return usuario;
        }


    }
}
