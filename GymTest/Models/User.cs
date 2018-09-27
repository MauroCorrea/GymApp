using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace GymTest.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required (ErrorMessage = "Campo nombre de usuario es obligatorio")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "El nombre de usuario debe ser entre 6 y 50 caracteres de largo")]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Display(Name = "Fecha Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Número de documento")]
        public string DocumentNumber { get; set; }

        [Required]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        [StringLength(200)]
        public string Phones { get; set; }

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
