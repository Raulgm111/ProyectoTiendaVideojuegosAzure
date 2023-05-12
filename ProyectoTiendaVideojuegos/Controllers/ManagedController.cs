using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;
using System.Numerics;
using NugetProyectoTinedaVideojuegosAzure;

namespace ProyectoTiendaVideojuegos.Controllers
{
    public class ManagedController : Controller
    {
        //private RepositoryUsuarios repo;

        //public ManagedController(RepositoryUsuarios repo)
        //{
        //    this.repo = repo;
        //}

        //public IActionResult Index()
        //{
        //    return View();
        //}

        //public IActionResult LogIn()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> LogIn
        //    (string username, string password)
        //{
        //    Cliente cliente =
        //        await this.repo.ExisteCliente(username, password);
        //    if (cliente != null)
        //    {
        //        ClaimsIdentity identity =
        //        new ClaimsIdentity
        //        (CookieAuthenticationDefaults.AuthenticationScheme
        //        , ClaimTypes.Name, ClaimTypes.Role);
        //        identity.AddClaim
        //        (new Claim(ClaimTypes.Name, cliente.Email));
        //        identity.AddClaim
        //        (new Claim(ClaimTypes.NameIdentifier, cliente.Contraseña.ToString()));
        //        if (cliente.IdCliente == 5)
        //        {
        //            identity.AddClaim(new Claim("Administrador", "Soy el admin"));
        //        }
        //        Claim claimNombre =
        //            new Claim("Nombre", cliente.Nombre.ToString());
        //        identity.AddClaim(claimNombre);
        //        Claim claimApellidos =
        //            new Claim("Apellidos", cliente.Apellidos.ToString());
        //        identity.AddClaim(claimApellidos);
        //        Claim claimIdCliente =
        //            new Claim("IdCliente", cliente.IdCliente.ToString());
        //        identity.AddClaim(claimIdCliente);
        //        //Claim claimImagen =
        //        //    new Claim("Imagen", cliente.Imagen.ToString());
        //        //identity.AddClaim(claimImagen);
        //        ClaimsPrincipal user = new ClaimsPrincipal(identity); 
        //        await HttpContext.SignInAsync
        //        (CookieAuthenticationDefaults.AuthenticationScheme
        //        , user);
        //        return RedirectToAction("MisVistas", "Productos");
        //    }
        //    else
        //    {
        //        ViewData["MENSAJE"] = "Usuario/Password incorrectos";
        //        return View();
        //    }
        //}

        //public IActionResult ErrorAcceso()
        //{
        //    return View();
        //}

        //public async Task<IActionResult> LogOut()
        //{
        //    await HttpContext.SignOutAsync
        //        (CookieAuthenticationDefaults.AuthenticationScheme);
        //    return RedirectToAction("MisVistas", "Productos");
        //}
    }
}
