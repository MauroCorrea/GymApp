using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymTest.Models
{
    public class Action
    {
        public int ActionId { get; set; }

        [Required(ErrorMessage = "Campo tarea es obligatorio")]
        [ForeignKey("TaskId")]
        [Display(Name = "Tarea")]
        public int TaskId { get; set; }

        public virtual Task Task { get; set; }

        [Required(ErrorMessage = "Campo kilometraje es obligatorio")]
        [Display(Name = "Kilometraje")]
        public int Km { get; set; }

        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Comentarios")]
        public string Commentaries { get; set; }

        public Action()
        {
        }
    }
}
