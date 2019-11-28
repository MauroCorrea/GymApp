using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymTest.Models
{
    public class Assistance
    {
        [Required]
        public int AssistanceId { get; set; }

        [Display(Name = "Fecha Asistencia")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime AssistanceDate { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public Assistance()
        {
        }
    }
}
