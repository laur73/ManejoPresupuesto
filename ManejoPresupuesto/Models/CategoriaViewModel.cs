using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class CategoriaViewModel
    {
        //Aqui van los campos de la tabla y debemos declararlos tal cual incluso con su tipo de dato

        public int Id { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [StringLength(maximumLength:50, ErrorMessage ="No puede ser mayor a {1} caracteres")]
        public string Nombre { get; set; }
        //Si se ve feo el label lo cambiamos con el display
        [Display(Name ="Tipo Operación")]
        public TipoOperacion TipoOperacionId { get; set; }
        public int UsuarioId { get; set; }

    }
}
