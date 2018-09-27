using System;
using System.ComponentModel.DataAnnotations;
namespace GymTest.Models
{
    public class CashMovementType
    {
        public int CashMovementTypeId { get; set; }

        [Required(ErrorMessage = "Campo descripción es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Descripción")]
        public string CashMovementTypeDescription { get; set; }

        public CashMovementType()
        {
        }
    }
}
