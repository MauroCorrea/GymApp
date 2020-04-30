using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace GymTest.Models
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class Discipline
    {

        public Discipline() { }

        [Required]
        public int DisciplineId { get; set; }

        [Required(ErrorMessage = "Campo descripción es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Descripción")]
        public string DisciplineDescription { get; set; }
    }
}
