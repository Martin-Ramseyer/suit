using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace suitMvc.Models
{
    public class Flayer
    {
        [Key]
        public int flayer_id { get; set; }
        [Required]
        public int usuario_id { get; set; }
        [StringLength(255)]
        public string? imagen { get; set; }
        [Required]
        public DataSetDateTime fecha { get; set; }

        [ForeignKey("usuario_id")]
        public virtual Usuarios Usuarios { get; set; }




    }
}
