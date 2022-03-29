using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

//Esta clase contiene los metodos de Crear, Leer, Actualizar y Borrar un tipo de cuenta

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuentaViewModel tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuentaViewModel tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuentaViewModel>> Obtener(int usuarioId);
        Task<TipoCuentaViewModel> ObtenerPorId(int id, int usuarioId);
        Task Ordenar(IEnumerable<TipoCuentaViewModel> tipoCuentasOrdenados);
    }

    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string connectionString;

        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //Metodo para poder crear un tipo cuenta en la BD

        public async Task Crear(TipoCuentaViewModel tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"TiposCuentas_Insertar",
                                                           new { usuarioId = tipoCuenta.UsuarioId, nombre = tipoCuenta.Nombre },
                                                           commandType: System.Data.CommandType.StoredProcedure);
            tipoCuenta.Id = id;
        }

        //Metodo para saber si existe un registro de tipos cuentas para no repetirlo

        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1
                                                    FROM TiposCuentas
                                                    WHERE Nombre = @Nombre AND UsuarioId = @UsuarioId;", new { nombre, usuarioId });

            return existe == 1;
        }

        //Metodo para listar los tipos de cuentas creados

        public async Task<IEnumerable<TipoCuentaViewModel>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuentaViewModel>(@"SELECT Id, Nombre, Orden
                                                    FROM TiposCuentas
                                                    WHERE UsuarioId = @usuarioId
                                                    ORDER BY Orden;", new { usuarioId });
        }

        //Método para editar un tipo cuenta
        public async Task Actualizar(TipoCuentaViewModel tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE TiposCuentas
                                            SET Nombre = @Nombre
                                            WHERE Id = @Id;", tipoCuenta);
        }

        //Método para obtener tipo cuenta por Id
        public async Task<TipoCuentaViewModel> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuentaViewModel>(@"SELECT Id, Nombre, Orden
                                                    FROM TiposCuentas
                                                    WHERE Id = @Id AND UsuarioId = @UsuarioId;", new { id, usuarioId });
        }

        //Método para borrar un tipo cuenta
        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE TiposCuentas WHERE Id = @Id;", new { id });
        }

        //Método para ordenar los tipos cuentas
        public async Task Ordenar(IEnumerable<TipoCuentaViewModel> tipoCuentasOrdenados)
        {
            var query = "UPDATE TiposCuentas SET Orden = @Orden WHERE Id = @Id;";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, tipoCuentasOrdenados);
        }
    }
}
