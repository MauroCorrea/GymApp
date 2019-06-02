using System.ComponentModel.DataAnnotations;

namespace GymTest.Models
{
    public class Activity
    {
        public int ActivityId { get; set; }

        [Required(ErrorMessage = "Campo descripción es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Descripción")]
        public string ActivityDescription { get; set; }

        public Activity()
        {
        }
    }
}
