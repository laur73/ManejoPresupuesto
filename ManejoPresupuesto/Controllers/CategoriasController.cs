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

        //Método para listar las categorias
        public async Task<IActionResult> Index(PaginacionViewModel paginacionViewModel)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var categorias = await repositorioCategorias.Obtener(usuarioId, paginacionViewModel);
            var totalCategorias = await repositorioCategorias.Contar(usuarioId);

            var respuestaVM = new PaginacionRespuesta<CategoriaViewModel>
            {
                Elementos = categorias,
                Pagina = paginacionViewModel.Pagina,
                RegistrosPorPagina = paginacionViewModel.RegistrosPorPagina,
                CantidadTotalRegistros = totalCategorias,
                BaseURL = Url.Action()
            };
            return View(respuestaVM);
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
            //Le pasamos el Id del usuario (que por el momento es por defecto el 1, luego lo cambiaremos)
            categoria.UsuarioId = usuarioId;

            //Crear recibe un modelo de categoria
            await repositorioCategorias.Crear(categoria);

            //Lo redirigimos a donde estan sus categorías
            return RedirectToAction("Index");

        }

        //Acción para obtener el id del usuario para poder editar su registro
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var categoria = await repositorioCategorias.ObtenerPorId(id, usuarioId);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(categoria);
        }

        //Acción para mandar info a la BD cuando se edita
        [HttpPost]
        public async Task<IActionResult>Editar(CategoriaViewModel categoriaEditar)
        {
            //Aqui validamos nuestro modelo
            if (!ModelState.IsValid)
            {
                return View(categoriaEditar);
            }

            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var categoria = await repositorioCategorias.ObtenerPorId(categoriaEditar.Id, usuarioId);

            //Preguntamos si hay una categoría
            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            categoriaEditar.UsuarioId = usuarioId;
            await repositorioCategorias.Actualizar(categoriaEditar);
            return RedirectToAction("Index");
        }

        //Accion para la vista de Borrar
        public async Task<IActionResult> Borrar (int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var categoria = await repositorioCategorias.ObtenerPorId(id, usuarioId);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(categoria);
        }

        //Accion para postear el Borrar

        [HttpPost]
        public async Task<IActionResult>BorrarCategoria(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var categoria = await repositorioCategorias.ObtenerPorId(id, usuarioId);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioCategorias.Borrar(id);
            return RedirectToAction("Index");
        }
    }
}
