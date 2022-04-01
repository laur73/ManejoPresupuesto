using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    //Creamos la interfaz
    public interface IRepositorioCategorias
    {
        //Ctrl + . para empujar los metodos a la interfaz (esta)
        Task Crear(CategoriaViewModel categoria);
    }
    //Heredamos de la interfaz
    public class RepositorioCategorias: IRepositorioCategorias
    {
        //Para que solo lea la cadena de conexion
        private readonly string connectionString;

        //Para que tome la cadena de configuracion para conectarse a la BD
        public RepositorioCategorias(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConfiguration");
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
    }
}
