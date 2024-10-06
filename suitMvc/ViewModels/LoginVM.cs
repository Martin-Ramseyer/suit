using System.ComponentModel.DataAnnotations;

namespace suitMvc.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "El campo usuario es obligatorio.")]
        public required string usuario { get; set; }

        [Required(ErrorMessage = "El campo contraseña es obligatorio.")]
        [DataType(DataType.Password)]
        public required string contrasena { get; set; }

        public int admin { get; set; }
    }
}