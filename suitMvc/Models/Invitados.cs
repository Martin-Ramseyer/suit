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

        [ForeignKey("usuario_id")]
        public virtual Usuarios Usuarios { get; set; }
    }
}
