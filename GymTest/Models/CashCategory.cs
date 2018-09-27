using System;
using System.ComponentModel.DataAnnotations;
namespace GymTest.Models
{
    public class CashCategory
    {
        public int CashCategoryId { get; set; }
        [Required(ErrorMessage = "Campo descripción es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Descripción")]
        public string CashCategoryDescription { get; set; }
    }
}
