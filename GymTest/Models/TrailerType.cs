using System.ComponentModel.DataAnnotations;

namespace GymTest.Models
{
    public class TrailerType
    {
        public int TrailerTypeId { get; set; }

        [Required(ErrorMessage = "Campo descripción es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Descripción")]
        public string TrailerTypeDescription { get; set; }

        public TrailerType()
        {
        }
    }
}
