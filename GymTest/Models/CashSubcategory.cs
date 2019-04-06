using System.ComponentModel.DataAnnotations;

namespace GymTest.Models
{
    public class CashSubcategory
    {
        public int CashSubcategoryId { get; set; }
        [Required(ErrorMessage = "Campo descripción es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Descripción")]
        public string CashSubcategoryDescription { get; set; }

        [Display(Name = "Categoría")]
        [Required(ErrorMessage = "Campo Categoría es obligatorio")]
        public int CashCategoryId { get; set; }

        [Display(Name = "Categoría")]
        public virtual CashCategory CashCategory { get; set; }

        public CashSubcategory()
        {
        }
    }
}
