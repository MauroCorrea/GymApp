using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace GymTest.Models
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class Resource
    {
        public int ResourceId { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Campo nombre de recurso es obligatorio")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "El nombre de recurso debe ser entre 6 y 100 caracteres de largo")]
        public string FullName { get; set; }

        [Display(Name = "Fecha Nacimiento")]
        [Required(ErrorMessage = "Campo Fecha de nacimiento de recurso es obligatorio")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Correo electrónico de recurso es obligatorio")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [StringLength(200)]
        [Display(Name = "Dirección")]
        public string Address { get; set; }

        [StringLength(200)]
        [Required(ErrorMessage = "Teléfono de recurso es obligatorio")]
        [Display(Name = "Teléfono")]
        public string Phones { get; set; }

        [Required(ErrorMessage = "Campo rol es obligatorio")]
        [ForeignKey("RoleId")]
        [Display(Name = "Rol")]
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }

        public Resource()
        {
        }
    }
}
