using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El campo debe ser un correo electrónico válido")]
        public string Email { get; set; }
        [StringLength(maximumLength: 18)]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Password { get; set; }
        [Display(Name ="Recuérdame")]
        public bool Recuerdame { get; set; }
    }
}
