using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymTest.Models
{
    public class Model
    {
        public int ModelId { get; set; }

        [Required(ErrorMessage = "Campo descripción es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Descripción")]
        public string ModelDescription { get; set; }

        [Required(ErrorMessage = "Campo marca es obligatorio")]
        [ForeignKey("MakeId")]
        [Display(Name = "Marca")]
        public int MakeId { get; set; }

        public virtual Make Make { get; set; }

        public Model()
        {
        }
    }
}
