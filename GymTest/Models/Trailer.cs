using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymTest.Models
{
    public class Trailer
    {
        public int TrailerId { get; set; }

        [Required(ErrorMessage = "Campo tipo es obligatorio")]
        [ForeignKey("TrailerTypeId")]
        [Display(Name = "Tipo")]
        public int TrailerTypeId { get; set; }

        public virtual TrailerType TrailerType { get; set; }

        [Required(ErrorMessage = "Campo actividad es obligatorio")]
        [ForeignKey("ActivityId")]
        [Display(Name = "Actividad")]
        public int ActivityId { get; set; }

        public virtual Activity Activity { get; set; }

        public Trailer()
        {
        }
    }
}
