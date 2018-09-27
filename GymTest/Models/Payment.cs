using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymTest.Models
{
    public class Payment
    {
        [Required]
        public int PaymentId { get; set; }

        [Display(Name = "Fecha pago")]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Monto")]
        [Required]
        public float? Amount { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public Payment()
        {
        }
    }
}
