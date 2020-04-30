﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GymTest.Models
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class MedicalEmergency
    {
        public int MedicalEmergencyId { get; set; }

        [Required(ErrorMessage = "Campo descripción es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Descripción")]
        public string MedicalEmergencyDescription { get; set; }

        public MedicalEmergency()
        {
        }
    }
}
