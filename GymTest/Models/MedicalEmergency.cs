using System.ComponentModel.DataAnnotations;

namespace GymTest.Models
{
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
