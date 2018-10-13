using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymTest.Models
{
    public class Payment
    {
        [Required]
        public int PaymentId { get; set; }

        [Required(ErrorMessage = "La fecha de pago es obligatoria")]
        [Display(Name = "Fecha Pago")]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }

        [ForeignKey("MovmentType")]
        [Display(Name = "Tipo Pago")]
        public int MovementTypeId { get; set; }

        public virtual MovementType MovmentType { get; set; }

        [Required]
        [Display(Name = "Cant. Entradas")]
        public int QuantityMovmentType { get; set; }

        [Required(ErrorMessage = "Campo monto es obligatorio")]
        [DataType(DataType.Currency)]
        [Display(Name = "Monto")]
        [DisplayFormat(DataFormatString = "$ {0:n}")]
        public float? Amount { get; set; }

        [ForeignKey("User Id")]
        [Display(Name = "Usuario")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public Payment()
        {
        }
    }
}
