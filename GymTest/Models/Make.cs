using System.ComponentModel.DataAnnotations;

namespace GymTest.Models
{
    public class Make
    {
        public int MakeId { get; set; }

        [Required(ErrorMessage = "Campo descripción es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Descripción")]
        public string MakeDescription { get; set; }

        public Make()
        {
        }
    }
}
