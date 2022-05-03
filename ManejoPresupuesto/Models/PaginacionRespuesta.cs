namespace ManejoPresupuesto.Models
{
    public class PaginacionRespuesta
    {
        public int Pagina { get; set; } = 1;
        public int RegistrosPorPagina { get; set; } = 10;
        public int CantidadTotalRegistros { get; set; }
        public int CantiadTotalDePaginas => (int)Math.Ceiling((double)CantidadTotalRegistros / RegistrosPorPagina);
        public string BaseURL { get; set; }
    }

    public class PaginacionRespuesta<T>: PaginacionRespuesta
    {
        public IEnumerable<T> Elementos { get; set; }
    }
}
