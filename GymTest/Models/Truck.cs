using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymTest.Models
{
    public class Truck
    {
        public int TruckId { get; set; }

        [Required(ErrorMessage = "Campo matrícula es obligatorio")]
        [StringLength(10)]
        [Display(Name = "Matrícula")]
        public string Placa { get; set; }

        //[DisplayFormat(DataFormatString = "km {0:n}")]
        [Required(ErrorMessage = "Campo año es obligatorio")]
        [Display(Name = "Año")]
        public int Year { get; set; }

        [StringLength(50)]
        [Display(Name = "Número chasis")]
        public string Chasis { get; set; }

        [StringLength(50)]
        [Display(Name = "Número motor")]
        public string Engine { get; set; }

        [Required(ErrorMessage = "Campo marca es obligatorio")]
        [ForeignKey("MakeId")]
        [Display(Name = "Marca")]
        public int MakeId { get; set; }

        public virtual Make Make { get; set; }

        [Required(ErrorMessage = "Campo modelo es obligatorio")]
        [ForeignKey("ModelId")]
        [Display(Name = "Modelo")]
        public int ModelId { get; set; }

        public virtual Model Model { get; set; }

        [Required(ErrorMessage = "Campo color es obligatorio")]
        [ForeignKey("ColorId")]
        [Display(Name = "Color")]
        public int ColorId { get; set; }

        public virtual Color Color { get; set; }

        public Truck()
        {
        }
    }
}
