﻿using Microsoft.AspNetCore.Mvc;
using ProyectoTiendaVideojuegosAzure.Services;

namespace ProyectoTiendaVideojuegos.Controllers
{
    public class UsuariosController : Controller
    {
        private ServiceApiProductos service;

        public UsuariosController(ServiceApiProductos service)
        {
            this.service = service;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string nombre, string apellidos, string email, string password, string imagen)
        {
            await this.service.RegisterAsync(nombre, apellidos, email, password, imagen);
            return RedirectToAction("MisVistas", "Productos");
        }

    }
}
