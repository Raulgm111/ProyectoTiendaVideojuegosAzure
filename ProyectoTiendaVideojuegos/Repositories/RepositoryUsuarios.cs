using Microsoft.EntityFrameworkCore;
using NugetProyectoTinedaVideojuegosAzure;
using ProyectoTiendaVideojuegos.Data;
using ProyectoTiendaVideojuegos.Helpers;

namespace ProyectoTiendaVideojuegos.Repositories
{
    public class RepositoryUsuarios
    {
        private UsuariosContext context;

        public RepositoryUsuarios(UsuariosContext context)
        {
            this.context = context;
        }
        private int GetMaxIdUsuario()
        {
            if (this.context.Clientes.Count() == 0)
            {
                return 1;
            }
            else
            {
                return this.context.Clientes.Max(z => z.IdCliente) + 1;
            }
        }

        public async Task RegisterUsuario(string nombre, string apellidos,string email, string password, string imagen)
        {
            Cliente user = new Cliente();
            user.IdCliente = this.GetMaxIdUsuario();
            user.Nombre = nombre;
            user.Apellidos = apellidos;
            user.Email = email;
            user.Imagen = imagen;
            user.Salt = HelperCryptography.GenerateSalt();
            user.Contraseña = HelperCryptography.EncryptPassword(password, user.Salt);
            this.context.Clientes.Add(user);
            await this.context.SaveChangesAsync();
        }


        public async Task<Cliente> ExisteCliente
            (string username, string password)
        {
            Cliente cliente = await this.FindEmailAsync(username);
            var usuario = await this.context.Clientes.Where(x => x.Email == username && x.Contraseña == HelperCryptography.EncryptPassword(password, cliente.Salt)).FirstOrDefaultAsync();
            return usuario;
        }

        public async Task<Cliente> FindEmailAsync(string username)
        {
            Cliente usuario =
            await this.context.Clientes.Where
            (x => x.Email == username).FirstOrDefaultAsync();
            return usuario;
        }
    }
}
