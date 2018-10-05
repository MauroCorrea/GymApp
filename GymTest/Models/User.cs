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

        [Required(ErrorMessage = "Campo nombre de usuario es obligatorio")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "El nombre de usuario debe ser entre 6 y 100 caracteres de largo")]
        public string FullName { get; set; }

        [Display(Name = "Fecha nacimiento")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Campo número de documento es obligatorio")]
        [StringLength(20)]
        [Display(Name = "Número de documento")]
        public string DocumentNumber { get; set; }

        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(200)]
        public string Phones { get; set; }

        [Required(ErrorMessage = "La fecha de ingreso es obligatoria")]
        [Display(Name = "Fecha ingreso")]
        [DataType(DataType.Date)]
        public DateTime SignInDate { get; set; }

        [StringLength(20)]
        [Display(Name = "Comentarios")]
        public string Commentaries { get; set; }

        public ICollection<Payment> Payments { get; set; }

        public User()
        {
        }
    }
}
