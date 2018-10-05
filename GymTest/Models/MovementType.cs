using System.ComponentModel.DataAnnotations;

namespace GymTest.Models
{
    public class MovementType
    {
        [Required]
        public int MovementTypeId { get; set; }

        [Required(ErrorMessage = "Debe ingresar una descripción")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "La descripción debe ser entre 4 y 20 caracteres de largo")]
        [Display(Name = "Descripción")]
        public string Description { get; set; }

        public MovementType()
        {
        }
    }
}
