using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace GymTest.Models
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class Payment
    {
        [Required]
        public int PaymentId { get; set; }

        [Required(ErrorMessage = "La fecha de pago es obligatoria")]
        [Display(Name = "Fecha Pago")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }


        [Required(ErrorMessage = "La fecha límite de uso es obligatoria")]
        [Display(Name = "Fecha límite")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime LimitUsableDate { get; set; }

        [ForeignKey("MovmentType")]
        [Display(Name = "Tipo Pago")]
        public int MovementTypeId { get; set; }

        [Display(Name = "Tipo Pago")]
        public virtual MovementType MovmentType { get; set; }

        [Display(Name = "Medio de Pago")]
        [Required(ErrorMessage = "Campo Tipo es obligatorio")]
        public int PaymentMediaId { get; set; }

        [Display(Name = "Medio de Pago")]
        public virtual PaymentMedia PaymentMedia { get; set; }

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
