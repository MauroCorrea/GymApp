using System.ComponentModel.DataAnnotations;

namespace GymTest.Models
{
    public class Role
    {
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Campo descripción es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Descripción")]
        public string RoleDescription { get; set; }

        public Role()
        {
        }
    }
}
