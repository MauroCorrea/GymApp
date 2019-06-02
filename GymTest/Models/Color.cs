using System.ComponentModel.DataAnnotations;

namespace GymTest.Models
{
    public class Color
    {
        public int ColorId { get; set; }

        [Required(ErrorMessage = "Campo descripción es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Descripción")]
        public string ColorDescription { get; set; }

        public Color()
        {
        }
    }
}
