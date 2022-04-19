using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTransacciones
    {
        Task Actualizar(TransaccionViewModel transaccion, decimal montoAnterior, int cuentaAnterior);
        Task Borrar(int id);
        Task Crear(TransaccionViewModel transaccion);
        Task<IEnumerable<TransaccionViewModel>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo);
        Task<TransaccionViewModel> ObtenerPorId(int id, int usuarioId);
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo);
        Task<IEnumerable<TransaccionViewModel>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo);
    }

    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        private readonly string connectionString;

        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(TransaccionViewModel transaccion)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("Transacciones_Insertar",
                new
                {
                    transaccion.UsuarioId,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota
                },
                commandType: System.Data.CommandType.StoredProcedure);
            transaccion.Id = id;
        }

        public async Task<IEnumerable<TransaccionViewModel>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TransaccionViewModel>(
                @"SELECT t.Id, t.Monto, t.FechaTransaccion, c.Nombre AS Categoria, cu.Nombre as Cuenta, c.TipoOperacionId
                FROM Transacciones t
                INNER Join Categorias c
                ON c.Id = t.CategoriaId
                INNER JOIN Cuentas cu
                ON cu.Id = t.CuentaId
                WHERE t.CuentaId =  @CuentaId AND t.UsuarioId = @UsuarioId
                AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin", modelo);
        }

        public async Task<IEnumerable<TransaccionViewModel>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TransaccionViewModel>(
                @"SELECT t.Id, t.Monto, t.FechaTransaccion, c.Nombre AS Categoria, cu.Nombre as Cuenta, c.TipoOperacionId
                FROM Transacciones t
                INNER Join Categorias c
                ON c.Id = t.CategoriaId
                INNER JOIN Cuentas cu
                ON cu.Id = t.CuentaId
                WHERE t.UsuarioId = @UsuarioId
                AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin
                ORDER BY t.FechaTransaccion DESC", modelo);
        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultadoObtenerPorSemana>(@"
                    SELECT DATEDIFF(d, @fechaInicio, FechaTransaccion) / 7 + 1 as Semana,
                    SUM(Monto) AS Monto, cat.TipoOperacionId
                    FROM Transacciones
                    INNER JOIN Categorias cat
                    ON cat.Id = Transacciones.CategoriaId
                    WHERE Transacciones.UsuarioId = @usuarioId AND
                    FechaTransaccion BETWEEN @fechaInicio AND @fechaFin
                    GROUP BY DATEDIFF(d, @fechaInicio, FechaTransaccion) / 7, cat.TipoOperacionId", modelo);
        }

        public async Task Actualizar(TransaccionViewModel transaccion, decimal montoAnterior, int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Actualizar",
                new
                {
                    transaccion.Id,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota,
                    montoAnterior,
                    cuentaAnteriorId
                }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<TransaccionViewModel> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TransaccionViewModel>(
                                                                        @"SELECT Transacciones.*, cat.TipoOperacionId
                                                                        FROM Transacciones
                                                                        INNER JOIN Categorias cat
                                                                        ON cat.Id = Transacciones.CategoriaId
                                                                        WHERE Transacciones.Id = @Id AND Transacciones.UsuarioId = @UsuarioId",
                                                                        new { id, usuarioId });
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Borrar",
                new { id }, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
