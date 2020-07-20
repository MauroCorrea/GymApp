using System;
using System.ComponentModel.DataAnnotations;

namespace GymTest.Models
{
    public class RegisterUser
    {
        [Display(Name = "Token")]
        [Required(ErrorMessage = "Campo token es obligatorio")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "El token debe ser entre 3 y 10 caracteres de largo")]
        public string Token { get; set; }

        [StringLength(20)]
        [Required(ErrorMessage = "Campo Número de documento es obligatorio. Solo números sin puntos ni guiones.")]
        [Display(Name = "Núm. Documento")]
        public string DocumentNumber { get; set; }

        [Required(ErrorMessage = "Campo Clase es obligatorio")]
        [Display(Name = "Clase")]
        public int ScheduleId { get; set; }

        public RegisterUser()
        {
        }
    }
}
