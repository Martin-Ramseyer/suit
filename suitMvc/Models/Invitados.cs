using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace suitMvc.Models
{
    public class Invitados
    {
        [Key]
        public int invitado_id { get; set; }
        [Required]
        public int usuario_id { get; set; }
        [Required]
        [StringLength(50)]
        public string? nombre { get; set; }
        [Required]
        [StringLength(50)]
        public string? apellido { get; set; }
        [Required]
        public int acompanantes { get; set; }
        public int entrada_free { get; set; }
        public int consumiciones { get; set; }
        public int pulsera { get; set; }
        public int paso { get; set; } = 0;

        [ForeignKey("usuario_id")]
        public virtual required Usuarios Usuarios { get; set; }
    }
}
