using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace GymTest.Models
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class Schedule
    {
        [Required]
        public int ScheduleId { get; set; }

        [Required(ErrorMessage = "La selección de la cancha es obligatoria")]
        [ForeignKey("FielId")]
        [Display(Name = "Cancha")]
        public int FieldId { get; set; }

        public virtual Field Field { get; set; }

        [Required(ErrorMessage = "La fecha de la reserva es obligatoria")]
        [Display(Name = "Fecha de reserva")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime ScheduleDate { get; set; }

        [Required]
        [Display(Name = "Hora Inicio")]
        public string StartTime { get; set; }

        [Required]
        [Display(Name = "Cantidad de Horas")]
        public short HourQuantity { get; set; }

        [Required(ErrorMessage = "Campo monto es obligatorio")]
        [DataType(DataType.Currency)]
        [Display(Name = "Monto")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public int Amount { get; set; }

        [Required]
        [Display(Name = "A nombre de")]
        public string ClientName { get; set; }

        [Display(Name = "Teléfono")]
        public string ClientPhoneNumber { get; set; }

        [Display(Name = "Pago?")]
        public bool isPayed { get; set; }


        public Schedule()
        {
        }
    }
}
