using System.ComponentModel.DataAnnotations;

namespace suitMvc.Models
{
    public class Usuarios
    {
        [Key]
        public int usuario_id { get; set; }
        [StringLength(50)]
        public string? nombre { get; set; }
        [StringLength(50)]
        public string? apellido { get; set; }
        [StringLength(50)]
        public string? usuario { get; set; }
        [StringLength(255)]
        public string? contrasena { get; set; }
        public int admin { get; set; } = 0;


    }
}
