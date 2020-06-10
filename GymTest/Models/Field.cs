using System;
using System.ComponentModel.DataAnnotations;

namespace GymTest.Models
{
    public class Field
    {
        [Required]
        public int FieldId { get; set; }

        [Required(ErrorMessage = "Campo descripción es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Descripción")]
        public string FieldDescription { get; set; }

        public Field()
        {
        }
    }
}
