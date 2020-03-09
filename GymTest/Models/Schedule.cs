using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymTest.Models
{
    public class Schedule
    {
        [Required]
        public int ScheduleId { get; set; }

        [Required(ErrorMessage = "Campo Disciplina es obligatorio")]
        [ForeignKey("DisciplineId")]
        [Display(Name = "Discipline")]
        public int DisciplineId { get; set; }

        public virtual Discipline Discipline { get; set; }

        [Required]
        [Display(Name = "Fecha Inicio")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Fecha Fin")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Lunes")]
        public bool Monday { get; set; }
        [Display(Name = "Martes")]
        public bool Tuesday { get; set; }
        [Display(Name = "Miercoles")]
        public bool Wednesday { get; set; }
        [Display(Name = "Jueves")]
        public bool Thursday { get; set; }
        [Display(Name = "Viernes")]
        public bool Friday { get; set; }
        [Display(Name = "Sabado")]
        public bool Saturday { get; set; }
        [Display(Name = "Domingo")]
        public bool Sunday { get; set; }

        [Required]
        [Display(Name = "Hora Inicio")]
        public string StartTime { get; set; }

        [Required]
        [Display(Name = "Hora Fin")]
        public string EndTime { get; set; }


        public Schedule()
        {
        }
    }
}
