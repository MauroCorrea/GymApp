using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymTest.Models
{
    public class Discipline
    {

        public Discipline() { }

        [Required]
        public int DisciplineId { get; set; }

        [Required(ErrorMessage = "Campo descripción es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Descripción")]
        public string DisciplineDescription { get; set; }

        [Required(ErrorMessage = "Campo Recurso es obligatorio")]
        [ForeignKey("ResourceId")]
        [Display(Name = "Recurso")]
        public int ResourceId { get; set; }

        [Display(Name = "Profe")]
        public virtual Resource Resource { get; set; }
    }
}
