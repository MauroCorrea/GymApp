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
        [DisplayFormat(DataFormatString = "$ {0:n}")]
        public float? Amount { get; set; }

        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "Campo Tipo es obligatorio")]
        public int CashMovementTypeId { get; set; }

        [Required(ErrorMessage = "La fecha de movimiento es obligatoria")]
        [Display(Name = "Fecha de movimiento")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime CashMovementDate { get; set; }

        [Display(Name = "Tipo")]
        public virtual CashMovementType CashMovementType { get; set; }

        [Display(Name = "Categoría")]
        [Required(ErrorMessage = "Campo Categoría es obligatorio")]
        public int CashCategoryId { get; set; }

        [Display(Name = "Categoría")]
        public virtual CashCategory CashCategory { get; set; }


        [Display(Name = "Proveedor")]
        [Required(ErrorMessage = "Campo Proveedor es obligatorio")]
        public int SupplierId { get; set; }

        [Display(Name = "Proveedor")]
        public virtual Supplier Supplier { get; set; }

        public CashMovement()
        {
        }
    }
}
