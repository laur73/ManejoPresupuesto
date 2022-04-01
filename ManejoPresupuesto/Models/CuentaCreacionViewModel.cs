using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Models
{
    public class CuentaCreacionViewModel: CuentaViewModel
    {
        public IEnumerable<SelectListItem> TiposCuentas { get; set; }
    }
}
