using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace GymTest.Models
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class Workday
    {
        public int WorkdayId { get; set; }

        [Required(ErrorMessage = "Campo Recurso es obligatorio")]
        [ForeignKey("ResourceId")]
        [Display(Name = "Recurso")]
        public int ResourceId { get; set; }

        public virtual Resource Resource { get; set; }

        [Display(Name = "Fecha de jornada")]
        [Required(ErrorMessage = "Campo Fecha de jornada de trabajo es obligatorio")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime WorkingDate { get; set; }

        [Display(Name = "Cantidad Horas")]
        [Required(ErrorMessage = "Campo Cantidad Horas es obligatorio.")]
        public int QuantityOne { get; set; }

        [Display(Name = "Monto Total ($)")]
        [Required(ErrorMessage = "Campo Monto Total es obligatorio.")]
        public int QuantityTwo { get; set; }

        public Workday()
        {
        }
    }
}
