﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymTest.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Campo TOKEN es obligatorio")]
        public string Token { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Campo nombre de usuario es obligatorio")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "El nombre de usuario debe ser entre 6 y 100 caracteres de largo")]
        public string FullName { get; set; }

        [Display(Name = "Fecha Nacimiento")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Campo número de documento es obligatorio")]
        [StringLength(20)]
        [Display(Name = "Núm. Documento")]
        public string DocumentNumber { get; set; }

        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [StringLength(200)]
        [Display(Name = "Dirección")]
        public string Address { get; set; }

        [StringLength(200)]
        [Display(Name = "Teléfono")]
        public string Phones { get; set; }

        [Display(Name = "Nombre de contacto")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "El nombre de usuario debe ser entre 6 y 100 caracteres de largo")]
        public string ContactFullName { get; set; }

        [StringLength(200)]
        [Display(Name = "Teléfono de contacto")]
        public string ContactPhones { get; set; }

        [Required(ErrorMessage = "La fecha de ingreso es obligatoria")]
        [Display(Name = "Fecha Ingreso")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime SignInDate { get; set; }

        [ForeignKey("MedicalEmergencyId")]
        [Display(Name = "Emergencia médica")]
        public int MedicalEmergencyId { get; set; }

        public virtual MedicalEmergency MedicalEmergency { get; set; }

        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Problemas físicos")]
        public string HealthPhysicalProblems { get; set; }

        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Problemas cardiovasculares/corazón")]
        public string HealthHeartProblems { get; set; }

        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Enfermedades crónicas")]
        public string HealthCronicalProblems { get; set; }

        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Medicación regular")]
        public string HealthRegularPills { get; set; }


        [StringLength(200)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Objetivos buscados")]
        public string Target { get; set; }

        [StringLength(200)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Comentarios")]
        public string Commentaries { get; set; }

        public ICollection<Payment> Payments { get; set; }

        public User()
        {
        }
    }
}
