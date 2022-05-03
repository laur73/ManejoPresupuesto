using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    //Creamos la interfaz
    public interface IRepositorioCategorias
    {
        Task Actualizar(CategoriaViewModel categoria);
        Task Borrar(int id);
        Task<int> Contar(int usuarioId);

        //Ctrl + . para empujar los metodos a la interfaz (esta)
        Task Crear(CategoriaViewModel categoria);
        Task<IEnumerable<CategoriaViewModel>> Obtener(int usuarioId, PaginacionViewModel paginacion);
        Task<IEnumerable<CategoriaViewModel>> Obtener(int usuarioId, TipoOperacion tipoOperacionId);
        Task<CategoriaViewModel> ObtenerPorId(int id, int usuarioId);
    }
    //Heredamos de la interfaz
    public class RepositorioCategorias : IRepositorioCategorias
    {
        //Para que solo lea la cadena de conexion
        private readonly string connectionString;

        //Para que tome la cadena de configuracion para conectarse a la BD
        public RepositorioCategorias(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //Metodo para crear una categoria
        public async Task Crear(CategoriaViewModel categoria)
        {
            //Cadena de conexion
            using var connection = new SqlConnection(connectionString);
            //Query
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Categorias(Nombre, TipoOperacionId, UsuarioId)
                                                            VALUES (@Nombre, @TipoOperacionId, @UsuarioId);
                                                            SELECT SCOPE_IDENTITY();", categoria);
            categoria.Id = id;
        }

        //Metodo para listar las categorias
        public async Task<IEnumerable<CategoriaViewModel>> Obtener(int usuarioId, PaginacionViewModel paginacion)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<CategoriaViewModel>(
                @$"SELECT * FROM Categorias WHERE UsuarioId=@usuarioId
                ORDER BY Nombre
                OFFSET {paginacion.RegistrosASaltar} ROWS FETCH NEXT {paginacion.RegistrosPorPagina}
                ROWS ONLY", new { usuarioId });
        }

        public async Task<int> Contar (int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Categorias WHERE UsuarioId = @usuarioId", new { usuarioId });
        }

        //Metodo para listar las categorias pero con el tipo operacion
        public async Task<IEnumerable<CategoriaViewModel>> Obtener(int usuarioId, TipoOperacion tipoOperacionId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<CategoriaViewModel>(
                @"SELECT * FROM Categorias 
                WHERE UsuarioId=@usuarioId AND TipoOperacionId=@tipoOperacionId", new { usuarioId, tipoOperacionId });
        }

        //Metodos para actualizar (editar)

        //Primero uno para obtener el id del registro y del usuario para validar que el registro que
        //queremos editar corresponda al del usuario y no de otro

        //Actualización a abril: como ya entendi que chingados con el CRUD, básicamente para editar y eliminar necesitamos
        //primero saber el Id del registro a editar/eliminar
        public async Task<CategoriaViewModel> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<CategoriaViewModel>(
                                    @"SELECT * FROM Categorias WHERE Id = @Id AND UsuarioId = @UsuarioId",
                                    new { id, usuarioId });
        }

        //Segundo metodo que es para editar/actualizar
        public async Task Actualizar(CategoriaViewModel categoria)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Categorias SET Nombre = @Nombre, TipoOperacionId = @TipoOperacionId
                                            WHERE Id = @Id", categoria);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE Categorias WHERE Id = @Id", new { id });
        }
    }
}
