using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required(ErrorMessage = "La fecha de ingreso es obligatoria")]
        [Display(Name = "Fecha Ingreso")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime SignInDate { get; set; }

        [StringLength(200)]
        [Display(Name = "Comentarios")]
        public string Commentaries { get; set; }

        public ICollection<Payment> Payments { get; set; }

        public User()
        {
        }
    }
}
