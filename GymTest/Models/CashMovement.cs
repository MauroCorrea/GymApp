using System;
using System.ComponentModel.DataAnnotations;
namespace GymTest.Models
{

    public class CashMovement
    {
        public int CashMovementId { get; set; }

        [StringLength(200)]
        [Display(Name = "Detalles")]
        public string CashMovementDetails { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Monto")]
        [Required]
        public float? Amount { get; set; }

        [Display(Name = "Tipo")]
        [Required]
        public int CashMovementTypeId { get; set; }

        public virtual CashMovementType CashMovementType { get; set; }

        [Display(Name = "Categoría")]
        [Required]
        public int CashCategoryId { get; set; }

        public virtual CashCategory CashCategory { get; set; }

        public CashMovement()
        {
        }
    }
}
