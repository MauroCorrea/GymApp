using System.ComponentModel.DataAnnotations;

namespace GymTest.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        [Required(ErrorMessage = "Campo descripción es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Descripción")]
        public string SupplierDescription { get; set; }
    }
}
