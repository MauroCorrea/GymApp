using System.ComponentModel.DataAnnotations;
namespace GymTest.Models
{
    public class PaymentMedia
    {
        public int PaymentMediaId { get; set; }

        [Required(ErrorMessage = "Campo descripción es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Descripción")]
        public string PaymentMediaDescription { get; set; }
    }
}
