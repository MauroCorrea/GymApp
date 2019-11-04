using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymTest.Models
{
    public class UserReport
    {
        public int UserReportId { get; set; }

        [Display(Name = "Ingreso Desde")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? SignInDateFrom { get; set; }

        [Display(Name = "Ingreso Hasta")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? SignInDateTo { get; set; }

        [Display(Name = "Asistencias Desde")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? AssitanceFrom { get; set; }

        [Display(Name = "Asistencias Hasta")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? AssitanceTo { get; set; }

        [Display(Name = "Edad Desde")]
        public int? AgeFrom { get; set; }

        [Display(Name = "Edad Hasta")]
        public int? AgeTo { get; set; }

        [Display(Name = "Entradas Desde")]
        public int? AssitanceCountFrom { get; set; }

        [Display(Name = "Entradas Hasta")]
        public int? AssitanceCountTo { get; set; }




        [Display(Name = "Pago Desde")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? PayDateFrom { get; set; }

        [Display(Name = "Pago Hasta")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? PayDateTo { get; set; }

        [ForeignKey("MovmentType")]
        [Display(Name = "Tipo Pago")]
        public int? MovementTypeId { get; set; }

        [Display(Name = "Tipo Pago")]
        public virtual MovementType MovmentType { get; set; }

        [Display(Name = "Medio de Pago")]
        [Required(ErrorMessage = "Campo Tipo es obligatorio")]
        public int? PaymentMediaId { get; set; }

        [Display(Name = "Medio de Pago")]
        public virtual PaymentMedia PaymentMedia { get; set; }

        public UserReport()
        {
        }
    }
}
