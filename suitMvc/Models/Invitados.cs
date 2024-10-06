using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace suitMvc.Models
{
    public class Invitados
    {
        [Key]
        public int invitado_id { get; set; }

        [Required(ErrorMessage = "El campo usuario_id es obligatorio.")]
        public int usuario_id { get; set; }

        [Required(ErrorMessage = "El campo nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
        public string? nombre { get; set; }

        [Required(ErrorMessage = "El campo apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido no puede tener más de 50 caracteres.")]
        public string? apellido { get; set; }

        [Required(ErrorMessage = "El campo acompañantes es obligatorio.")]
        public int acompanantes { get; set; }

        public int entrada_free { get; set; }
        public int consumiciones { get; set; }
        public int pulsera { get; set; }
        public int paso { get; set; } = 0;

        [ForeignKey("usuario_id")]
        public virtual required Usuarios Usuarios { get; set; }
    }
}