using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;
using System.Numerics;
using NugetProyectoTinedaVideojuegosAzure;
using ProyectoTiendaVideojuegosAzure.Services;

namespace ProyectoTiendaVideojuegos.Controllers
{
    public class ManagedController : Controller
    {
        private ServiceApiProductos service;
        public ManagedController(ServiceApiProductos service)
        {
            this.service = service;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login
        (string username, string password)
        {
            string token =
            await this.service.GetTokenAsync(username, password);
            if (token == null)
            {
                ViewData["MENSAJE"] = "Usuario/Password incorrectas";
                return View();
            }
            else
            {
                Cliente cliente =
                    await this.service.GetPerfilUsuarioAsync(token);
                HttpContext.Session.SetString("TOKEN", token);
                ClaimsIdentity identity =
                new ClaimsIdentity
                (CookieAuthenticationDefaults.AuthenticationScheme
                , ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim
                (new Claim(ClaimTypes.Name, cliente.Email));
                identity.AddClaim
                (new Claim(ClaimTypes.NameIdentifier, cliente.Contraseña.ToString()));
                if (cliente.IdCliente == 5)
                {
                    identity.AddClaim(new Claim("Administrador", "Soy el admin"));
                }
                Claim claimNombre =
                    new Claim("Nombre", cliente.Nombre.ToString());
                identity.AddClaim(claimNombre);
                Claim claimApellidos =
                    new Claim("Apellidos", cliente.Apellidos.ToString());
                identity.AddClaim(claimApellidos);
                Claim claimIdCliente =
                    new Claim("IdCliente", cliente.IdCliente.ToString());
                identity.AddClaim(claimIdCliente);
                //Claim claimImagen =
                //    new Claim("Imagen", cliente.Imagen.ToString());
                //identity.AddClaim(claimImagen);
                ClaimsPrincipal user = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync
                (CookieAuthenticationDefaults.AuthenticationScheme
                , user);
                return RedirectToAction("MisVistas", "Productos");
            }
        }

        public async Task<IActionResult> Perfil()
        {
            string token = HttpContext.Session.GetString("TOKEN");
            Cliente usuario = await this.service.GetPerfilUsuarioAsync(token);
            return View(usuario);
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync
            (CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("TOKEN");
            return RedirectToAction("MisVistas", "Productos");
        } 
    }
}
