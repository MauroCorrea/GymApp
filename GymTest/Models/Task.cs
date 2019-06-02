using System.ComponentModel.DataAnnotations;

namespace GymTest.Models
{
    public class Task
    {
        public int TaskId { get; set; }

        [Required(ErrorMessage = "Campo descripción es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Descripción")]
        public string TaskDescription { get; set; }

        public Task()
        {
        }
    }
}
