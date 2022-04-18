using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class TransaccionCreacionViewModel : TransaccionViewModel
    {
        //La queremos solo para agregar dos IEnumerables para los select
        public IEnumerable<SelectListItem> Cuentas { get; set; }
        public IEnumerable<SelectListItem> Categorias { get; set; }

    }
}
