using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace GymTest.Models
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class ScheduleMassively
    {
        [Required]
        public int ScheduleMassivelyId { get; set; }

        [Required(ErrorMessage = "Campo Disciplina es obligatorio")]
        [ForeignKey("DisciplineId")]
        [Display(Name = "Discipline")]
        public int DisciplineId { get; set; }

        public virtual Discipline Discipline { get; set; }

        [Required]
        [Display(Name = "Hora Inicio")]
        public string StartTime { get; set; }

        [Required]
        [Display(Name = "Hora Fin")]
        public string EndTime { get; set; }

        [Required(ErrorMessage = "Campo Recurso es obligatorio")]
        [ForeignKey("ResourceId")]
        [Display(Name = "Recurso")]
        public int ResourceId { get; set; }

        [Display(Name = "Recurso")]
        public virtual Resource Resource { get; set; }

        [Required]
        [Display(Name = "Cupos")]
        public int Places { get; set; }

        [Required(ErrorMessage = "La fecha de la clase es obligatoria")]
        [Display(Name = "Fecha de inicio")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime DataFormatStartString { get; set; }

        [Required(ErrorMessage = "La fecha de la clase es obligatoria")]
        [Display(Name = "Fecha de fin")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime DataFormatEndString { get; set; }

        public ScheduleMassively()
        {
        }
    }
}
