using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IServicioUsuarios servicioUsuarios;

        //Constructor que recibe o importa los servicios
        //Ctrl + . para asignarlos como un campo
        public CategoriasController(IRepositorioCategorias repositorioCategorias,
            IServicioUsuarios servicioUsuarios)
        {
            this.repositorioCategorias = repositorioCategorias;
            this.servicioUsuarios = servicioUsuarios;
        }

        //Aqui leemos la categoría, simplemente vemos la vista
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        //Aqui posteamos, mandamos info al servidor de crear una categoria
        [HttpPost]
        public async Task<IActionResult> Crear(CategoriaViewModel categoria)
        {
            //Aqui validamos nuestro modelo
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            //Para poder crear una categoria debemos saber a que usuario le vamos a crear la categoría
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            //Le pasamos el Id del usuario
            categoria.UsuarioId = usuarioId;
            
        }
    }
}
